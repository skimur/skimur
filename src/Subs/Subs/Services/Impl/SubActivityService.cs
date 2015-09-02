using System;
using System.Linq;
using Cassandra;
using Cassandra.Data.Linq;
using Cassandra.Mapping;
using Infrastructure.Membership;
using Infrastructure.Settings;

namespace Subs.Services.Impl
{
    public class SubActivityService : ISubActivityService
    {
        private readonly ISession _session;
        private readonly ISettingsProvider<SubSettings> _subSettings;
        private PreparedStatement _insertStatement;
        private PreparedStatement _countStatement;
        
        public SubActivityService(ISession session, ISettingsProvider<SubSettings> subSettings)
        {
            _session = session;
            _subSettings = subSettings;
        }

        public virtual void MarkSubActive(Guid userId, Guid subId)
        {
            if (_insertStatement == null)
                _insertStatement = _session.Prepare(
                "INSERT INTO ActivityBySub (subId, accountId) " +
                "VALUES (?, ?) USING TTL ?;");

            var bound = _insertStatement.Bind(subId, userId, _subSettings.Settings.ActivityExpirationSeconds);
            _session.Execute(bound);
        }

        public virtual int GetActiveNumberOfUsersForSub(Guid subId)
        {
            if (_countStatement == null)
                _countStatement = _session.Prepare("SELECT COUNT(*) FROM ActivityBySub WHERE subId = ?");

            var bound = _countStatement.Bind(subId);

            return (int)_session.Execute(bound).GetRows().First().GetValue<long>("count");
        }
    }
}
