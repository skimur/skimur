using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Membership.ReadModel
{
    public interface IMembershipDao
    {
        User GetUserById(Guid userId);
        
        User GetUserByUserName(string userName);
        
        User GetUserByEmail(string emailAddress);
        
        List<User> GetUsersByIds(List<Guid> ids);
    }
}
