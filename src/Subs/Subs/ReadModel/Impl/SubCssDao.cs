using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Data;
using Subs.Services.Impl;

namespace Subs.ReadModel.Impl
{
    public class SubCssDao : 
        SubCssService, ISubCssDao
    {
        public SubCssDao(IDbConnectionProvider conn)
            :base(conn)
        {
            
        }
    }
}
