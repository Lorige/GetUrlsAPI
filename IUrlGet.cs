
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

namespace GetUrlsAPI
{
    public interface IUrlGet
    {
        Task<string> GetUrlsFromHtml(string url);
    }

    public class GetAllUrlsMiddleware
    {
        private readonly RequestDelegate next;
        int counter = 0;
        public GetAllUrlsMiddleware(RequestDelegate next) => this.next = next;
        public async Task InvokeAsync(HttpContext httpContext, IUrlGet urlGet)
        {
            string patternUrl = @"^/api/URLS&(https?://[^www][\w-]+(\.[a-z]+)+(/[\w-/]+(\.[a-z]+))?)$";
            if (Regex.IsMatch(httpContext.Request.Path, patternUrl))
            {
                MatchCollection matches = Regex.Matches(httpContext.Request.Path, patternUrl);
                var path = matches[0].Groups[1].Value;
                counter++;
                httpContext.Response.ContentType = "text/html;charset=utf-8";
                await httpContext.Response.WriteAsync($"Запрос {counter}; " +
                    $"Counter: {await urlGet.GetUrlsFromHtml(path)}");
            }
            else
                await next.Invoke(httpContext);
        }
    }

}
