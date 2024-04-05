using Microsoft.AspNetCore.Http;
using GetUrlsAPI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;

namespace GetUrlsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class URLS : ControllerBase, IUrlGet
    {
        List<string?> ListUrls { get; set; }
        public URLS()
        {
            ListUrls = new List<string?>();
        }

        public string? GetUrls(string url)
        {
            ListUrls = new List<string?>();
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
            using var client = new HttpClient();
            using HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url);
            using HttpResponseMessage response = await client.SendAsync(request);
            var readStream = response.Content.ReadAsStreamAsync().Result;
            var streamReader = new StreamReader(readStream);
            while (!streamReader.EndOfStream)
            {
                var a = streamReader.ReadLine();
                Substring(a);
            }
        }
        private void Substring(string? str)
        {
            var strRemove = str;
            while (true && !string.IsNullOrEmpty(strRemove))
            {
                if (strRemove.Contains("http://"))
                {
                    strRemove = GetString(strRemove, "http://");
                }
                else if (strRemove.Contains("https://"))
                {
                    strRemove = GetString(strRemove, "https://");
                }
                else
                    break;
            }
        }

        private string GetString(string strBody, string subStringIndexOf)
        {
            var startIndexUrl = strBody.IndexOf(subStringIndexOf);
            var endIndexUrl = GetMinimumIndex(strBody, startIndexUrl + subStringIndexOf.Length);
            var subString = strBody.Substring(startIndexUrl, endIndexUrl - startIndexUrl);
            if (!ListUrls.Contains(subString) && subString.Length > 10 &&
                (('A' <= subString[subStringIndexOf.Length] & subString[subStringIndexOf.Length] <= 'Z') ||
                ('a' <= subString[subStringIndexOf.Length] & subString[subStringIndexOf.Length] <= 'z'))
                )
            {
                ListUrls.Add(subString);
            }
            var strRemove = strBody.Remove(startIndexUrl, endIndexUrl - startIndexUrl);
            return strRemove;
        }

        private int GetMinimumIndex(string str, int startIndex)
        {
            int[] index = [str.IndexOf(')', startIndex), str.IndexOf('\'', startIndex),
            str.IndexOf('"', startIndex), str.IndexOf('/', startIndex), str.IndexOf('?', startIndex)];
            var currentMinIndex = index.Max();
            foreach (var ind in index)
                if (ind > startIndex && ind < currentMinIndex && ind > 0)
                    currentMinIndex = ind;
            return currentMinIndex;
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
