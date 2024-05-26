using Microsoft.OpenApi.Models;
using Npgsql;
using RegionalContactsApp.Application.Services;
using RegionalContactsApp.Domain.Interfaces;
using RegionalContactsApp.Infrastructure.Repositories;
using System.Data;

var builder = WebApplication.CreateBuilder(args);

// Adicione a configuração da string de conexão
string connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ??
    throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");


// Adicione serviços à coleção de injeção de dependência
builder.Services.AddScoped<IContactRepository, ContactRepository>();
builder.Services.AddScoped<IRegionRepository, RegionRepository>();
builder.Services.AddScoped<IContactService, ContactService>();
builder.Services.AddScoped<IRegionService, RegionService>();

// Adicione a conexão do banco de dados como um serviço
builder.Services.AddScoped<IDbConnection>((sp) => new NpgsqlConnection(connectionString));

// Adicione a configuração da string de conexão
builder.Services.AddSingleton<IConfiguration>(builder.Configuration);

// Adicione suporte para controllers
builder.Services.AddControllers();

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

// Configure Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Regional Contacts API", Version = "v1" });
    // Inclua comentários do XML (se houver) para melhorar a documentação do Swagger
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath)) // Verifique se o arquivo XML existe
    {
        c.IncludeXmlComments(xmlPath);
    }
});

var app = builder.Build();

// Configure o pipeline de requisição HTTP
//if (app.Environment.IsDevelopment())
//{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Regional Contacts API v1");
    });
//}

// Configure CORS, se necessário
app.UseCors("AllowAll");

// Configure o roteamento e suporte para controllers
app.UseRouting();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();
