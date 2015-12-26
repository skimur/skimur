using Microsoft.Extensions.DependencyInjection;

namespace Skimur.Markdown
{
    public class Registrar : IRegistrar
    {
        public void Register(IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IMarkdownCompiler, MarkdownCompiler>();
        }

        public int Order { get { return 0; } }
    }
}
