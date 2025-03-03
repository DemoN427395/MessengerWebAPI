using System.Text;
using AuthService.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

using MessengerLib.Data;
using MessengerLib.Interfaces;
using MessengerLib.Models;

namespace AuthService
{
    public class AuthService
    {
        public static void Main(string[] args)
        {
            try
            {
                var builder = WebApplication.CreateBuilder(args);
                builder.Services.AddControllers();

                // CORS setup to allow specific origins
                builder.Services.AddCors(options =>
                {
                    options.AddPolicy("AllowAll", policy =>
                    {
                        policy.WithOrigins("http://localhost:5001", "http://localhost:4000") // MessageService & Gateway URLs
                            .AllowAnyHeader()
                            .AllowAnyMethod()
                            .AllowCredentials();
                    });
                });

                string connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

                // Add hosted service for migrations
                builder.Services.AddHostedService<MigrationHostedService>();

                builder.Services.AddDbContext<AuthDbContext>(options =>
                    options.UseNpgsql(connectionString, b =>
                    {
                        b.MigrationsAssembly("MessengerLib");
                    }));

                // Identity and JWT authentication setup
                builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
                    .AddEntityFrameworkStores<AuthDbContext>()
                    .AddDefaultTokenProviders();

                builder.Services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    var secret = builder.Configuration["JWT:secret"]
                                 ?? throw new ArgumentNullException("JWT:secret is not configured");

                    var validAudience = builder.Configuration["JWT:ValidAudience"]
                                        ?? throw new ArgumentNullException("JWT:ValidAudience is not configured");

                    var validIssuer = builder.Configuration["JWT:ValidIssuer"]
                                      ?? throw new ArgumentNullException("JWT:ValidIssuer is not configured");

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidAudience = validAudience,
                        ValidIssuer = validIssuer,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret))
                    };
                });

                builder.Services.AddAuthorization();
                builder.Services.AddScoped<ITokenService, TokenService>();

                var app = builder.Build();

                // Apply migrations on startup
                using (var scope = app.Services.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<AuthDbContext>();
                    context.Database.Migrate();
                }

                // Use CORS middleware with "AllowAll" policy
                app.UseCors("AllowAll");

                app.UseAuthentication();
                app.UseAuthorization();

                // Register controllers and run the app
                app.MapControllers();
                app.Run();
            }
            catch (Exception ex)
            {
                // Catch and log exceptions during startup
                Console.WriteLine(ex.Message);
                Console.ReadKey();
            }
        }
    }
}
