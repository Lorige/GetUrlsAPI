using GetUrlsAPI.Controllers;
using GetUrlsAPI;
using System;
using Microsoft.AspNetCore.Builder;
using System.Text.Json;
using Microsoft.Graph;
using Microsoft.Graph.Models;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = Microsoft.AspNetCore.Builder.WebApplication.CreateBuilder();
        builder.Services.AddScoped<IUrlGet, URLS>();
        builder.Services.AddHttpClient();
        var app = builder.Build();

        app.Map("/", () => "Index Page");

        app.UseMiddleware<GetAllUrlsMiddleware>();
        app.Run();

    }
}