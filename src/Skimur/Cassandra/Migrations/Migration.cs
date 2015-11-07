using Cassandra;

namespace Skimur.Cassandra.Migrations
{
    public abstract class Migration
    {
        protected Migration(MigrationType type, int version)
        {
            Type = type;
            Version = version;
        }
        
        public MigrationType Type { get; private set; }
        
        public int Version { get; private set; }
        
        public abstract void Execute(ISession session);
        
        public abstract string GetDescription();
    }
}
