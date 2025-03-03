using System.Text;
using MessageService.Hubs;
using MessageService.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;

// ���������� �������� ����� � redis �� postgres

namespace MessageService
{
    public class Service
    {
        public static void Main(string[] args)
        {
            try
            {
                var builder = WebApplication.CreateBuilder(args);

                // ��������� CORS
                builder.Services.AddCors(options =>
                {
                    options.AddPolicy("AllowServices", policy =>
                    {
                        policy.WithOrigins("http://localhost:5000", "http://localhost:4000")
                            .AllowAnyHeader()
                            .AllowAnyMethod()
                            .AllowAnyMethod()
                            .AllowCredentials();
                    });
                });

                // ��������� ��������������
                builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(options =>
                    {
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateIssuer = true,
                            ValidateAudience = true,
                            // ���������� ���������� ����� �� ������������
                            ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
                            ValidAudience = builder.Configuration["JWT:ValidAudience"],
                            IssuerSigningKey = new SymmetricSecurityKey(
                                Encoding.UTF8.GetBytes(builder.Configuration["JWT:secret"]))
                        };

                        // ��� WebSocket ����������
                        options.Events = new JwtBearerEvents
                        {
                            OnMessageReceived = context =>
                            {
                                var accessToken = context.Request.Query["access_token"];
                                var path = context.HttpContext.Request.Path;

                                if (!string.IsNullOrEmpty(accessToken) &&
                                    path.StartsWithSegments("/chatHub"))
                                {
                                    context.Token = accessToken;
                                }
                                return Task.CompletedTask;
                            }
                        };
                    });



                // ��������� SignalR
                builder.Services.AddSignalR(options =>
                {
                    options.EnableDetailedErrors = true;
                    options.ClientTimeoutInterval = TimeSpan.FromMinutes(2);
                });

                // ��������� �����������
                builder.Services.AddAuthorization(options =>
                {
                    options.DefaultPolicy = new AuthorizationPolicyBuilder()
                        .RequireAuthenticatedUser()
                        .Build();
                });

                // ����������� ��������
                builder.Services.AddSingleton<RabbitMQStreamService>();

                builder.Services.AddHttpClient<AuthServiceClient>(client =>
                {
                    client.BaseAddress = new Uri(builder.Configuration["ConnectionStrings:AuthServiceHttpUrl"]);
                });

                builder.Services.AddHttpClient<AuthServiceClient>(client =>
                {
                    client.BaseAddress = new Uri(builder.Configuration["ConnectionStrings:AuthServiceHttpsUrl"]);
                });

                var app = builder.Build();

                app.UseCors("AllowServices");
                app.UseAuthentication();
                app.UseAuthorization();

                app.MapHub<ChatHub>("/chatHub");
                app.MapGet("/", () => "SignalR Chat Service");

                app.Run();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.ReadKey();
            }
        }
    }
}