using Microsoft.AspNet.FileProviders;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Http.Features;
using Microsoft.AspNet.Http.Headers;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.StaticFiles;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNet.Hosting;
using Microsoft.Extensions.Primitives;

namespace Skimur.Web.Infrastructure
{
    public class StaticVirtualFileResult : ActionResult
    {
        IFileInfo _fileInfo;
        string _contentType;
        static IContentTypeProvider _contentTypeProvider = new FileExtensionContentTypeProvider();

        public StaticVirtualFileResult(IFileInfo fileInfo, string contentType = null)
        {
            _fileInfo = fileInfo;
            _contentType = contentType;
        }

        public override Task ExecuteResultAsync(ActionContext context)
        {
            var logger = context.HttpContext.RequestServices.GetRequiredService<ILoggerFactory>().CreateLogger<StaticVirtualFileResult>();
  
            var fileContext = new StaticFileContext(context.HttpContext, _contentTypeProvider, logger, _fileInfo, _contentType);

            if (!fileContext.ValidateMethod())
            {
                LoggerExtensions.LogRequestMethodNotSupported(logger, context.HttpContext.Request.Method);
            }
            else if (!fileContext.LookupContentType())
            {
                LoggerExtensions.LogFileTypeNotSupported(logger, fileContext.SubPath);
            }
            else if (!fileContext.LookupFileInfo())
            {
                LoggerExtensions.LogFileNotFound(logger, fileContext.SubPath);
            }
            else
            {
                // If we get here, we can try to serve the file
                fileContext.ComprehendRequestHeaders();

                switch (fileContext.GetPreconditionState())
                {
                    case StaticFileContext.PreconditionState.Unspecified:
                    case StaticFileContext.PreconditionState.ShouldProcess:
                        if (fileContext.IsHeadMethod)
                        {
                            return fileContext.SendStatusAsync(Constants.Status200Ok);
                        }
                        if (fileContext.IsRangeRequest)
                        {
                            return fileContext.SendRangeAsync();
                        }

                        LoggerExtensions.LogFileServed(logger, fileContext.SubPath, fileContext.PhysicalPath);
                        return fileContext.SendAsync();

                    case StaticFileContext.PreconditionState.NotModified:
                        LoggerExtensions.LogPathNotModified(logger, fileContext.SubPath);
                        return fileContext.SendStatusAsync(Constants.Status304NotModified);

                    case StaticFileContext.PreconditionState.PreconditionFailed:
                        LoggerExtensions.LogPreconditionFailed(logger, fileContext.SubPath);
                        return fileContext.SendStatusAsync(Constants.Status412PreconditionFailed);

                    default:
                        var exception = new NotImplementedException(fileContext.GetPreconditionState().ToString());
                        Debug.Fail(exception.ToString());
                        throw exception;
                }
            }

            return Task.FromResult(0);
        }

        private static class Constants
        {
            internal const string ServerCapabilitiesKey = "server.Capabilities";
            internal const string SendFileVersionKey = "sendfile.Version";
            internal const string SendFileVersion = "1.0";

            internal const int Status200Ok = 200;
            internal const int Status206PartialContent = 206;
            internal const int Status304NotModified = 304;
            internal const int Status412PreconditionFailed = 412;
            internal const int Status416RangeNotSatisfiable = 416;

            internal static readonly Task CompletedTask = CreateCompletedTask();

            private static Task CreateCompletedTask()
            {
                var tcs = new TaskCompletionSource<object>();
                tcs.SetResult(null);
                return tcs.Task;
            }
        }

        internal struct StaticFileContext
        {
            private readonly HttpContext _context;
            private readonly IContentTypeProvider _contentTypeProvider;
            private readonly HttpRequest _request;
            private readonly HttpResponse _response;
            private readonly ILogger _logger;
            private readonly IFileInfo _fileInfo;
            private string _method;
            private bool _isGet;
            private bool _isHead;
            private PathString _subPath;
            private string _contentType;
            private long _length;
            private DateTimeOffset _lastModified;
            private EntityTagHeaderValue _etag;

            private RequestHeaders _requestHeaders;
            private ResponseHeaders _responseHeaders;

            private PreconditionState _ifMatchState;
            private PreconditionState _ifNoneMatchState;
            private PreconditionState _ifModifiedSinceState;
            private PreconditionState _ifUnmodifiedSinceState;

