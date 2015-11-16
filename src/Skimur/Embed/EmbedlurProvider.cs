using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;
using ServiceStack.Logging;
using Skimur.Logging;
using Skimur.Settings;

namespace Skimur.Embed
{
    public class EmbedlurProvider : IEmbeddedProvider
    {
        private readonly ISettingsProvider<EmbedSettings> _embedSettings;
        private readonly ILogger<EmbedlurProvider> _logger;

        public EmbedlurProvider(ISettingsProvider<EmbedSettings> embedSettings, ILogger<EmbedlurProvider> logger)
        {
            _embedSettings = embedSettings;
            _logger = logger;
        }

        public bool IsEnabled { get { return _embedSettings.Settings.EmbedlurEnabled; } }

        public IEmbeddedResult Embed(string url)
        {
            if (string.IsNullOrEmpty(url)) return null;

            if (!IsEnabled) return null;

            OEmbedJsonResult result;

            try
            {
                result = JsonConvert.DeserializeObject<OEmbedJsonResult>(GetRequest(url));
            }
            catch (WebException ex)
            {
                if (ex.Status == WebExceptionStatus.ProtocolError)
                {
                    if (((HttpWebResponse) ex.Response).StatusCode == HttpStatusCode.NotFound)
                        return null;
                }
                throw;
            }
            catch (HttpException ex)
            {
                if (ex.WebEventCode == (int)HttpStatusCode.NotFound)
                    return null; // the url can't be embeded

                // this was a real error which needs to be handled by the caller.
                throw;
            }

            if (result == null)
                return null;

            int? width = null;
            int? height = null;

            if (!string.IsNullOrEmpty(result.Width))
            {
                int temp;
                if (int.TryParse(result.Width, out temp))
                    width = temp;;
            }

            if (!string.IsNullOrEmpty(result.Height))
            {
                int temp;
                if (int.TryParse(result.Height, out temp))
                    height = temp; ;
            }


            switch (result.Type)
            {
                case "link":
                    // we handle links just find, don't need a special embeded object for it. DERP!
                    return null; 
                default:
                    var endpoint = _embedSettings.Settings.EmbedlurEndpoint;
                    if (!endpoint.EndsWith("/"))
                        endpoint += "/";
                    endpoint += "?url=" + WebUtility.UrlEncode(url);
                    // all other types will be shown in an <iframe /> object and served from embedlur.
                    return new EmbeddedResult(
                        EmbeddedResultType.IFrame, 
                        result.ProviderName, 
                        url: endpoint,
                        width: width, 
                        height: height);
            }
        }

        private string GetRequest(string url)
        {
            var endpoint = _embedSettings.Settings.EmbedlurEndpoint;
            if (!endpoint.EndsWith("/"))
                endpoint += "/";
            endpoint += "/oembed";

            var request = (HttpWebRequest)WebRequest.Create(endpoint + "?url=" + WebUtility.UrlEncode(url));
            request.Accept = "application/json";
            request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
            using (var response = (HttpWebResponse)request.GetResponse())
            {
                switch (response.StatusCode)
                {
                    case HttpStatusCode.OK:
                        // all fine
                        break;
                    default:
                        throw new HttpException((int)response.StatusCode, response.StatusDescription);
                }

                using (var responseStream = response.GetResponseStream())
                {
                    if (responseStream == null) throw new Exception("Couldn't get the response stream");
                    using (var streamReader = new StreamReader(responseStream))
                        return streamReader.ReadToEnd();
                }
            }
        }
        
        public class OEmbedJsonResult
        {
            [JsonProperty(PropertyName = "type")]
            public string Type { get; set; }

            [JsonProperty(PropertyName = "version")]
            public string Version { get; set; }

            [JsonProperty(PropertyName = "title")]
            public string Title { get; set; }

            [JsonProperty(PropertyName = "author_name")]
            public string AuthorName { get; set; }

            [JsonProperty(PropertyName = "author_url")]
            public string AuthorUrl { get; set; }

            [JsonProperty(PropertyName = "provider_name")]
            public string ProviderName { get; set; }

            [JsonProperty(PropertyName = "provider_url")]
            public string ProviderUrl { get; set; }

            [JsonProperty(PropertyName = "cache_age")]
            public string CacheAge { get; set; }

            [JsonProperty(PropertyName = "thumbnail_url")]
            public string ThumbnailUrl { get; set; }

            [JsonProperty(PropertyName = "thumbnail_width")]
            public string ThumbnailWidth { get; set; }

            [JsonProperty(PropertyName = "thumbnail_height")]
            public string ThumbnailHeight { get; set; }

            [JsonProperty(PropertyName = "url")]
            public string Url { get; set; }

            [JsonProperty(PropertyName = "html")]
            public string Html { get; set; }

            [JsonProperty(PropertyName = "width")]
            public string Width { get; set; }

            [JsonProperty(PropertyName = "height")]
            public string Height { get; set; }
        }

        internal class EmbeddedResult : IEmbeddedResult
        {
            internal EmbeddedResult(EmbeddedResultType type, string providerName, string url = null, string html = null, int ? width = null, int? height = null)
            {
                Type = type;
                ProviderName = providerName;
                Url = url;
                Html = html;
                Width = width;
                Height = height;
            }

            public EmbeddedResultType Type { get; }

            public string ProviderName { get; }

            public string Url { get; }

            public string Html { get; }

            public int? Width { get; }

            public int? Height { get; }
           
        }
    }
}
