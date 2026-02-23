using H.DataAccess;
using H.DataAccess.Infrastructure;
using H.DataAccess.Repositorios;
using H.DataAccess.UnitofWork;
using H.Services;
using H.Services.Configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ============================================
// CONEXIÓN SQL SERVER
// ============================================
builder.Services.AddDbContext<sistemContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
    )
);

// ============================================
// CONNECTION FACTORY (DAPPER)
// ============================================
builder.Services.AddScoped<H.DataAccess.Infrastructure.IConnectionFactory>(
    provider => new ConnectionFactory(
        builder.Configuration.GetConnectionString("DefaultConnection")
    )
);

// ============================================
// UNIT OF WORK
// ============================================
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// ============================================
// REPOSITORIOS
// ============================================
builder.Services.AddScoped<IAuthRepository, AuthRepository>();

// ============================================
// SERVICIOS
// ============================================
builder.Services.AddScoped<IAuthService, AuthService>();

// ============================================
// CONTROLLERS
// ============================================
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Configuración de Cloudinary
builder.Services.Configure<CloudinarySettings>(
    builder.Configuration.GetSection("Cloudinary"));

builder.Services.AddScoped<ICloudinaryService, CloudinaryService>();

// ============================================
// JWT AUTHENTICATION
// ============================================
var jwtKey = builder.Configuration["Jwt:Key"]
    ?? throw new Exception("JWT Key no configurada en appsettings.json");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtKey)),
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidateAudience = true,
            ValidAudience = builder.Configuration["Jwt:Audience"],
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

// ============================================
// CORS
// ============================================
builder.Services.AddCors(options =>
{
    options.AddPolicy("UI-FrontEnd", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// ============================================
// SWAGGER CON JWT
// ============================================
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "H.API.PRINCIPAL",
        Version = "v1",
        Description = "API Sistema de Ventas de Tortas"
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization. Ingresa: Bearer {tu_token}",
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
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// ============================================
// BUILD APP
// ============================================
var app = builder.Build();

// ============================================
// VERIFICAR CONEXIÓN A SQL SERVER
// ============================================
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<sistemContext>();
        var canConnect = await context.Database.CanConnectAsync();
        if (canConnect)
            Console.WriteLine("✅ Conexión a SQL Server exitosa.");
        else
            Console.WriteLine("❌ No se pudo conectar a SQL Server.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Error al conectar a SQL Server: {ex.Message}");
    }
}

// ============================================
// PIPELINE
// ============================================
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "H.API.PRINCIPAL v1");
    });
}

app.UseHttpsRedirection();

app.UseCors("UI-FrontEnd");

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();