            private IList<RangeItemHeaderValue> _ranges;

            public StaticFileContext(HttpContext context, 
                IContentTypeProvider contentTypeProvider,
                ILogger logger,
                IFileInfo fileInfo,
                string contentType)
            {
                _context = context;
                _contentTypeProvider = contentTypeProvider;
                _request = context.Request;
                _response = context.Response;
                _logger = logger;
                _fileInfo = fileInfo;
                _contentType = contentType;
                _requestHeaders = _request.GetTypedHeaders();
                _responseHeaders = _response.GetTypedHeaders();

                _method = null;
                _isGet = false;
                _isHead = false;
                _subPath = PathString.Empty;
                _length = 0;
                _lastModified = new DateTimeOffset();
                _etag = null;
                _ifMatchState = PreconditionState.Unspecified;
                _ifNoneMatchState = PreconditionState.Unspecified;
                _ifModifiedSinceState = PreconditionState.Unspecified;
                _ifUnmodifiedSinceState = PreconditionState.Unspecified;
                _ranges = null;
            }

            internal enum PreconditionState
            {
                Unspecified,
                NotModified,
                ShouldProcess,
                PreconditionFailed,
            }

            public bool IsHeadMethod
            {
                get { return _isHead; }
            }

            public bool IsRangeRequest
            {
                get { return _ranges != null; }
            }

            public string SubPath
            {
                get { return _subPath.Value; }
            }

            public string PhysicalPath
            {
                get { return _fileInfo?.PhysicalPath; }
            }

            public bool ValidateMethod()
            {
                _method = _request.Method;
                _isGet = Helpers.IsGetMethod(_method);
                _isHead = Helpers.IsHeadMethod(_method);
                return _isGet || _isHead;
            }
            
            public bool LookupContentType()
            {
                if(string.IsNullOrEmpty(_contentType))
                {
                    return _contentTypeProvider.TryGetContentType(_fileInfo.Name, out _contentType);
                }
                return true;

                //if (_options.ContentTypeProvider.TryGetContentType(_subPath.Value, out _contentType))
                //{
                //    return true;
                //}

                //if (_options.ServeUnknownFileTypes)
                //{
                //    _contentType = _options.DefaultContentType;
                //    return true;
                //}

                //return false;
            }

            public bool LookupFileInfo()
            {
                if (_fileInfo.Exists)
                {
                    _length = _fileInfo.Length;

                    DateTimeOffset last = _fileInfo.LastModified;
                    // Truncate to the second.
                    _lastModified = new DateTimeOffset(last.Year, last.Month, last.Day, last.Hour, last.Minute, last.Second, last.Offset);

                    long etagHash = _lastModified.ToFileTime() ^ _length;
                    _etag = new EntityTagHeaderValue('\"' + Convert.ToString(etagHash, 16) + '\"');
                }
                return _fileInfo.Exists;
            }

            public void ComprehendRequestHeaders()
            {
                ComputeIfMatch();

                ComputeIfModifiedSince();

                ComputeRange();
            }

            private void ComputeIfMatch()
            {
                // 14.24 If-Match
                var ifMatch = _requestHeaders.IfMatch;
                if (ifMatch != null && ifMatch.Any())
                {
                    _ifMatchState = PreconditionState.PreconditionFailed;
                    foreach (var etag in ifMatch)
                    {
                        if (etag.Equals(EntityTagHeaderValue.Any) || etag.Equals(_etag))
                        {
                            _ifMatchState = PreconditionState.ShouldProcess;
                            break;
                        }
                    }
                }

                // 14.26 If-None-Match
                var ifNoneMatch = _requestHeaders.IfNoneMatch;
                if (ifNoneMatch != null && ifNoneMatch.Any())
                {
                    _ifNoneMatchState = PreconditionState.ShouldProcess;
                    foreach (var etag in ifNoneMatch)
                    {
                        if (etag.Equals(EntityTagHeaderValue.Any) || etag.Equals(_etag))
                        {
                            _ifNoneMatchState = PreconditionState.NotModified;
                            break;
                        }
                    }
                }
            }

