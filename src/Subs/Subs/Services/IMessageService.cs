using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Subs.Services
{
    public interface IMessageService
    {
        void InsertMessage(Message message);

        int GetNumberOfUnreadMessagesForUser(Guid userId);
    }
}
