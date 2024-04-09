using GetUrlsAPI.Controllers;
using GetUrlsAPI;
using System;
using Microsoft.AspNetCore.Builder;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder();

        builder.Services.AddScoped<IUrlGet, URLS>();
        var app = builder.Build();
        app.Map("/", () => "Index Page");

        app.UseMiddleware<GetAllUrlsMiddleware>();
        app.Run();
    }
}