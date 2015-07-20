using System;
using Infrastructure.Messaging;

namespace Subs.Worker.Cons
{
    class Program
    {
        static void Main(string[] args)
        {
            Skimur.SkimurContext.Initialize(new Infrastructure.Registrar(),
                new Infrastructure.Membership.Registrar(),
                new Infrastructure.Email.Registrar(),
                new Infrastructure.Messaging.Registrar(),
                new Infrastructure.Messaging.RabbitMQ.Registrar(),
                new Skimur.Markdown.Registrar(),
                new Subs.Registrar(),
                new Registrar());

            using (Skimur.SkimurContext.Resolve<IBusLifetime>())
            {
                Console.WriteLine("Press enter to end process...");
                Console.ReadLine();
            }
        }
    }
}
