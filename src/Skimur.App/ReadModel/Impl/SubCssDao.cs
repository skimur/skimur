using Skimur.Data;
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
