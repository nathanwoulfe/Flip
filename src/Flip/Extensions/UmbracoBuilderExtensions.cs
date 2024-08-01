using System.Text.Json;
using System.Text.Json.Serialization;
using Flip.Api.Configuration;
using Flip.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using Umbraco.Cms.Api.Common.DependencyInjection;
using Umbraco.Cms.Api.Common.OpenApi;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Workflow.Web.Api.Configuration;

namespace Flip.Extensions;

public static class UmbracoBuilderExtensions
{
    public static IUmbracoBuilder AddFlip(this IUmbracoBuilder builder)
    {
        _ = builder.Services
            .AddSingleton<IFlipService, FlipService>();

        //.AddSingleton<IContentTreeMenuRenderingExecutor, ContentTreeMenuRenderingExecutor>()
        //.AddSingleton<IServerVariablesParsingExecutor, ServerVariablesParsingExecutor>();

        //_ = builder.AddNotificationHandler<MenuRenderingNotification, ContentTreeMenuRenderingHandler>()
        //    .AddNotificationHandler<ServerVariablesParsingNotification, ServerVariablesParsingHandler>();

        _ = builder
            .AddServices()
            .AddSwagger()
            .AddJsonOptions();

        return builder;
    }

    private static IUmbracoBuilder AddJsonOptions(this IUmbracoBuilder builder)
    {
        _ = builder.Services.AddControllers().AddJsonOptions(Constants.Alias, ConfigureJsonOptions);
        return builder;
    }

    private static IUmbracoBuilder AddSwagger(this IUmbracoBuilder builder)
    {
        // Generate Swagger documentation for the management API
        _ = builder.Services
            .Configure<SwaggerGenOptions>(options =>
            {
                options.SwaggerDoc(
                    ApiConstants.ApiName,
                    new OpenApiInfo
                    {
                        Title = ApiConstants.ApiTitle,
                        Version = "Latest",
                        Description = $"Describes the {ApiConstants.ApiTitle}.",
                    });
                options.DocumentFilter<MimeTypeDocumentFilter>(ApiConstants.ApiName);
                options.OperationFilter<BackOfficeSecurityRequirementsOperationFilter>();
            })
            .AddSingleton<ISchemaIdHandler, FlipSchemaIdHandler>()
            .AddSingleton<IOperationIdHandler, FlipOperationIdHandler>();

        return builder;
    }

    private static IUmbracoBuilder AddServices(this IUmbracoBuilder builder)
    {
        _ = builder.Services.AddSingleton<IFlipService, FlipService>();
        return builder;
    }

    private static void ConfigureJsonOptions(JsonOptions options)
    {
        // Reset web specific settings
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
        options.JsonSerializerOptions.NumberHandling = JsonNumberHandling.Strict;
    }
}
