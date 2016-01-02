using System;
using System.Collections.Generic;
using Cassandra;
using Cassandra.Data.Linq;
using Cassandra.Mapping;

namespace Skimur.App.Services.Impl
{
    public class KarmaService : IKarmaService
    {
        private readonly ISession _session;
        private PreparedStatement _adjustStatement;
        private Table<UserKarma> _table;

        static KarmaService()
        {
            MappingConfiguration.Global.Define<KarmaMappings>();
        }

        public KarmaService(ISession session)
        {
            _session = session;
            _table = new Table<UserKarma>(_session);
        }

        public void AdjustKarma(Guid userId, Guid subId, KarmaType karmaType, int change)
        {
            if (change == 0) return;
            EnsureStatementsReady();
            _session.Execute(_adjustStatement.Bind((long)change, userId, subId + "-" + karmaType).SetConsistencyLevel(ConsistencyLevel.All));
        }

        public void DeleteAllKarmaForUser(Guid userId)
        {
            EnsureStatementsReady();
            // cassandra doesn't allow deleting columns that are counters.
            // http://stackoverflow.com/questions/31780331/how-to-re-insert-record-with-counter-column-after-delete-it-in-cassandra
            // so, we are going to just adjust the counters so that reach 0.
            var report = GetKarma(userId);
            if (report.Count == 0) return;
            foreach (var karma in report)
                AdjustKarma(userId, karma.Key.SubId, karma.Key.Type, -karma.Value);
        }

        public void IncreaseKarma(Guid userId, Guid subId, KarmaType karmaType)
        {
            AdjustKarma(userId, subId, karmaType, 1);
        }

        public void DecreaseKarma(Guid userId, Guid subId, KarmaType karmaType)
        {
            AdjustKarma(userId, subId, karmaType, -1);
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
            if (_adjustStatement == null)
                _adjustStatement = _session.Prepare("UPDATE UserKarma SET karma = karma + ? where user_id = ? and sub_type = ?;");
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
