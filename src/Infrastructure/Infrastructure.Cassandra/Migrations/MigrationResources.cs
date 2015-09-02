using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Cassandra.Migrations
{
    public class MigrationResources
    {
        private readonly LinkedList<Migration> _migrations = new LinkedList<Migration>();

        /// <summary>
        /// Sorted migrations
        /// </summary>
        public ICollection<Migration> Migrations
        {
            get
            {
                return _migrations;
            }
        }

        /// <summary>
        /// Add migration to collection
        /// </summary>
        /// <param name="migration">Migration parameter</param>
        public void Add(Migration migration)
        {
            if (_migrations.Any(m => m.Type == migration.Type && m.Version == migration.Version))
                throw new NotSupportedException("Migration versions for same type must differ");

            _migrations.AddLast(migration);
        }

        /// <summary>
        /// Add multiple migrations to collection
        /// </summary>
        /// <param name="migrations">Migration collection</param>
        public void AddAll(ICollection<Migration> migrations)
        {
            foreach (var migration in migrations)
            {
                Add(migration);
            }
        }
    }
}
