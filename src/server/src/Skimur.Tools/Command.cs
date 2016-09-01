using Microsoft.Extensions.CommandLineUtils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.IO;

namespace Skimur.Tools
{
    public class Command : IRegistrar
    {
        public int Order => 0;

        public void Register(IServiceCollection serviceCollection)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");
            
            serviceCollection.AddSingleton<IConfiguration>(builder.Build());
        }

        public static void Initialize(CommandLineApplication app)
        {
            app.Command("migrate", c =>
            {
                c.Description = "Migrate the database";
                c.HelpOption("-?|-h|--help");
                c.OnExecute(() =>
                {
                    SkimurContext.Initialize(new Command());
                    SkimurContext.ServiceProvider.GetService<ILoggerFactory>().AddConsole();
                    Data.Postgres.Migrations.Run(SkimurContext.ServiceProvider).Wait();
                    return 0;
                });
            });
        }
    }
}
