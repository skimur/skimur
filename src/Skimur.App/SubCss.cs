using System;
using ServiceStack.DataAnnotations;

namespace Skimur.App
{
    [Alias("SubCss")]
    public class SubCss
    {
        public Guid Id { get; set; }

        public Guid SubId { get; set; }

        public int Type { get; set; }

        public string Embedded { get; set; }

        public string ExternalCss { get; set; }

        [Alias("GithubCssProjectName")]
        public string GitHubCssProjectName { get; set; }

        [Alias("GithubCssProjectTag")]
        public string GitHubCssProjectTag { get; set; }

        [Alias("GithubLessProjectName")]
        public string GitHubLessProjectName { get; set; }

        [Alias("GithubLessProjectTag")]
        public string GitHubLessProjectTag { get; set; }

        [Ignore]
        public CssType CssType
        {
            get { return (CssType)Type; }
            set { Type = (int)value; }
        }
    }

    public enum CssType
    {
        /// <summary>
        /// No styles for the sub
        /// </summary>
        None,
        /// <summary>
        /// Css added inline to the page
        /// </summary>
        Embedded,
        /// <summary>
        /// Css that is hosted externally
        /// </summary>
        ExternalCss,
        /// <summary>
        /// Css that is hosted externally on GitHub
        /// </summary>
        GitHubCss,
        /// <summary>
        /// LESS that is hosted externally on GitHub
        /// </summary>
        GitHubLess
    }
}
