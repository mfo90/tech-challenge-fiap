using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Npgsql;
using Prometheus;
using RegionalContactsApp.Application.Services;
using RegionalContactsApp.Domain.Interfaces;
using RegionalContactsApp.Infrastructure.Repositories;
using System.Data;
using System.Security.Cryptography;
using System.Text;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Adicione a configuração da string de conexão
        string connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ??
            throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

        // Adicione serviços à coleção de injeção de dependência
        builder.Services.AddScoped<IContactRepository, ContactRepository>();
        builder.Services.AddScoped<IRegionRepository, RegionRepository>();
        builder.Services.AddScoped<IUserRepository, UserRepository>();
        builder.Services.AddScoped<IDatabaseInitializer, DatabaseInitializer>();

        builder.Services.AddScoped<IContactService, ContactService>();
        builder.Services.AddScoped<IRegionService, RegionService>();
        builder.Services.AddScoped<IUserService, UserService>();

        // Adicione a conexão do banco de dados como um serviço
        builder.Services.AddScoped<IDbConnection>(sp => new NpgsqlConnection(connectionString));

        // Adicione a configuração da string de conexão
        builder.Services.AddSingleton<IConfiguration>(builder.Configuration);

        // Adicione suporte para controllers
        builder.Services.AddControllers();

        builder.WebHost.UseKestrel()
            .UseUrls("http://*:6000");

        // Configure CORS, se necessário
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", builder =>
            {
                builder.AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader();
            });
        });

        // Adicione a autenticação JWT
        var key = Encoding.ASCII.GetBytes(builder.Configuration["Jwt:Key"]);
        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            options.RequireHttpsMetadata = false;
            options.SaveToken = true;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = builder.Configuration["Jwt:Issuer"],
                ValidAudience = builder.Configuration["Jwt:Issuer"],
                IssuerSigningKey = new SymmetricSecurityKey(key)
            };
        });

        // Configure Swagger/OpenAPI
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "Regional Contacts API", Version = "v1" });
            var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            if (File.Exists(xmlPath))
            {
                c.IncludeXmlComments(xmlPath);
            }
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });
            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Id = "Bearer",
                            Type = ReferenceType.SecurityScheme
                        }
                    },
                    new List<string>()
                }
            });
        });

        // Adicione a autorização e defina uma política para administradores
        builder.Services.AddAuthorization(options =>
        {
            options.AddPolicy("AdminPolicy", policy => policy.RequireRole("Admin"));
        });

        var app = builder.Build();

        app.UseMetricServer(); // Expor métricas em /metrics
        app.UseHttpMetrics();  // Coletar métricas HTTP

        // Inicialize o banco de dados
        using (var scope = app.Services.CreateScope())
        {
            var dbInitializer = scope.ServiceProvider.GetRequiredService<IDatabaseInitializer>();
            dbInitializer.Initialize();
        }

        // Configure o pipeline de requisição HTTP
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "Regional Contacts API v1");
        });

        // Configure CORS antes da autenticação e autorização
        app.UseCors("AllowAll");

        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        // Configure o roteamento e suporte para controllers
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });

        app.Run();
    }
}
