using System;
using Skimur;
using Skimur.Messaging;

namespace Subs.Worker.Cons
{
    class Program
    {
        static void Main(string[] args)
        {
            SkimurContext.ContainerInitialized += Skimur.Cassandra.Migrations.Migrations.Run;
            SkimurContext.ContainerInitialized += Skimur.Postgres.Migrations.Migrations.Run;
            SkimurContext.Initialize(new Skimur.Markdown.Registrar(),
                new Skimur.Scraper.Registrar(),
                new Subs.Registrar(),
                new Registrar(),
                new Membership.Registrar());
            
            using (SkimurContext.Resolve<IBusLifetime>())
            {
                Console.WriteLine("Press enter to end process...");
                Console.ReadLine();
            }
        }
    }
}
