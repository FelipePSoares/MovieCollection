using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using MovieCollection.Domain.AccessControl;
using MovieCollection.Infrastructure.Authentication;
using MovieCollection.Persistence;

namespace MovieCollection.Server.Extensions
{
    public static class AuthenticationMiddleware
    {
        public static IServiceCollection AddAuthenticationServices(this IServiceCollection services, IConfiguration configuration)
        {
            var tokenSettings = configuration.GetSection("TokenSettings").Get<TokenSettings>() ?? default!;

            services.AddAuthorizationBuilder();

            services.AddIdentityCore<User>()
                .AddSignInManager()
                .AddRoles<IdentityRole<Guid>>()
                .AddEntityFrameworkStores<MovieCollectionDatabaseContext>()
                .AddClaimsPrincipalFactory<CustomClaimsPrincipalFactory>()
                .AddTokenProvider<DataProtectorTokenProvider<User>>("REFRESHTOKENPROVIDER");

            services.Configure<IdentityOptions>(options =>
            {
                // Default Password settings.
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequiredLength = 8;
                // Default SignIn settings.
                options.SignIn.RequireConfirmedEmail = false;
                options.User.RequireUniqueEmail = true;
            });

            services.Configure<DataProtectionTokenProviderOptions>(options =>
            {
                options.TokenLifespan = TimeSpan.FromSeconds(tokenSettings.RefreshTokenExpireSeconds);
            });

            services
                .AddAuthentication(config =>
                {
                    config.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    config.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                    config.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(config =>
                {
                    config.RequireHttpsMetadata = false;
                    config.SaveToken = false;
                    config.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenSettings.SecretKey)),
                        ValidateIssuer = !string.IsNullOrEmpty(tokenSettings.Issuer),
                        ValidIssuer = tokenSettings.Issuer,
                        ValidateAudience = !string.IsNullOrEmpty(tokenSettings.Audience),
                        ValidAudience = tokenSettings.Audience,
                        ClockSkew = TimeSpan.Zero
                    };
                });

            return services;
        }
    }
}
