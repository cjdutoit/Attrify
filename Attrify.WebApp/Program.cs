// -----------------------------------------------------
// Copyright (c)  Christo du Toit - All rights reserved.
// -----------------------------------------------------

using System.IO;
using System.Text.Json;
using Attrify.Extensions;
using Attrify.InvisibleApi.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Attrify.WebApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = Microsoft.AspNetCore.Builder.WebApplication.CreateBuilder(args);
            var invisibleApiKey = new InvisibleApiKey();
            ConfigureServices(builder, builder.Configuration, invisibleApiKey);
            var app = builder.Build();
            ConfigurePipeline(app, invisibleApiKey);
            app.Run();
        }

        public static void ConfigureServices(
            WebApplicationBuilder builder,
            IConfiguration configuration,
            InvisibleApiKey invisibleApiKey)
        {
            var projectDir = Directory.GetCurrentDirectory();
            var launchSettingsPath = Path.Combine(projectDir, "Properties", "launchSettings.json");

            if (File.Exists(launchSettingsPath))
            {
                builder.Configuration.AddJsonFile(launchSettingsPath);
            }

            builder.Services.AddSingleton(invisibleApiKey);
            builder.Services.AddAuthorization();
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddControllers();
            builder.Services.AddDeprecatedApiSupport();
            builder.Services.AddSingleton<IConfiguration>(builder.Configuration);
            JsonNamingPolicy jsonNamingPolicy = JsonNamingPolicy.CamelCase;

            builder.Services.AddControllers()
               .AddJsonOptions(options =>
               {
                   options.JsonSerializerOptions.PropertyNamingPolicy = jsonNamingPolicy;
                   options.JsonSerializerOptions.DictionaryKeyPolicy = jsonNamingPolicy;
                   options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
                   options.JsonSerializerOptions.WriteIndented = true;
               });
        }

        public static void ConfigurePipeline(
            Microsoft.AspNetCore.Builder.WebApplication app,
            InvisibleApiKey invisibleApiKey)
        {
            app.UseDefaultFiles();
            app.UseStaticFiles();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("/swagger/v1/swagger.json", "API V1");
                    options.RoutePrefix = string.Empty; // Optional: This makes Swagger available at the root URL.
                });
            }

            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseDeprecatedApiMiddleware();
            app.UseInvisibleApiMiddleware(invisibleApiKey);
            app.MapControllers().WithOpenApi();
            app.MapFallbackToFile("/swagger/index.html");
        }
    }
}