            private void ComputeIfModifiedSince()
            {
                // 14.25 If-Modified-Since
                var ifModifiedSince = _requestHeaders.IfModifiedSince;
                if (ifModifiedSince.HasValue)
                {
                    bool modified = ifModifiedSince < _lastModified;
                    _ifModifiedSinceState = modified ? PreconditionState.ShouldProcess : PreconditionState.NotModified;
                }

                // 14.28 If-Unmodified-Since
                var ifUnmodifiedSince = _requestHeaders.IfUnmodifiedSince;
                if (ifUnmodifiedSince.HasValue)
                {
                    bool unmodified = ifUnmodifiedSince >= _lastModified;
                    _ifUnmodifiedSinceState = unmodified ? PreconditionState.ShouldProcess : PreconditionState.PreconditionFailed;
                }
            }

            private void ComputeRange()
            {
                // 14.35 Range
                // http://tools.ietf.org/html/draft-ietf-httpbis-p5-range-24

                // A server MUST ignore a Range header field received with a request method other
                // than GET.
                if (!_isGet)
                {
                    return;
                }

                var rangeHeader = _requestHeaders.Range;
                if (rangeHeader == null)
                {
                    return;
                }

                if (rangeHeader.Ranges.Count > 1)
                {
                    // The spec allows for multiple ranges but we choose not to support them because the client may request
                    // very strange ranges (e.g. each byte separately, overlapping ranges, etc.) that could negatively
                    // impact the server. Ignore the header and serve the response normally.
                    LoggerExtensions.LogMultipleFileRanges(_logger, rangeHeader.ToString());
                    return;
                }

                // 14.27 If-Range
                var ifRangeHeader = _requestHeaders.IfRange;
                if (ifRangeHeader != null)
                {
                    // If the validator given in the If-Range header field matches the
                    // current validator for the selected representation of the target
                    // resource, then the server SHOULD process the Range header field as
                    // requested.  If the validator does not match, the server MUST ignore
                    // the Range header field.
                    bool ignoreRangeHeader = false;
                    if (ifRangeHeader.LastModified.HasValue)
                    {
                        if (_lastModified > ifRangeHeader.LastModified)
                        {
                            ignoreRangeHeader = true;
                        }
                    }
                    else if (ifRangeHeader.EntityTag != null && !_etag.Equals(ifRangeHeader.EntityTag))
                    {
                        ignoreRangeHeader = true;
                    }
                    if (ignoreRangeHeader)
                    {
                        return;
                    }
                }

                _ranges = RangeHelpers.NormalizeRanges(rangeHeader.Ranges, _length);
            }

            public void ApplyResponseHeaders(int statusCode)
            {
                _response.StatusCode = statusCode;
                if (statusCode < 400)
                {
                    // these headers are returned for 200, 206, and 304
                    // they are not returned for 412 and 416
                    if (!string.IsNullOrEmpty(_contentType))
                    {
                        _response.ContentType = _contentType;
                    }
                    _responseHeaders.LastModified = _lastModified;
                    _responseHeaders.ETag = _etag;
                    _responseHeaders.Headers[HeaderNames.AcceptRanges] = "bytes";
                }
                if (statusCode == Constants.Status200Ok)
                {
                    // this header is only returned here for 200
                    // it already set to the returned range for 206
                    // it is not returned for 304, 412, and 416
                    _response.ContentLength = _length;
                }
                //_options.OnPrepareResponse(new StaticFileResponseContext()
                //{
                //    Context = _context,
                //    File = _fileInfo,
                //});
            }

            public PreconditionState GetPreconditionState()
            {
                return GetMaxPreconditionState(_ifMatchState, _ifNoneMatchState,
                    _ifModifiedSinceState, _ifUnmodifiedSinceState);
            }

            private static PreconditionState GetMaxPreconditionState(params PreconditionState[] states)
            {
                PreconditionState max = PreconditionState.Unspecified;
                for (int i = 0; i < states.Length; i++)
                {
                    if (states[i] > max)
                    {
                        max = states[i];
                    }
                }
                return max;
            }

            public Task SendStatusAsync(int statusCode)
            {
                ApplyResponseHeaders(statusCode);

                LoggerExtensions.LogHandled(_logger, statusCode, SubPath);
                return Constants.CompletedTask;
            }

