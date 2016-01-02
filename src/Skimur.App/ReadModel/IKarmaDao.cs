using System;
using System.Collections.Generic;
using Skimur.App.Services;

namespace Skimur.App.ReadModel
{
    public interface IKarmaDao
    {
        Dictionary<KarmaReportKey, int> GetKarma(Guid userId);
    }
}
