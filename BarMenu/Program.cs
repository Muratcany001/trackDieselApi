using BarMenu.Abstract;
using BarMenu.Concrete;
using BarMenu.Entities;
using Microsoft.EntityFrameworkCore;
using BarMenu;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;

namespace BarMenu
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowFrontend", policy =>
                {
                    policy
                        .WithOrigins(
                            "https://localhost",
                            "https://localhost:4200",
                            "https://127.0.0.1:4200",
                            "https://localhost:4200",
                            "https://localhost:8100",
                            "https://track-diesel-gjagw81k9-muratcany001s-projects.vercel.app",
                            "https://track-diesel-ui.vercel.app",
                            "https://trackdieselapi.onrender.com"
                        )
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
            });

            // JWT Authentication ayarlarý
            var key = Encoding.ASCII.GetBytes("your-256-bit-long-secret-key-that-you-need-to-ensure-is-256-bits-long");
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.SaveToken = true;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.Zero
                    };
                });

            builder.Services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
            });

            // Add Swagger/OpenAPI configuration
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Track Diesel API", Version = "v1" });

                // JWT Authentication için Swagger ayarlarý
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] {}
                    }
                });
            });

            // Add DbContext
            builder.Services.AddDbContext<Context>(options =>
                options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Add scoped services
            builder.Services.AddScoped<ICarRepository, CarRepository>();
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IErrorRepository, ErrorRepository>();
            builder.Services.AddScoped<IPartRepository, PartRepository>();

            var app = builder.Build();

            // Data seeding
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var context = services.GetRequiredService<Context>();
                    var jsonPath = Path.Combine(AppContext.BaseDirectory, "ErrorCodes.json");
                    DataSeeder.SeedErrors(context, jsonPath);
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "Veritabaný seed hatasý!");
                }
            }

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Track Diesel API V1");
                    c.RoutePrefix = string.Empty;
                    c.OAuthClientId("swagger-ui");
                    c.OAuthAppName("Swagger UI");
                });
            }

            // HTTPS redirection - sadece production'da
            if (app.Environment.IsProduction())
            {
                app.UseHttpsRedirection();
            }

            // Middleware sýrasý çok önemli!
            app.UseCors("AllowFrontend");  // CORS en baþta olmalý
            app.UseAuthentication();       // Authentication, Authorization'dan önce
            app.UseAuthorization();        // Authorization, MapControllers'dan önce

            app.MapControllers();

            app.Run();
        }
    }
}