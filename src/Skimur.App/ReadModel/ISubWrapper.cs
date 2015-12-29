using System;
using System.Collections.Generic;

namespace Skimur.App.ReadModel
{
    public interface ISubWrapper
    {
        List<SubWrapped> Wrap(List<Guid> subIds, User currentUser = null);

        SubWrapped Wrap(Guid subId, User currentUser = null);

        List<SubWrapped> Wrap(List<Sub> subs, User currentUser = null);

        SubWrapped Wrap(Sub sub, User currentUser = null);
    }
}
