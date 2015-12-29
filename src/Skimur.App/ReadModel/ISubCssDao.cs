using System;

namespace Skimur.App.ReadModel
{
    public interface ISubCssDao
    {
        SubCss GetStylesForSub(Guid subId);
    }
}
