using System.Text;
using FacturaSii.src.FacturaComponent.Application.Interfaces;
using FacturaSii.src.FacturaComponent.Application.UseCases;
using FacturaSii.src.Auth.Interfaces;
using FacturaSii.src.Auth.Services.Interfaces;
using FacturaSii.src.Auth.Shared.Models;
using FacturaSii.src.FacturaComponent.Domain.Interfaces;
using FacturaSii.src.FacturaComponent.Infrastructure.Persistence;
using FacturaSii.src.FacturaComponent.Infrastructure.Repositorios;
using FacturaSii.src.FacturaComponent.Infrastructure.Servicios;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Shared.Mappings;
using FacturaSii.src.FacturaComponent.Domain.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "FacturaSii API", Version = "v1" });

    // Definimos el esquema de seguridad "Bearer"
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        Description = "Ingrese 'Bearer ' seguido de su token JWT"
    });

    // Requerimos el esquema de seguridad para todos los endpoints
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
            Array.Empty<string>()
        }
    });
});

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IServicioFirmaElectronica, ServicioFirmaElectronica>();
builder.Services.AddScoped<IFacturaRepository, FacturaRepository>();
builder.Services.AddScoped<IServicioEnvioSII, ServicioEnvioSII>();
builder.Services.AddScoped<IConsultarEstadoUseCase, ConsultarEstadoUseCase>();

builder.Services.AddAutoMapper(typeof(MappingProfile));
builder.Services.AddScoped<EmitirFacturaUseCase>();
builder.Services.AddScoped<IValidadorDeFolio, ValidadorDeFolio>();
// builder.Services.AddScoped<IRangoFolioProvider>(provider =>
//     new RangoFolioProviderDesdeXml("/app/caf/caf_actual.xml"));

builder.Services.AddScoped<IRangoFolioProvider>(provider => 
{
    var configuration = provider.GetRequiredService<IConfiguration>();
    var path = configuration.GetValue<string>("CafConfig:ArchivoCafPath");
    return new RangoFolioProviderDesdeXml(path);
});

// JWT Authentication
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
builder.Services.AddSingleton<ITokenService, TokenService>();


var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();
var key = Encoding.UTF8.GetBytes(jwtSettings.Key);

builder.Services.AddAuthentication(options => {
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options => {
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings.Issuer,
        ValidAudience = jwtSettings.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});

builder.Services.AddAuthorization();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

app.Run();
