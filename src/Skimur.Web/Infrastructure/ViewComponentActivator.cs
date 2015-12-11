using Microsoft.AspNet.Html.Abstractions;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Controllers;
using Microsoft.AspNet.Mvc.Diagnostics;
using Microsoft.AspNet.Mvc.Infrastructure;
using Microsoft.AspNet.Mvc.ViewComponents;
using Microsoft.AspNet.Mvc.ViewFeatures;
using Microsoft.AspNet.Mvc.ViewFeatures.Logging;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;

namespace Skimur.Web.Infrastructure
{
    public class ViewComponentInvokerFactory : IViewComponentInvokerFactory
    {
        private readonly ITypeActivatorCache _typeActivatorCache;
        private readonly IViewComponentActivator _viewComponentActivator;
        private readonly ILogger _logger;
        private readonly DiagnosticSource _diagnosticSource;

        public ViewComponentInvokerFactory(
            ITypeActivatorCache typeActivatorCache,
            IViewComponentActivator viewComponentActivator,
            DiagnosticSource diagnosticSource,
            ILoggerFactory loggerFactory)
        {
            _typeActivatorCache = typeActivatorCache;
            _viewComponentActivator = viewComponentActivator;
            _diagnosticSource = diagnosticSource;

            _logger = loggerFactory.CreateLogger<DefaultViewComponentInvoker>();
        }

        /// <inheritdoc />
        // We don't currently make use of the descriptor or the arguments here (they are available on the context).
        // We might do this some day to cache which method we select, so resist the urge to 'clean' this without
        // considering that possibility.
        public IViewComponentInvoker CreateInstance(ViewComponentContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            return new Invoker(
                _typeActivatorCache,
                _viewComponentActivator,
                _diagnosticSource,
                _logger);
        }
    }

    class Invoker : IViewComponentInvoker
    {
        private readonly ITypeActivatorCache _typeActivatorCache;
        private readonly IViewComponentActivator _viewComponentActivator;
        private readonly DiagnosticSource _diagnosticSource;
        private readonly ILogger _logger;

        public Invoker(
            ITypeActivatorCache typeActivatorCache,
            IViewComponentActivator viewComponentActivator,
            DiagnosticSource diagnosticSource,
            ILogger logger)
        {
            if (typeActivatorCache == null)
            {
                throw new ArgumentNullException(nameof(typeActivatorCache));
            }

            if (viewComponentActivator == null)
            {
                throw new ArgumentNullException(nameof(viewComponentActivator));
            }

            if (diagnosticSource == null)
            {
                throw new ArgumentNullException(nameof(diagnosticSource));
            }

            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            _typeActivatorCache = typeActivatorCache;
            _viewComponentActivator = viewComponentActivator;
            _diagnosticSource = diagnosticSource;
            _logger = logger;
        }

        public void Invoke(ViewComponentContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            //var method = ViewComponentMethodSelector.FindSyncMethod(
            //    context.ViewComponentDescriptor.Type.GetTypeInfo(),
            //    context.Arguments);
            var method = context.ViewComponentDescriptor.Type.GetMethods().First(x => x.Name == "Invoke");
            if (method == null)
            {
                throw new InvalidOperationException("SyncMethodName");
            }

            var result = InvokeSyncCore(method, context);

            result.Execute(context);
        }

        public async Task InvokeAsync(ViewComponentContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            IViewComponentResult result;

            var asyncMethod = ViewComponentMethodSelector.FindAsyncMethod(
                context.ViewComponentDescriptor.Type.GetTypeInfo(),
                context.Arguments);
            if (asyncMethod == null)
            {
                // We support falling back to synchronous if there is no InvokeAsync method, in this case we'll still
                // execute the IViewResult asynchronously.
                var syncMethod = ViewComponentMethodSelector.FindSyncMethod(
                    context.ViewComponentDescriptor.Type.GetTypeInfo(),
                    context.Arguments);
                if (syncMethod == null)
                {
                    throw new InvalidOperationException("AsyncMethodName");
                }
                else
                {
                    result = InvokeSyncCore(syncMethod, context);
                }
            }
            else
            {
                result = await InvokeAsyncCore(asyncMethod, context);
            }

            await result.ExecuteAsync(context);
        }

        private object CreateComponent(ViewComponentContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var services = context.ViewContext.HttpContext.RequestServices;
            var component = _typeActivatorCache.CreateInstance<object>(
                services,
                context.ViewComponentDescriptor.Type);
            _viewComponentActivator.Activate(component, context);
            return component;
        }

