internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder();
        builder.Services.AddTransient<IUrlGet, URLS>();

        var app = builder.Build();

        app.Run(async(context) => 
        { 
            var IUrlGet = app.Services.GetService<IUrlGet>();
            await context.Response.WriteAsync($"this: {IUrlGet?.UrlStatus().Result}");
            await context.Response.WriteAsync($"\ndate: {IUrlGet?.DateTime()}");
        });
        app.Run();
    }
}

interface IUrlGet
{
    Task<string?> UrlStatus();
    string DateTime();
}
public class URLS : IUrlGet
{
    async Task<string?> IUrlGet.UrlStatus()
    {
        using (var client = new HttpClient())
        {
            using var result = await client.GetAsync("https://metanit.com");
            return result.Headers.ToString();
        }
    }
    string IUrlGet.DateTime() { return DateTime.Now.ToString(); }
}