using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cassandra;
using Cassandra.Data.Linq;
using Cassandra.Mapping;

namespace Subs.Services.Impl
{
    public class KarmaService : IKarmaService
    {
        private readonly ISession _session;
        private PreparedStatement _increaseStatement;
        private PreparedStatement _decreaseStatement;
        private Table<UserKarma> _table;

        public KarmaService(ISession session)
        {
            _session = session;
            MappingConfiguration.Global.Define<KarmaMappings>();
            _table = new Table<UserKarma>(_session);
        }

        public void IncreaseKarma(Guid userId, Guid subId, KarmaType karmaType)
        {
            EnsureStatementsReady();
            var bound = _increaseStatement.Bind(userId, subId + "-" + karmaType);
            _session.Execute(bound);
        }

        public void DecreaseKarma(Guid userId, Guid subId, KarmaType karmaType)
        {
            EnsureStatementsReady();
            var bound = _decreaseStatement.Bind(userId, subId + "-" + karmaType);
            _session.Execute(bound);
        }

        public Dictionary<KarmaReportKey, int> GetKarma(Guid userId)
        {
            var result = new Dictionary<KarmaReportKey, int>();
            
            foreach (var item in _table.Where(x => x.UserId == userId).Execute())
            {
                var guidString = item.SubType.Substring(0, 36);
                var typestring = item.SubType.Substring(37);

                Guid subId;
                if(!Guid.TryParse(guidString, out subId)) continue; // how?

                KarmaType type;
                if(!Enum.TryParse(typestring, out type)) continue; // how?

                result.Add(new KarmaReportKey(subId, type), item.Karma);
            }

            return result;
        }

        private void EnsureStatementsReady()
        {
            if (_increaseStatement == null)
                _increaseStatement = _session.Prepare("UPDATE UserKarma SET karma = karma + 1 where user_id = ? and sub_type = ?;");
            if (_decreaseStatement == null)
                _decreaseStatement = _session.Prepare("UPDATE UserKarma SET karma = karma - 1 where user_id = ? and sub_type = ?;");
        }

        private class KarmaMappings : Mappings
        {
            public KarmaMappings()
            {
                For<UserKarma>()
                    .TableName("UserKarma")
                    .PartitionKey("user_id")
                    .ClusteringKey("sub_type")
                    .Column(c => c.UserId, cm => cm.WithName("user_id"))
                    .Column(c => c.SubType, cm => cm.WithName("sub_type"))
                    .Column(c => c.Karma, cm => cm.WithName("karma"));
            }
        }

        private class UserKarma
        {
            public Guid UserId { get; set; }

            public string SubType { get; set; }

            public int Karma { get; set; }
        }
    }
}