        private async Task<IViewComponentResult> InvokeAsyncCore(
            MethodInfo method,
            ViewComponentContext context)
        {
            if (method == null)
            {
                throw new ArgumentNullException(nameof(method));
            }

            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var component = CreateComponent(context);

            using (_logger.ViewComponentScope(context))
            {
                _diagnosticSource.BeforeViewComponent(context, component);
                _logger.ViewComponentExecuting(context);

                var startTime = Environment.TickCount;
                var result = await ControllerActionExecutor.ExecuteAsync(method, component, context.Arguments);

                var viewComponentResult = CoerceToViewComponentResult(result);
                _logger.ViewComponentExecuted(context, startTime, viewComponentResult);
                _diagnosticSource.AfterViewComponent(context, viewComponentResult, component);

                return viewComponentResult;
            }
        }

        public IViewComponentResult InvokeSyncCore(MethodInfo method, ViewComponentContext context)
        {
            if (method == null)
            {
                throw new ArgumentNullException(nameof(method));
            }

            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var component = CreateComponent(context);

            using (_logger.ViewComponentScope(context))
            {
                _diagnosticSource.BeforeViewComponent(context, component);
                _logger.ViewComponentExecuting(context);

                try
                {
                    var startTime = Environment.TickCount;
                    var result = method.Invoke(component, context.Arguments);

                    var viewComponentResult = CoerceToViewComponentResult(result);
                    _logger.ViewComponentExecuted(context, startTime, viewComponentResult);
                    _diagnosticSource.AfterViewComponent(context, viewComponentResult, component);

                    return viewComponentResult;
                }
                catch (TargetInvocationException ex)
                {
                    // Preserve callstack of any user-thrown exceptions.
                    var exceptionInfo = ExceptionDispatchInfo.Capture(ex.InnerException);
                    exceptionInfo.Throw();
                    return null; // Unreachable
                }
            }
        }

        private static IViewComponentResult CoerceToViewComponentResult(object value)
        {
            if (value == null)
            {
                throw new InvalidOperationException("MustReturnValue");
            }

            var componentResult = value as IViewComponentResult;
            if (componentResult != null)
            {
                return componentResult;
            }

            var stringResult = value as string;
            if (stringResult != null)
            {
                return new ContentViewComponentResult(stringResult);
            }

            var htmlContent = value as IHtmlContent;
            if (htmlContent != null)
            {
                return new HtmlContentViewComponentResult(htmlContent);
            }

            throw new InvalidOperationException("FormatViewComponent_InvalidReturnValue");
        }
    }

    /// <summary>
    /// An <see cref="IViewComponentResult"/> which writes an <see cref="IHtmlContent"/> when executed.
    /// </summary>
    /// <remarks>
    /// The provided content will be HTML-encoded as specified when the content was created. To encoded and write
    /// text, use a <see cref="ContentViewComponentResult"/>.
    /// </remarks>
    public class HtmlContentViewComponentResult : IViewComponentResult
    {
        /// <summary>
        /// Initializes a new <see cref="HtmlContentViewComponentResult"/>.
        /// </summary>
        public HtmlContentViewComponentResult(IHtmlContent encodedContent)
        {
            if (encodedContent == null)
            {
                throw new ArgumentNullException(nameof(encodedContent));
            }

            EncodedContent = encodedContent;
        }

        /// <summary>
        /// Gets the encoded content.
        /// </summary>
        public IHtmlContent EncodedContent { get; }

        /// <summary>
        /// Writes the <see cref="EncodedContent"/>.
        /// </summary>
        /// <param name="context">The <see cref="ViewComponentContext"/>.</param>
        public void Execute(ViewComponentContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var htmlWriter = context.Writer as HtmlTextWriter;
            if (htmlWriter == null)
            {
                throw new Exception("");
                //EncodedContent.WriteTo(context.Writer, context.HtmlEncoder);
            }
            else
            {
                htmlWriter.Write(EncodedContent);
            }
        }

        /// <summary>
        /// Writes the <see cref="EncodedContent"/>.
        /// </summary>
        /// <param name="context">The <see cref="ViewComponentContext"/>.</param>
        /// <returns>A completed <see cref="Task"/>.</returns>
        public Task ExecuteAsync(ViewComponentContext context)
        {
            Execute(context);

            return Microsoft.AspNet.Mvc.Internal.TaskCache.CompletedTask;
        }
    }
}
