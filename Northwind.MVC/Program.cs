using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Caching.Memory;
using Northwind.DataContext;
using Northwind.MVC;
using Northwind.MVC.Data;
using Northwind.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDistributedMemoryCache();

builder.Services.AddSingleton<IMemoryCache>(new MemoryCache(
    new MemoryCacheOptions
    {
        TrackStatistics = true,
        SizeLimit = 50 // Products.
    })
);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(
    options =>
        options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
builder.Services.AddControllersWithViews().AddViewLocalization();

string? sqlServerConnection = builder.Configuration.GetConnectionString("DefaultConnection");

if (sqlServerConnection is null)
{
    Console.WriteLine("Northwind (DefaultConnection) database connection string is missing from configuration!");
}
else
{
    // If you are using SQL Server authentication then disable
    // Windows Integrated authentication and set user and password.
    SqlConnectionStringBuilder sql = new(sqlServerConnection);

    sql.IntegratedSecurity = false;
    sql.UserID = Environment.GetEnvironmentVariable("NORTHWINDMVC_USER");
    sql.Password = Environment.GetEnvironmentVariable("NORTHWINDMVC_PWD");

    builder.Services.AddNorthwindContext(sql.ConnectionString);
}

builder.Services.AddOutputCache(options =>
{
    options.DefaultExpirationTimeSpan =
    TimeSpan.FromSeconds(DurationInSeconds.TenSeconds);

    options.AddPolicy("views", p => p.SetVaryByQuery("alertstyle"));
});

builder.Services.AddHybridCache(options =>
{
    options.DefaultEntryOptions = new HybridCacheEntryOptions
    {
        Expiration = TimeSpan.FromSeconds(60),
        LocalCacheExpiration = TimeSpan.FromSeconds(30)
    };
});

builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();

var app = builder.Build();

string[] cultures = { "en-US", "en-GB", "fr", "fr-FR" };
RequestLocalizationOptions localizationOptions = new();

// cultures[0] will be "en-US"
localizationOptions.SetDefaultCulture(cultures[0])

    // Set globalization of data formats like dates and currencies.
    .AddSupportedCultures(cultures)

    // Set localization of user interface text.
    .AddSupportedUICultures(cultures);

app.UseRequestLocalization(localizationOptions);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.UseOutputCache();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();
//.CacheOutput(policyName: "views");

app.MapRazorPages()
   .WithStaticAssets();

app.MapGet("/notcached", () => DateTime.Now.ToString());
app.MapGet("/cached", () => DateTime.Now.ToString())
    .CacheOutput();


app.Run();
