using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.CommandLineUtils;

namespace Skimur.Web
{
    public class Program
    {
        public static int Main(string[] args)
        {
            var app = new CommandLineApplication();

            Tools.Command.Initialize(app);

            app.OnExecute(() =>
            {
                var host = new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<Startup>()
                .Build();

                host.Run();

                return 0;
            });

            return app.Execute(args);
        }
    }
}
