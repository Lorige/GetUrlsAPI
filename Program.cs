using GetUrlsAPI.Controllers;
using GetUrlsAPI;
using System;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder();
        builder.Services.AddMvc();
        builder.Services.AddSingleton<IUrlGet, URLS>();

        var app = builder.Build();
        app.Run(async(context) => 
        { 
            var IUrlGet = app.Services.GetService<IUrlGet>();
            await context.Response.WriteAsync($"Urls: {IUrlGet?.GetUrls("https://ya.ru")}");
        });
        app.Run();
    }
}