            public async Task SendAsync()
            {
                ApplyResponseHeaders(Constants.Status200Ok);

                string physicalPath = _fileInfo.PhysicalPath;
                var sendFile = _context.Features.Get<IHttpSendFileFeature>();
                if (sendFile != null && !string.IsNullOrEmpty(physicalPath))
                {
                    await sendFile.SendFileAsync(physicalPath, 0, _length, _context.RequestAborted);
                    return;
                }

                Stream readStream = _fileInfo.CreateReadStream();
                try
                {
                    await StreamCopyOperation.CopyToAsync(readStream, _response.Body, _length, _context.RequestAborted);
                }
                finally
                {
                    readStream.Dispose();
                }
            }

            // When there is only a single range the bytes are sent directly in the body.
            internal async Task SendRangeAsync()
            {
                bool rangeNotSatisfiable = false;
                if (_ranges.Count == 0)
                {
                    rangeNotSatisfiable = true;
                }

                if (rangeNotSatisfiable)
                {
                    // 14.16 Content-Range - A server sending a response with status code 416 (Requested range not satisfiable)
                    // SHOULD include a Content-Range field with a byte-range-resp-spec of "*". The instance-length specifies
                    // the current length of the selected resource.  e.g. */length
                    _responseHeaders.ContentRange = new ContentRangeHeaderValue(_length);
                    ApplyResponseHeaders(Constants.Status416RangeNotSatisfiable);

                    LoggerExtensions.LogRangeNotSatisfiable(_logger, SubPath);
                    return;
                }

                // Multi-range is not supported.
                Debug.Assert(_ranges.Count == 1);

                long start, length;
                _responseHeaders.ContentRange = ComputeContentRange(_ranges[0], out start, out length);
                _response.ContentLength = length;
                ApplyResponseHeaders(Constants.Status206PartialContent);

                string physicalPath = _fileInfo.PhysicalPath;
                var sendFile = _context.Features.Get<IHttpSendFileFeature>();
                if (sendFile != null && !string.IsNullOrEmpty(physicalPath))
                {
                    LoggerExtensions.LogSendingFileRange(_logger, _response.Headers[HeaderNames.ContentRange], physicalPath);
                    await sendFile.SendFileAsync(physicalPath, start, length, _context.RequestAborted);
                    return;
                }

                Stream readStream = _fileInfo.CreateReadStream();
                try
                {
                    readStream.Seek(start, SeekOrigin.Begin); // TODO: What if !CanSeek?
                    LoggerExtensions.LogCopyingFileRange(_logger, _response.Headers[HeaderNames.ContentRange], SubPath);
                    await StreamCopyOperation.CopyToAsync(readStream, _response.Body, length, _context.RequestAborted);
                }
                finally
                {
                    readStream.Dispose();
                }
            }

            // Note: This assumes ranges have been normalized to absolute byte offsets.
            private ContentRangeHeaderValue ComputeContentRange(RangeItemHeaderValue range, out long start, out long length)
            {
                start = range.From.Value;
                long end = range.To.Value;
                length = end - start + 1;
                return new ContentRangeHeaderValue(start, end, _length);
            }
        }

        private static class RangeHelpers
        {
            // 14.35.1 Byte Ranges - If a syntactically valid byte-range-set includes at least one byte-range-spec whose
            // first-byte-pos is less than the current length of the entity-body, or at least one suffix-byte-range-spec
            // with a non-zero suffix-length, then the byte-range-set is satisfiable.
            // Adjusts ranges to be absolute and within bounds.
            internal static IList<RangeItemHeaderValue> NormalizeRanges(ICollection<RangeItemHeaderValue> ranges, long length)
            {
                IList<RangeItemHeaderValue> normalizedRanges = new List<RangeItemHeaderValue>(ranges.Count);
                foreach (var range in ranges)
                {
                    long? start = range.From;
                    long? end = range.To;

                    // X-[Y]
                    if (start.HasValue)
                    {
                        if (start.Value >= length)
                        {
                            // Not satisfiable, skip/discard.
                            continue;
                        }
                        if (!end.HasValue || end.Value >= length)
                        {
                            end = length - 1;
                        }
                    }
                    else
                    {
                        // suffix range "-X" e.g. the last X bytes, resolve
                        if (end.Value == 0)
                        {
                            // Not satisfiable, skip/discard.
                            continue;
                        }

                        long bytes = Math.Min(end.Value, length);
                        start = length - bytes;
                        end = start + bytes - 1;
                    }
                    normalizedRanges.Add(new RangeItemHeaderValue(start.Value, end.Value));
                }
                return normalizedRanges;
            }
        }

