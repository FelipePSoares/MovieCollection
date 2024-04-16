using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using MovieCollection.Persistence;
using MovieCollection.Application.Features;
using MovieCollection.Server.Extensions;
using Newtonsoft.Json.Converters;
using MovieCollection.Infrastructure.Authentication;

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


var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
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