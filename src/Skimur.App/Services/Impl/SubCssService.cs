using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceStack.OrmLite;
using Skimur.Data;
using Skimur.Utils;

namespace Subs.Services.Impl
{
    public class SubCssService : ISubCssService
    {
        private readonly IDbConnectionProvider _conn;

        public SubCssService(IDbConnectionProvider conn)
        {
            _conn = conn;
        }

        public SubCss GetStylesForSub(Guid subId)
        {
            return _conn.Perform(conn =>
            {
                return conn.Single<SubCss>(x => x.SubId == subId);
            });
        }

        public void UpdateStylesForSub(SubCss styles)
        {
            _conn.Perform(conn =>
            {
                var existing = conn.Single<SubCss>(x => x.SubId == styles.SubId);
                if (existing != null)
                {
                    existing.CssType = styles.CssType;
                    existing.Embedded = styles.Embedded;
                    existing.ExternalCss = styles.ExternalCss;
                    existing.GitHubCssProjectName = styles.GitHubCssProjectName;
                    existing.GitHubCssProjectTag = styles.GitHubCssProjectTag;
                    existing.GitHubLessProjectName = styles.GitHubLessProjectName;
                    existing.GitHubLessProjectTag = styles.GitHubLessProjectTag;
                    conn.Update(existing);
                }
                else
                {
                    existing = new SubCss();
                    existing.Id = GuidUtil.NewSequentialId();
                    existing.SubId = styles.SubId;
                    existing.CssType = styles.CssType;
                    existing.Embedded = styles.Embedded;
                    existing.ExternalCss = styles.ExternalCss;
                    existing.GitHubCssProjectName = styles.GitHubCssProjectName;
                    existing.GitHubCssProjectTag = styles.GitHubCssProjectTag;
                    existing.GitHubLessProjectName = styles.GitHubLessProjectName;
                    existing.GitHubLessProjectTag = styles.GitHubLessProjectTag;
                    conn.Insert(existing);
                    styles.Id = existing.Id;
                }
            });
        }
    }
}
