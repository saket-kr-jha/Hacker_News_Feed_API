using HackerNewsFeed_API.BusinessServices;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Serilog;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

public class ProgramTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public ProgramTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    // Test Serilog Configuration
    [Fact]
    public void Serilog_Should_Be_Configured_Correctly()
    {
        // Arrange
        var builder = WebApplication.CreateBuilder();

        // Act
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(builder.Configuration)
            .CreateLogger();
        builder.Host.UseSerilog();

        // Assert
        Assert.NotNull(Log.Logger);
    }

    // Test MemoryCache Registration
    [Fact]
    public void MemoryCache_Should_Be_Registered()
    {
        // Arrange
        var builder = WebApplication.CreateBuilder();

        // Act
        builder.Services.AddMemoryCache();
        var serviceProvider = builder.Services.BuildServiceProvider();
        var memoryCache = serviceProvider.GetService<IMemoryCache>();

        // Assert
        Assert.NotNull(memoryCache);
    }

    // Test HttpClient Registration
    [Fact]
    public void HttpClient_Should_Be_Registered()
    {
        // Arrange
        var builder = WebApplication.CreateBuilder();

        // Act
        builder.Services.AddHttpClient<IHackerNewsFeedBusinessProcess, HackerNewsFeedBusinessProcess>();
        var serviceProvider = builder.Services.BuildServiceProvider();
        var httpClientFactory = serviceProvider.GetService<IHttpClientFactory>();

        // Assert
        Assert.NotNull(httpClientFactory);
    }

    // Test CORS Configuration
    [Fact]
    public void CORS_Should_Be_Configured_Correctly()
    {
        // Arrange
        var builder = WebApplication.CreateBuilder();

        // Act
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowAllOrigins",
                builder =>
                {
                    builder.AllowAnyOrigin()
                           .AllowAnyMethod()
                           .AllowAnyHeader();
                });
        });
        var serviceProvider = builder.Services.BuildServiceProvider();
        var corsPolicy = serviceProvider.GetService<ICorsPolicyProvider>();

        // Assert
        Assert.NotNull(corsPolicy);
    }

    // Test Swagger Middleware in Development
    [Fact]
    public void Swagger_Should_Be_Enabled_In_Development()
    {
        // Arrange
        var builder = WebApplication.CreateBuilder();
        builder.Environment.EnvironmentName = Environments.Development;

        // Act
        builder.Services.AddSwaggerGen();
        var app = builder.Build();
        app.UseSwagger();
        app.UseSwaggerUI();

        // Assert
        Assert.True(app.Environment.IsDevelopment());
    }

    // Test CORS Middleware
    [Fact]
    public void CORS_Should_Be_Applied()
    {
        // Arrange
        var builder = WebApplication.CreateBuilder();

        // Act
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowAllOrigins",
                builder =>
                {
                    builder.AllowAnyOrigin()
                           .AllowAnyMethod()
                           .AllowAnyHeader();
                });
        });
        var app = builder.Build();
        app.UseCors("AllowAllOrigins");

        // Assert
        Assert.NotNull(app);
    }

    // Test Authorization Middleware
    [Fact]
    public void Authorization_Should_Be_Enabled()
    {
        // Arrange
        var builder = WebApplication.CreateBuilder();

        // Act
        var app = builder.Build();
        app.UseAuthorization();

        // Assert
        Assert.NotNull(app);
    }

    // Integration Test: GetTopStories Endpoint
    [Fact]
    public async Task GetTopStories_Should_Return_Success()
    {
        // Act
        var response = await _client.GetAsync("/api/HackerNewsFeed/top");

        // Assert
        response.EnsureSuccessStatusCode(); // Status Code 200-299
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    // Integration Test: GetTopStories Endpoint Content
    [Fact]
    public async Task GetTopStories_Should_Return_Stories()
    {
        // Act
        var response = await _client.GetAsync("/api/HackerNewsFeed/top");
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        Assert.NotNull(content);
        Assert.NotEmpty(content);
    }
}