using Skimur.Data;
using Subs.Services.Impl;

namespace Subs.ReadModel.Impl
{
    public class ReportDao : ReportService, IReportDao
    {
        public ReportDao(IDbConnectionProvider conn)
            :base(conn)
        {
            
        }
    }
}
