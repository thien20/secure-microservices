using System.Text;
using System.Threading.RateLimiting;

using AuthService.Services;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;

namespace AuthService
{
    public class Startup
    {
        public IConfiguration _configuration { get; }

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // Configure services for the application 
        // through `dependency injection`
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
            
            services.AddLogging(logging =>
            {
                logging.ClearProviders();
                logging.AddConsole();
            });

            // Add JWT authentication
            var jwtKey = _configuration["Jwt:Key"];
            if (string.IsNullOrEmpty(jwtKey))
            {
                throw new InvalidOperationException("JWT Key is not configured.");
            }

            services.AddAuthentication("Bearer")
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = _configuration["Jwt:Issuer"],
                        ValidAudience = _configuration["Jwt:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
                    };

                    options.Events = new JwtBearerEvents
                    {
                        OnAuthenticationFailed = context =>
                        {
                            Console.WriteLine("Authentication failed: " + context.Exception.Message);
                            return Task.CompletedTask;
                        },
                        OnTokenValidated = context =>
                        {
                            Console.WriteLine("Token validated successfully.");
                            return Task.CompletedTask;
                        }
                    };
                });


            // Add rate limiting middleware
            services.AddRateLimiter(options =>
            {
                options.AddFixedWindowLimiter(policyName: "fixed", options =>
                {
                    options.PermitLimit = 3;
                    options.Window = TimeSpan.FromSeconds(5);
                    options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                });
                options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            });

          
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", builder =>
                {
                    builder.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader();
                });
                options.AddPolicy("SpecificOrigins", builder =>
                {
                    builder.WithOrigins("http://localhost:8000") // Python Business Service
                        .AllowAnyMethod()
                        .AllowAnyHeader();
                
                });
            });

            // Register dependency injection services
            services.AddSingleton<IUserService, UserService>();
            services.AddSingleton<ITokenService, TokenService>();
            services.AddSingleton<IFailedLoginTracker, FailedLoginTracker>();

        }

        // Configure the HTTP request pipeline
        public void Configure(WebApplication app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("/swagger/v1/swagger.json", "AuthService API");
                    options.RoutePrefix = string.Empty;
                });
                app.UseDeveloperExceptionPage();
            }
            // else
            // {
            //     // app.UseHsts();
            // }

            app.UseCors("AllowAll");
            // app.UseHttpsRedirection();
            app.UseRateLimiter();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();
        }
    }
}
