using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Subs.Services
{
    public interface IReportService
    {
        List<Report.CommentReport> GetReportsForComment(Guid commentId);

        List<Report.PostReport> GetReportsForPost(Guid postId);

        void ReportComment(Guid commentId, Guid reportBy, string reason);

        void ReportPost(Guid postId, Guid reportBy, string reason);

        void RemoveReportsForPost(Guid postId);

        void RemoveReportsForComment(Guid commentId);
    }
}
