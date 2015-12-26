﻿using Membership;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Skimur.Web.ViewModels
{
    public class AccountHeaderViewModel
    {
        public User CurrentUser { get; set; }

        public int NumberOfUnreadMessages { get; set; }
    }
}
