using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Infrastructure.Messaging.Handling;
using MassTransit;
using NUnit.Framework;
using SimpleInjector;

namespace Infrastructure.Messaging.MassTransit.IntegrationTests
{
    [TestFixture]
    public class EventTests
    {
        [Test]
        public void Can_send_event()
        {
            // arrange
            MassTransit massTransit = null;
            try
            {
                var container = new Container();
                var handler = new TestEventHandler();
                container.Register<IEventHandler<TestEvent>>(() => handler);
                var eventRegistry = new EventHandlerRegistry(container);
                var commandRegistry = new CommandHandlerRegistry(container);
                massTransit = new MassTransit(commandRegistry, eventRegistry, container);
                var eventBus = new EventBus(massTransit);

                // act
                eventBus.Publish(new TestEvent());

                // assert
                Thread.Sleep(TimeSpan.FromSeconds(2));
                Assert.That(handler.NumberOfTimesRan, Is.EqualTo(1));
            }
            finally
            {
                if(massTransit != null)
                    massTransit.Dispose();
            }
        }

        public class TestEvent : IEvent
        {
            public Guid SourceId { get; set; }

            public int Message { get; set; }
        }

        public class TestEventHandler : IEventHandler<TestEvent>
        {
            public int NumberOfTimesRan { get; set; }

            public void Handle(TestEvent command)
            {
                NumberOfTimesRan++;
            }
        }
    }
}
