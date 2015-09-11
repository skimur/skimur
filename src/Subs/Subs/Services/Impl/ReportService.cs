using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Data;
using ServiceStack.OrmLite;
using Skimur;

namespace Subs.Services.Impl
{
    public class ReportService : IReportService
    {
        private readonly IDbConnectionProvider _conn;

        public ReportService(IDbConnectionProvider connectionProvider)
        {
            _conn = connectionProvider;
        }

        public List<Report.CommentReport> GetReportsForComment(Guid commentId)
        {
            return _conn.Perform(conn => conn.Select(conn.From<Report.CommentReport>().Where(x => x.CommentId == commentId)));
        }

        public List<Report.PostReport> GetReportsForPost(Guid postId)
        {
            return _conn.Perform(conn => conn.Select(conn.From<Report.PostReport>().Where(x => x.PostId == postId)));
        }

        public void ReportComment(Guid commentId, Guid reportBy, string reason)
        {
            _conn.Perform(conn => conn.Insert(new Report.CommentReport
            {
                CreatedDate = Common.CurrentTime(),
                ReportedBy = reportBy,
                Reason = reason,
                CommentId = commentId
            }));
        }

        public void ReportPost(Guid postId, Guid reportBy, string reason)
        {
            _conn.Perform(conn => conn.Insert(new Report.PostReport
            {
                CreatedDate = Common.CurrentTime(),
                ReportedBy = reportBy,
                Reason = reason,
                PostId = postId
            }));
        }

        public void RemoveReportsForPost(Guid postId)
        {
            _conn.Perform(conn => conn.Delete<Report.PostReport>(x => x.PostId == postId));
        }

        public void RemoveReportsForComment(Guid commentId)
        {
            _conn.Perform(conn => conn.Delete<Report.CommentReport>(x => x.CommentId == commentId));
        }
    }
}
