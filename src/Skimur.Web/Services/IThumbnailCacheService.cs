using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Skimur.Web.Services
{
    public interface IThumbnailCacheService
    {
        string GetThumbnail(string thumbnail, ThumbnailType type);
    }

    public enum ThumbnailType
    {
        Post,
        Full
    }
}
