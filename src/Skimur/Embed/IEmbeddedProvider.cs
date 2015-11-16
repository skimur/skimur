using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skimur.Embed
{
    /// <summary>
    /// A provider used to build preview objects
    /// </summary>
    public interface IEmbeddedProvider
    {
        /// <summary>
        /// Is this provider enabled?
        /// </summary>
        bool IsEnabled { get; }

        /// <summary>
        /// Attempt to embed a given url
        /// </summary>
        /// <param name="url">The url to embed</param>
        /// <returns></returns>
        IEmbeddedResult Embed(string url);
    }
    
    public interface IEmbeddedResult
    {
        /// <summary>
        /// The type of this result
        /// </summary>
        EmbeddedResultType Type { get; }

        /// <summary>
        /// The name of the provider used to build this result.
        /// For example, "YouTube", "Imgur", etc.
        /// </summary>
        string ProviderName { get; }

        /// <summary>
        /// The url for this result.
        /// Used for high-res photo displays, or <iframe /> results.
        /// </summary>
        string Url { get; }

        /// <summary>
        /// The html to display when the html type is used.
        /// </summary>
        string Html { get; }

        /// <summary>
        /// The width needed to render the content. NULL if fluid.
        /// </summary>
        int? Width { get; }
        
        /// <summary>
        /// The height needed to render the content. NULL if fluid
        /// </summary>
        int? Height { get; }
    }

    public enum EmbeddedResultType
    {
        /// <summary>
        /// The URL parameter represents a high-res image to display
        /// </summary>
        Photo = 0,
        /// <summary>
        /// The HTML property should be used to render html content
        /// </summary>
        Html = 1,
        /// <summary>
        /// The Url represents a page that should be loaded in an <iframe /> element,
        /// using the URL parameter
        /// </summary>
        // ReSharper disable once InconsistentNaming
        IFrame = 2
    }
}
