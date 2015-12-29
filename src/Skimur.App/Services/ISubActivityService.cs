using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Subs.Services
{
    public interface ISubActivityService
    {
        void MarkSubActive(Guid userId, Guid subId);

        int GetActiveNumberOfUsersForSub(Guid subId);
    }
}
