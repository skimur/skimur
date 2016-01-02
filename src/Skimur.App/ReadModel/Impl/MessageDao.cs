using Skimur.App.Services.Impl;
using Skimur.Data;

namespace Skimur.App.ReadModel.Impl
{
    public class MessageDao
        // this class temporarily implements the service, until we implement the proper read-only layer
        : MessageService, IMessageDao
    {
        public MessageDao(IDbConnectionProvider conn)
            :base(conn)
        {
            
        }
    }
}
