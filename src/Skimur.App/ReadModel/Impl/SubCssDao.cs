using Skimur.App.Services.Impl;
using Skimur.Data;

namespace Skimur.App.ReadModel.Impl
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
