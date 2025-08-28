using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Caching.Hybrid;
using Northwind.DataContext;
using Northwind.Repositories;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthorization();
builder.Services.AddAuthentication(defaultScheme: "Bearer")
    .AddJwtBearer();

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "Northwind.Mvc.Policy",
    policy =>
    {
        policy.WithOrigins("https://localhost:7297");
    });
});

builder.Services.AddNorthwindContext();

builder.Services.AddControllers(options =>
{
    WriteLine("Default output formatters:");
    foreach (IOutputFormatter formatter in options.OutputFormatters)
    {
        OutputFormatter? mediaFormatter = formatter as OutputFormatter;
        if (mediaFormatter is null)
        {
            WriteLine($"  {formatter.GetType().Name}");
        }
        else // OutputFormatter class has SupportedMediaTypes.
        {
            WriteLine("  {0}, Media types: {1}",
              arg0: mediaFormatter.GetType().Name,
              arg1: string.Join(", ",
                mediaFormatter.SupportedMediaTypes));
        }
    }
});

builder.Services.AddOpenApi();

builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();

builder.Services.AddHybridCache(options =>
{
    options.DefaultEntryOptions = new HybridCacheEntryOptions
    {
        Expiration = TimeSpan.FromSeconds(60),
        LocalCacheExpiration = TimeSpan.FromSeconds(30)
    };
});

builder.Services.AddResponseCaching();

builder.Services.AddHttpLogging(options =>
{
    // Add the Origin header so it will not be redacted.
    options.RequestHeaders.Add("Origin");

    options.LoggingFields = HttpLoggingFields.All;
    options.RequestBodyLogLimit = 4096; // Default is 32k.
    options.ResponseBodyLogLimit = 4096; // Default is 32k.
});

var app = builder.Build();

app.UseHttpLogging();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseCors(policyName: "Northwind.Mvc.Policy");

app.UseResponseCaching();

app.UseAuthorization();

app.MapControllers();

app.MapGet("/secret", (ClaimsPrincipal user) =>
    string.Format("Welcome, {0}. The secret ingredient is love.", user.Identity?.Name ?? "secure user"))
    .RequireAuthorization();

app.Run();
