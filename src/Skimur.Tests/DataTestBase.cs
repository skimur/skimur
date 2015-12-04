using System.Collections.Generic;
using ServiceStack.DataAnnotations;
using ServiceStack.OrmLite;
using Skimur.Data;
using Microsoft.Extensions.DependencyInjection;

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

            Cassandra.Migrations.Migrations.Run(_serviceProvider);
            Postgres.Migrations.Migrations.Run(_serviceProvider);

            _conn = _serviceProvider.GetService<IDbConnectionProvider>();
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
