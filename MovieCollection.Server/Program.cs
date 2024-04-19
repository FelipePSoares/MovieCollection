using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using MovieCollection.Application.Features;
using MovieCollection.Domain.AccessControl;
using MovieCollection.Infrastructure.Authentication;
using MovieCollection.Infrastructure.Middleware;
using MovieCollection.Persistence;
using MovieCollection.Server.Extensions;
using Newtonsoft.Json.Converters;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

var tokenSettings = builder.Configuration.GetSection("TokenSettings").Get<TokenSettings>() ?? default!;
builder.Services.AddSingleton(tokenSettings);

// Add services to the container.
builder.Services.AddPersistenceServices(builder.Configuration);
builder.Services.AddApplicationServices();
builder.Services.AddAuthenticationServices(builder.Configuration);

builder.Services.AddControllers(config =>
{
    var policy = new AuthorizationPolicyBuilder()
                     .RequireAuthenticatedUser()
                     .Build();
    config.Filters.Add(new AuthorizeFilter(policy));
})
    .AddNewtonsoftJson(setup =>
        setup.SerializerSettings.Converters.Add(new StringEnumConverter()));

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Add support to logging with SERILOG
builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

//Add support to logging request with SERILOG
app.UseSerilogRequestLogging();

using (var serviceScope = app.Services.CreateScope())
{
    app.UseCustomExceptionHandler(serviceScope.ServiceProvider.GetRequiredService<ILogger<ExceptionMiddleware>>());
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    using (var serviceScope = app.Services.CreateScope())
    {
        var roleManager = serviceScope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
        var adminRole = new IdentityRole<Guid>("Administrator");
        roleManager.CreateAsync(adminRole).GetAwaiter().GetResult();

        var userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<User>>();

        var admin = new User()
        {
            UserName = "Admin",
            Email = "admin@admin.com",
            FirstName = "Admin",
            LastName = "Admin",
            HasIncompletedInformation = false
        };
        userManager.CreateAsync(admin, "Admin@123456").GetAwaiter().GetResult();
        userManager.AddToRoleAsync(admin, "Administrator").GetAwaiter().GetResult();
    }
}
else
{
    app.UseMigration();
    app.MapHealthChecks("/healthcheck/readness");
}

app.UseAuthorization();

app.MapControllers();

app.MapFallbackToFile("/index.html");

app.Run();