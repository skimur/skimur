﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Subs.Services
{
    public interface ISubCssService
    {
        SubCss GetStylesForSub(Guid subId);

        void UpdateStylesForSub(SubCss styles);
    }
}