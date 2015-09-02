using System;
using Infrastructure.Cassandra.Migrations;
using Infrastructure.Messaging;
using Skimur;

namespace Subs.Worker.Cons
{
    class Program
    {
        static void Main(string[] args)
        {
            SkimurContext.ContainerInitialized += Migrations.Run;
            SkimurContext.Initialize(new Infrastructure.Registrar(),
                new Infrastructure.Settings.Registrar(),
                new Infrastructure.Caching.Registrar(),
                new Infrastructure.Membership.Registrar(),
                new Infrastructure.Email.Registrar(),
                new Infrastructure.Messaging.Registrar(),
                new Infrastructure.Messaging.RabbitMQ.Registrar(),
                new Infrastructure.Cassandra.Registrar(),
                new Infrastructure.Postgres.Registrar(),
                new Infrastructure.Logging.Registrar(),
                new Skimur.Markdown.Registrar(),
                new Subs.Registrar(),
                new Registrar());
            
            using (SkimurContext.Resolve<IBusLifetime>())
            {
                Console.WriteLine("Press enter to end process...");
                Console.ReadLine();
            }
        }
    }
}
