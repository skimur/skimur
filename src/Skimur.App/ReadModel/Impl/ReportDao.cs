using Skimur.App.Services.Impl;
using Skimur.Data;

namespace Skimur.App.ReadModel.Impl
{
    public class ReportDao : ReportService, IReportDao
    {
        public ReportDao(IDbConnectionProvider conn)
            :base(conn)
        {
            
        }
    }
}
