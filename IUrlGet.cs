
using Microsoft.AspNetCore.Mvc;

namespace GetUrlsAPI
{
    public interface IUrlGet
    {
        string? GetUrls(string url);
    }
}
