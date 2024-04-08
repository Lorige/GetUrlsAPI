using Microsoft.AspNetCore.Http;
using GetUrlsAPI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using System.Net;
using System.Text.RegularExpressions;

namespace GetUrlsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class URLS : ControllerBase, IUrlGet
    {
        List<string> ListUrls { get; set; }
        public string? GetUrls(string url)
        {
            ListUrls = new List<string>() { url };
            try
            {
                Task.Run(() => AllUrlsGet(url)).Wait();
                return WriteUrls();
            } catch (Exception ex)
            {
                Console.Write(ex.ToString());
                return ex.Message;
            }
        }
        async Task AllUrlsGet(string url)
        {
            if (!ValidateUrl(url))
                throw new Exception("Invalid URL");
            try
            {
                var client = new HttpClient();
                using HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url);
                using HttpResponseMessage response = await client.SendAsync(request);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    using StreamReader streamHtml = new StreamReader(response.Content.ReadAsStream());
                    var html = streamHtml.ReadToEnd();
                    MatchCollection matches = Regex.Matches(html, @"(?<=<a\s+(?:[^>]*?\s+)?href=""([^"">]*)"")[^>]*>([^<]*)<>", RegexOptions.IgnoreCase);

                    foreach (Match match in matches)
                    {
                        string href = match.Groups[1].Value;
                        string text = match.Groups[2].Value;
                        if (href.StartsWith("/") || href.StartsWith("http://" + url.Split('/')[1]) || href.StartsWith("https://" + url.Split('/')[1]))
                        {
                            ListUrls.Add(href);
                        }
                    }
                }
            } catch (HttpRequestException ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public bool ValidateUrl(string url)
        {
            Uri? uri;
            return Uri.TryCreate(url, UriKind.Absolute, out uri) &&
                (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);
        }
        
        private string WriteUrls()
        {
            string allstrings = "";
            foreach (var readLine in ListUrls)
            {
                if (readLine == "")
                    continue;
                else
                    allstrings += $"\n{readLine}";
            }
            return allstrings;
        }
    }
}
