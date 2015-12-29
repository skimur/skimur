using Skimur.App.Services.Impl;
using Skimur.Data;

namespace Skimur.App.ReadModel.Impl
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
