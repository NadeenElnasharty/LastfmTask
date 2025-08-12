using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddCors(o => o.AddPolicy("AllowLocal", p =>
{
    p.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
}));
builder.Services.AddHttpClient("LastFm", c =>
{
    c.BaseAddress = new Uri("https://ws.audioscrobbler.com/2.0/");
    c.DefaultRequestHeaders.Add("User-Agent", "LastfmSampleApp/1.0");
});

var app = builder.Build();

app.UseCors("AllowLocal");
app.MapControllers();

app.Run();