        private static class Helpers
        {
            internal static bool IsGetOrHeadMethod(string method)
            {
                return IsGetMethod(method) || IsHeadMethod(method);
            }

            internal static bool IsGetMethod(string method)
            {
                return string.Equals("GET", method, StringComparison.OrdinalIgnoreCase);
            }

            internal static bool IsHeadMethod(string method)
            {
                return string.Equals("HEAD", method, StringComparison.OrdinalIgnoreCase);
            }

            internal static bool PathEndsInSlash(PathString path)
            {
                return path.Value.EndsWith("/", StringComparison.Ordinal);
            }

            internal static bool TryMatchPath(HttpContext context, PathString matchUrl, bool forDirectory, out PathString subpath)
            {
                var path = context.Request.Path;

                if (forDirectory && !PathEndsInSlash(path))
                {
                    path += new PathString("/");
                }

                if (path.StartsWithSegments(matchUrl, out subpath))
                {
                    return true;
                }
                return false;
            }
        }

        // FYI: In most cases the source will be a FileStream and the destination will be to the network.
        private static class StreamCopyOperation
        {
            private const int DefaultBufferSize = 1024 * 16;

            internal static async Task CopyToAsync(Stream source, Stream destination, long? length, CancellationToken cancel)
            {
                long? bytesRemaining = length;
                byte[] buffer = new byte[DefaultBufferSize];

                Debug.Assert(source != null);
                Debug.Assert(destination != null);
                Debug.Assert(!bytesRemaining.HasValue || bytesRemaining.Value >= 0);
                Debug.Assert(buffer != null);

                while (true)
                {
                    // The natural end of the range.
                    if (bytesRemaining.HasValue && bytesRemaining.Value <= 0)
                    {
                        return;
                    }

                    cancel.ThrowIfCancellationRequested();

                    int readLength = buffer.Length;
                    if (bytesRemaining.HasValue)
                    {
                        readLength = (int)Math.Min(bytesRemaining.Value, (long)readLength);
                    }
                    int count = await source.ReadAsync(buffer, 0, readLength, cancel);

                    if (bytesRemaining.HasValue)
                    {
                        bytesRemaining -= count;
                    }

                    // End of the source stream.
                    if (count == 0)
                    {
                        return;
                    }

                    cancel.ThrowIfCancellationRequested();

                    await destination.WriteAsync(buffer, 0, count, cancel);
                }
            }
        }

        /// <summary>
        /// Defines *all* the logger messages produced by static files
        /// </summary>
        private static class LoggerExtensions
        {
            private static Action<ILogger, string, Exception> _logMethodNotSupported;
            private static Action<ILogger, string, string, Exception> _logFileServed;
            private static Action<ILogger, string, Exception> _logPathMismatch;
            private static Action<ILogger, string, Exception> _logFileTypeNotSupported;
            private static Action<ILogger, string, Exception> _logFileNotFound;
            private static Action<ILogger, string, Exception> _logPathNotModified;
            private static Action<ILogger, string, Exception> _logPreconditionFailed;
            private static Action<ILogger, int, string, Exception> _logHandled;
            private static Action<ILogger, string, Exception> _logRangeNotSatisfiable;
            private static Action<ILogger, StringValues, string, Exception> _logSendingFileRange;
            private static Action<ILogger, StringValues, string, Exception> _logCopyingFileRange;
            private static Action<ILogger, long, string, string, Exception> _logCopyingBytesToResponse;
            private static Action<ILogger, string, Exception> _logMultipleFileRanges;

