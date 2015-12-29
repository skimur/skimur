using Skimur.Data;
using Subs.Services.Impl;

namespace Subs.ReadModel.Impl
{
    public class ModerationInviteDao :
        ModerationInviteService, IModerationInviteDao
    {
        public ModerationInviteDao(IDbConnectionProvider conn)
            : base(conn)
        {

        }
    }
}
