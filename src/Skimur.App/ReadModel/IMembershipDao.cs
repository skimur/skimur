using System;
using System.Collections.Generic;

namespace Skimur.App.ReadModel
{
    public interface IMembershipDao
    {
        User GetUserById(Guid userId);
        
        User GetUserByUserName(string userName);
        
        User GetUserByEmail(string emailAddress);
        
        List<User> GetUsersByIds(List<Guid> ids);
    }
}
