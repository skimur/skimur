using System.Collections.Generic;

namespace Skimur.App.ReadModel
{
    public interface ISubUserBanWrapper
    {
        List<SubUserBanWrapped> Wrap(List<SubUserBan> items);
    }
}
