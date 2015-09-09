using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Data;
using Subs.Services.Impl;

namespace Subs.ReadModel.Impl
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
