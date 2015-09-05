using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Data;
using ServiceStack.OrmLite;

namespace Subs.Services.Impl
{
    public class MessageService : IMessageService
    {
        private readonly IDbConnectionProvider _conn;

        public MessageService(IDbConnectionProvider conn)
        {
            _conn = conn;
        }

        public void InsertMessage(Message message)
        {
            _conn.Perform(conn =>
            {
                conn.Insert(message);
            });
        }

        public int GetNumberOfUnreadMessagesForUser(Guid userId)
        {
            return _conn.Perform(conn =>
            {
                return (int)conn.Count<Message>(x => x.ToUser == userId && x.IsNew);
            });
        }
    }
}
