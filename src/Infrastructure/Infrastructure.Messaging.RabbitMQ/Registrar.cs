using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EasyNetQ;
using SimpleInjector;
using Skimur;

namespace Infrastructure.Messaging.RabbitMQ
{
    public class Registrar : IRegistrar
    {
        public void Register(Container container)
        {
            container.RegisterSingle(() =>
            {
                var rabbitMqHost = ConfigurationManager.AppSettings["RabbitMQHost"];
                if (string.IsNullOrEmpty(rabbitMqHost)) throw new Exception("You must provide a 'RabbitMQHost' app setting.");
                return RabbitHutch.CreateBus("host=" + rabbitMqHost);
            });
            container.RegisterSingle<ICommandBus, CommandBus>();
            container.RegisterSingle<IEventBus, EventBus>();
            container.RegisterSingle<IBusLifetime, BusLifetime>();
        }

        public int Order
        {
            get { return 0; }
        }
    }
}
