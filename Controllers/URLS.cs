﻿using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.RegularExpressions;

namespace GetUrlsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class URLS(IHttpClientFactory httpClientFactory) : ControllerBase, IUrlGet
    {
        public IHttpClientFactory HttpClientFactory { get; } = httpClientFactory;
        
        [HttpGet]
        public async Task<string> GetUrlsFromHtml(string url)
        {
            try
            {
                var htmlUrls = "";
                HttpClient client = HttpClientFactory.CreateClient();
                using HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url);
                using HttpResponseMessage response = await client.SendAsync(request);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    using StreamReader streamHtml = new StreamReader(await response.Content.ReadAsStreamAsync());
                    var html = await streamHtml.ReadToEndAsync();
                    MatchCollection matches = Regex.Matches(html, @"href=""([a-z:/._]*)[""/]", RegexOptions.IgnoreCase); 
                    foreach (Match match in matches)
                    {
                        string href = match.Groups[1].Value;
                        if (href.StartsWith("http://" + url.Split('/')[1]) || href.StartsWith("https://" + url.Split('/')[1]))
                        {
                            htmlUrls += "\n" + href;
                        }
                    }
                }
                return htmlUrls;

            } catch (HttpRequestException ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return "Invalid URL";
        }
    }
}
