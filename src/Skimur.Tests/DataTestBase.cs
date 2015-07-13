using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Data;
using ServiceStack.DataAnnotations;
using ServiceStack.OrmLite;

namespace Skimur.Tests
{
    public abstract class DataTestBase : TestBase
    {
        protected IDbConnectionProvider _conn;

        [Alias("clear_database")]
        public class ClearDatabaseFunction
        {
        }

        protected override void Setup()
        {
            base.Setup();
            _conn = _container.GetInstance<IDbConnectionProvider>();
            _conn.Perform(conn => conn.ExecuteProcedure(new ClearDatabaseFunction()));
        }

        protected override List<IRegistrar> GetRegistrars()
        {
            var result = base.GetRegistrars();

            result.AddRange(new List<IRegistrar>
            {
                new Infrastructure.Registrar(),
                new Infrastructure.Membership.Registrar(),
                new Infrastructure.Email.Registrar(),
                new Infrastructure.Messaging.Registrar(),
                new Infrastructure.Messaging.RabbitMQ.Registrar(),
            });

            return result;
        }
    }
}
