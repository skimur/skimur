using JavaScriptEngineSwitcher.Core;
using JavaScriptEngineSwitcher.V8;
using JavaScriptEngineSwitcher.V8.Configuration;
using SimpleInjector;

namespace Skimur.Markdown
{
    public class Registrar : IRegistrar
    {
        public void Register(Container container)
        {
            container.Register<IJsEngine>(() => new V8JsEngine());
            container.RegisterSingleton<IMarkdownCompiler, MarkdownCompiler>();
        }

        public int Order { get { return 0; } }
    }
}
