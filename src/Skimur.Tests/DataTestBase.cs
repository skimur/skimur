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

        [Alias("ensure_default_user_exists")]
        public class EnsureDefaultUserFunction
        {
            
        }

        protected override void Setup()
        {
            base.Setup();

            Infrastructure.Cassandra.Migrations.Migrations.Run(_container);
            Infrastructure.Postgres.Migrations.Migrations.Run(_container);

            _conn = _container.GetInstance<IDbConnectionProvider>();
            _conn.Perform(conn =>
            {
                conn.ExecuteProcedure(new ClearDatabaseFunction());
                conn.ExecuteProcedure(new EnsureDefaultUserFunction());
            });
        }

        protected override List<IRegistrar> GetRegistrars()
        {
            var result = base.GetRegistrars();

            result.AddRange(new List<IRegistrar>
            {
                new Infrastructure.Registrar(),
                new Infrastructure.Settings.Registrar(),
                new Infrastructure.Caching.Registrar(),
                new Infrastructure.Email.Registrar(),
                new Infrastructure.Messaging.Registrar(),
                new Infrastructure.Messaging.RabbitMQ.Registrar(),
                new Infrastructure.Cassandra.Registrar(),
                new Infrastructure.Postgres.Registrar(),
                new Infrastructure.Logging.Registrar(),
                new Markdown.Registrar(),
                new Scraper.Registrar(),
                new Subs.Registrar(),
                new Subs.Worker.Registrar(),
                new Membership.Registrar()
            });

            return result;
        }
    }
}
