using System;

namespace Skimur.App.Services
{
    public interface ISubCssService
    {
        SubCss GetStylesForSub(Guid subId);

        void UpdateStylesForSub(SubCss styles);
    }
}