            static LoggerExtensions()
            {
                _logMethodNotSupported = LoggerMessage.Define<string>(
                    logLevel: LogLevel.Debug,
                    eventId: 1,
                    formatString: "{Method} requests are not supported");
                _logFileServed = LoggerMessage.Define<string, string>(
                   logLevel: LogLevel.Information,
                   eventId: 2,
                   formatString: "Sending file. Request path: '{VirtualPath}'. Physical path: '{PhysicalPath}'");
                _logPathMismatch = LoggerMessage.Define<string>(
                    logLevel: LogLevel.Debug,
                    eventId: 3,
                    formatString: "The request path {Path} does not match the path filter");
                _logFileTypeNotSupported = LoggerMessage.Define<string>(
                    logLevel: LogLevel.Debug,
                    eventId: 4,
                    formatString: "The request path {Path} does not match a supported file type");
                _logFileNotFound = LoggerMessage.Define<string>(
                    logLevel: LogLevel.Debug,
                    eventId: 5,
                    formatString: "The request path {Path} does not match an existing file");
                _logPathNotModified = LoggerMessage.Define<string>(
                    logLevel: LogLevel.Information,
                    eventId: 6,
                    formatString: "The file {Path} was not modified");
                _logPreconditionFailed = LoggerMessage.Define<string>(
                    logLevel: LogLevel.Information,
                    eventId: 7,
                    formatString: "Precondition for {Path} failed");
                _logHandled = LoggerMessage.Define<int, string>(
                    logLevel: LogLevel.Debug,
                    eventId: 8,
                    formatString: "Handled. Status code: {StatusCode} File: {Path}");
                _logRangeNotSatisfiable = LoggerMessage.Define<string>(
                    logLevel: LogLevel.Warning,
                    eventId: 9,
                    formatString: "Range not satisfiable for {Path}");
                _logSendingFileRange = LoggerMessage.Define<StringValues, string>(
                    logLevel: LogLevel.Information,
                    eventId: 10,
                    formatString: "Sending {Range} of file {Path}");
                _logCopyingFileRange = LoggerMessage.Define<StringValues, string>(
                    logLevel: LogLevel.Debug,
                    eventId: 11,
                    formatString: "Copying {Range} of file {Path} to the response body");
                _logCopyingBytesToResponse = LoggerMessage.Define<long, string, string>(
                    logLevel: LogLevel.Debug,
                    eventId: 12,
                    formatString: "Copying bytes {Start}-{End} of file {Path} to response body");
                _logMultipleFileRanges = LoggerMessage.Define<string>(
                    logLevel: LogLevel.Warning,
                    eventId: 13,
                    formatString: "Multiple ranges are not allowed: '{Ranges}'");
            }

            public static void LogRequestMethodNotSupported(ILogger logger, string method)
            {
                _logMethodNotSupported(logger, method, null);
            }

            public static void LogFileServed(ILogger logger, string virtualPath, string physicalPath)
            {
                if (string.IsNullOrEmpty(physicalPath))
                {
                    physicalPath = "N/A";
                }
                _logFileServed(logger, virtualPath, physicalPath, null);
            }

            public static void LogPathMismatch(ILogger logger, string path)
            {
                _logPathMismatch(logger, path, null);
            }

            public static void LogFileTypeNotSupported(ILogger logger, string path)
            {
                _logFileTypeNotSupported(logger, path, null);
            }

            public static void LogFileNotFound(ILogger logger, string path)
            {
                _logFileNotFound(logger, path, null);
            }

            public static void LogPathNotModified(ILogger logger, string path)
            {
                _logPathNotModified(logger, path, null);
            }

            public static void LogPreconditionFailed(ILogger logger, string path)
            {
                _logPreconditionFailed(logger, path, null);
            }

            public static void LogHandled(ILogger logger, int statusCode, string path)
            {
                _logHandled(logger, statusCode, path, null);
            }

            public static void LogRangeNotSatisfiable(ILogger logger, string path)
            {
                _logRangeNotSatisfiable(logger, path, null);
            }

            public static void LogSendingFileRange(ILogger logger, StringValues range, string path)
            {
                _logSendingFileRange(logger, range, path, null);
            }

            public static void LogCopyingFileRange(ILogger logger, StringValues range, string path)
            {
                _logCopyingFileRange(logger, range, path, null);
            }

            public static void LogCopyingBytesToResponse(ILogger logger, long start, long? end, string path)
            {
                _logCopyingBytesToResponse(
                    logger,
                    start,
                    end != null ? end.ToString() : "*",
                    path,
                    null);
            }

            public static void LogMultipleFileRanges(ILogger logger, string range)
            {
                _logMultipleFileRanges(logger, range, null);
            }
        }
    }
}
