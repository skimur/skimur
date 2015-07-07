using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Subs.ReadModel
{
    public interface ISubDao
    {
        List<Sub> GetAllSubs(string searchText = null);

        List<Sub> GetDefaultSubs();

        List<Sub> GetSubscribedSubsForUser(string userName);

        Sub GetRandomSub();

        Sub GetSubByName(string name);

        List<Sub> GetSubByNames(List<string> names);
    }
}
