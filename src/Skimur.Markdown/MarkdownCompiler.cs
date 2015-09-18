using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using JavaScriptEngineSwitcher.Core;

namespace Skimur.Markdown
{
    public class MarkdownCompiler : IMarkdownCompiler
    {
        private readonly object _compilationSynchronizer = new object();

        private IJsEngine _jsEngine;
        private bool _initialized;
        private bool _disposed;

        public MarkdownCompiler(IJsEngine jsEngine)
        {
            _jsEngine = jsEngine;
        }

        private void Initialize()
        {
            if (!_initialized)
            {
                var type = GetType();

                _jsEngine.Execute("var exports = {}");

                _jsEngine.ExecuteResource("Skimur.Markdown.markdown.js", type);
                _jsEngine.ExecuteResource("Skimur.Markdown.markdownCompiler.js", type);

                _initialized = true;
            }
        }

        public string Compile(string markdown)
        {
            if (string.IsNullOrEmpty(markdown)) return markdown;

            string result;

            lock (_compilationSynchronizer)
            {
                Initialize();

                _jsEngine.SetVariableValue("_markdownString", markdown);

                result = _jsEngine.Evaluate<string>("markdownHelper.compile(_markdownString)");
            }

            return result;
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;

                if (_jsEngine != null)
                {
                    _jsEngine.Dispose();
                    _jsEngine = null;
                }
            }
        }
    }
}
