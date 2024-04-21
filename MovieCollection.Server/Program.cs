using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using MovieCollection.Application.Contracts.Persistence;
using MovieCollection.Application.Features;
using MovieCollection.Domain;
using MovieCollection.Domain.AccessControl;
using MovieCollection.Infrastructure.Authentication;
using MovieCollection.Infrastructure.Middleware;
using MovieCollection.Persistence;
using MovieCollection.Persistence.Repositories;
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
        var unitOfWork = serviceScope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        var genreAdventure = new Genre() { Name = "Adventure" };
        var genreDrama = new Genre() { Name = "Drama" };
        var genreSciFi = new Genre() { Name = "Sci-Fi" };
        var genreRomance = new Genre() { Name = "Romance" };

        unitOfWork.GenreRepository.InsertOrUpdate(genreAdventure);
        unitOfWork.GenreRepository.InsertOrUpdate(genreDrama);
        unitOfWork.GenreRepository.InsertOrUpdate(genreSciFi);
        unitOfWork.GenreRepository.InsertOrUpdate(genreRomance);

        var movieTheMartian = new Movie()
        {
            Title = "The Martian",
            Description = "An astronaut becomes stranded on Mars after his team assume him dead, and must rely on his ingenuity to find a way to signal to Earth that he is alive and can survive until a potential rescue.",
            ReleaseYear = 2015,
            Duration = new TimeSpan(2, 24, 0),
            Genres = new List<Genre>() { genreAdventure, genreDrama, genreSciFi }
        };
        unitOfWork.MovieRepository.InsertOrUpdate(movieTheMartian);

        var movieInterestellar = new Movie()
        {
            Title = "Interstellar",
            Description = "When Earth becomes uninhabitable in the future, a farmer and ex-NASA pilot, Joseph Cooper, is tasked to pilot a spacecraft, along with a team of researchers, to find a new planet for humans.",
            ReleaseYear = 2014,
            Duration = new TimeSpan(2, 49, 0),
            Genres = new List<Genre>() { genreAdventure, genreDrama, genreSciFi }
        };
        unitOfWork.MovieRepository.InsertOrUpdate(movieInterestellar);

        var movieTitanic = new Movie()
        {
            Title = "Titanic",
            Description = "A seventeen-year-old aristocrat falls in love with a kind but poor artist aboard the luxurious, ill-fated R.M.S. Titanic.",
            ReleaseYear = 1997,
            Duration = new TimeSpan(3, 14, 0),
            Genres = new List<Genre>() { genreDrama, genreRomance }
        };
        unitOfWork.MovieRepository.InsertOrUpdate(movieTitanic);

        unitOfWork.CommitAsync().GetAwaiter().GetResult();

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
        userManager.CreateAsync(admin, "Passw0rd!").GetAwaiter().GetResult();
        userManager.AddToRoleAsync(admin, "Administrator").GetAwaiter().GetResult();

        var user = new User()
        {
            UserName = "teste@teste.com",
            Email = "teste@teste.com",
            FirstName = "Felipe",
            LastName = "Soares",
            HasIncompletedInformation = false,
            MovieCollection = new List<Movie>()
            {
                movieTheMartian,
                movieInterestellar
            }
        };
        userManager.CreateAsync(user, "Passw0rd!").GetAwaiter().GetResult();

        var user2 = new User()
        {
            UserName = "user@teste.com",
            Email = "user@teste.com",
            FirstName = "Bruna",
            LastName = "Soares",
            HasIncompletedInformation = false,
            MovieCollection = new List<Movie>() { movieTitanic }
        };
        userManager.CreateAsync(user2, "Passw0rd!").GetAwaiter().GetResult();
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