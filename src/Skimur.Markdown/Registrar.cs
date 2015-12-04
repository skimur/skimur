using JavaScriptEngineSwitcher.Core;
using JavaScriptEngineSwitcher.V8;
using JavaScriptEngineSwitcher.V8.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Skimur.Markdown
{
    public class Registrar : IRegistrar
    {
        public void Register(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<IJsEngine>(provider => new V8JsEngine());
            serviceCollection.AddSingleton<IMarkdownCompiler, MarkdownCompiler>();
        }

        public int Order { get { return 0; } }
    }
}
