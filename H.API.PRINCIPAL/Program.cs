using H.DataAccess;
using H.DataAccess.Infraestructure;
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
builder.Services.AddScoped<H.DataAccess.Infraestructure.IConnectionFactory>(
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
builder.Services.AddScoped<IEntradaInsumoService, EntradaInsumoService>();
builder.Services.AddScoped<IChatService, ChatService>();

// ============================================
// CONTROLLERS
// ============================================
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHttpClient();

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
        policy
            .AllowAnyOrigin()
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
        {
            // Estas tablas deben existir antes de ejecutar cualquier otra rutina de arranque.
            // Asi el modulo de solicitudes no depende de que otra migracion previa termine bien.
            await context.Database.ExecuteSqlRawAsync(@"
IF OBJECT_ID('dbo.TSolicitudPersonalizada', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.TSolicitudPersonalizada
    (
        Id INT IDENTITY(1,1) PRIMARY KEY, Codigo NVARCHAR(30) NOT NULL UNIQUE, IdPersona INT NOT NULL DEFAULT(0),
        NombreCliente NVARCHAR(150) NULL, EmailCliente NVARCHAR(150) NULL, TelefonoCliente NVARCHAR(30) NULL, TokenAcceso NVARCHAR(80) NOT NULL DEFAULT(''),
        Descripcion NVARCHAR(2000) NOT NULL, ImagenReferencia NVARCHAR(500) NULL, Evento NVARCHAR(100) NULL,
        Sabor NVARCHAR(100) NULL, Relleno NVARCHAR(100) NULL, Tamano NVARCHAR(50) NULL, Porciones INT NULL,
        Pisos INT NOT NULL DEFAULT(1), Cobertura NVARCHAR(100) NULL, Colores NVARCHAR(200) NULL,
        TextoDecorativo NVARCHAR(300) NULL, FechaEntregaSolicitada DATETIME NULL, PresupuestoMinimo DECIMAL(10,2) NULL,
        PresupuestoMaximo DECIMAL(10,2) NULL, EstimadoMinimo DECIMAL(10,2) NULL, EstimadoMaximo DECIMAL(10,2) NULL,
        Estado NVARCHAR(60) NOT NULL, Observaciones NVARCHAR(1000) NULL, Activo BIT NOT NULL DEFAULT(1),
        UsuarioCreacion NVARCHAR(100) NOT NULL, UsuarioModificacion NVARCHAR(100) NULL, FechaCreacion DATETIME NOT NULL, FechaModificacion DATETIME NULL
    );
END
IF OBJECT_ID('dbo.TCotizacionPersonalizada', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.TCotizacionPersonalizada
    (
        Id INT IDENTITY(1,1) PRIMARY KEY, IdSolicitud INT NOT NULL, Version INT NOT NULL, PrecioFinal DECIMAL(10,2) NOT NULL,
        Adelanto DECIMAL(10,2) NOT NULL, CostoDelivery DECIMAL(10,2) NOT NULL, FechaEntrega DATETIME NOT NULL,
        HoraEntrega TIME NULL, FechaVencimiento DATETIME NOT NULL, Observaciones NVARCHAR(1000) NULL, Estado NVARCHAR(40) NOT NULL,
        Activo BIT NOT NULL DEFAULT(1), UsuarioCreacion NVARCHAR(100) NOT NULL, UsuarioModificacion NVARCHAR(100) NULL,
        FechaCreacion DATETIME NOT NULL, FechaModificacion DATETIME NULL,
        CONSTRAINT FK_TCotizacionSolicitudEarly FOREIGN KEY (IdSolicitud) REFERENCES dbo.TSolicitudPersonalizada(Id)
    );
END
IF OBJECT_ID('dbo.TSolicitudHistorial', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.TSolicitudHistorial
    (
        Id INT IDENTITY(1,1) PRIMARY KEY, IdSolicitud INT NOT NULL, EstadoAnterior NVARCHAR(60) NULL, EstadoNuevo NVARCHAR(60) NOT NULL,
        Comentario NVARCHAR(1000) NULL, Activo BIT NOT NULL DEFAULT(1), UsuarioCreacion NVARCHAR(100) NOT NULL,
        UsuarioModificacion NVARCHAR(100) NULL, FechaCreacion DATETIME NOT NULL, FechaModificacion DATETIME NULL,
        CONSTRAINT FK_TSolicitudHistorialSolicitudEarly FOREIGN KEY (IdSolicitud) REFERENCES dbo.TSolicitudPersonalizada(Id)
    );
END");
            await context.Database.ExecuteSqlRawAsync(@"
IF NOT EXISTS (SELECT 1 FROM dbo.TEstadoVenta WHERE Nombre = 'Entregado')
    INSERT INTO dbo.TEstadoVenta (Nombre, Activo, UsuarioCreacion, FechaCreacion)
    VALUES ('Entregado', 1, 'sistema', GETDATE());
");
            await context.Database.ExecuteSqlRawAsync(@"
IF OBJECT_ID('dbo.TConfiguracionDelivery', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.TConfiguracionDelivery
    (
        Id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        CostoBase DECIMAL(10,2) NOT NULL DEFAULT (5.00),
        CostoPorKilometro DECIMAL(10,2) NOT NULL DEFAULT (1.50),
        LatitudCentro DECIMAL(10,6) NOT NULL DEFAULT (-13.531950),
        LongitudCentro DECIMAL(10,6) NOT NULL DEFAULT (-71.967460),
        RadioMaximoKm DECIMAL(10,2) NOT NULL DEFAULT (35.00),
        Activo BIT NOT NULL DEFAULT (1),
        UsuarioCreacion NVARCHAR(100) NOT NULL DEFAULT ('sistema'),
        UsuarioModificacion NVARCHAR(100) NULL,
        FechaCreacion DATETIME NOT NULL DEFAULT (GETDATE()),
        FechaModificacion DATETIME NULL
    );
END
IF NOT EXISTS (SELECT 1 FROM dbo.TConfiguracionDelivery WHERE Activo = 1)
    INSERT INTO dbo.TConfiguracionDelivery
    (CostoBase, CostoPorKilometro, LatitudCentro, LongitudCentro, RadioMaximoKm, Activo, UsuarioCreacion, FechaCreacion)
    VALUES (5.00, 1.50, -13.531950, -71.967460, 35.00, 1, 'sistema', GETDATE());");
            await context.Database.ExecuteSqlRawAsync(@"
IF OBJECT_ID('dbo.TTortaOpcion', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.TTortaOpcion
    (
        Id INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_TTortaOpcion PRIMARY KEY,
        IdTorta INT NOT NULL,
        Tipo NVARCHAR(20) NOT NULL,
        Valor NVARCHAR(150) NOT NULL,
        PrecioExtra DECIMAL(10,2) NOT NULL CONSTRAINT DF_TTortaOpcion_PrecioExtra DEFAULT (0),
        Maximo INT NULL,
        Orden INT NOT NULL CONSTRAINT DF_TTortaOpcion_Orden DEFAULT (0),
        Activo BIT NOT NULL CONSTRAINT DF_TTortaOpcion_Activo DEFAULT (1),
        UsuarioCreacion NVARCHAR(100) NOT NULL CONSTRAINT DF_TTortaOpcion_UsuarioCreacion DEFAULT ('sistema'),
        UsuarioModificacion NVARCHAR(100) NULL,
        FechaCreacion DATETIME NOT NULL CONSTRAINT DF_TTortaOpcion_FechaCreacion DEFAULT (GETDATE()),
        FechaModificacion DATETIME NULL,
        CONSTRAINT FK_TTortaOpcion_TTorta FOREIGN KEY (IdTorta) REFERENCES dbo.TTorta(Id),
        CONSTRAINT UQ_TTortaOpcion_TortaTipoValor UNIQUE (IdTorta, Tipo, Valor)
    );
END");
            await context.Database.ExecuteSqlRawAsync(@"
IF COL_LENGTH('dbo.TTortaOpcion', 'ModoPrecio') IS NULL
    ALTER TABLE dbo.TTortaOpcion ADD ModoPrecio NVARCHAR(20) NOT NULL CONSTRAINT DF_TTortaOpcion_ModoPrecio DEFAULT ('fijo');
IF COL_LENGTH('dbo.TTortaOpcion', 'PrecioPorUnidad') IS NULL
    ALTER TABLE dbo.TTortaOpcion ADD PrecioPorUnidad DECIMAL(10,2) NOT NULL CONSTRAINT DF_TTortaOpcion_PrecioPorUnidad DEFAULT (0);
IF COL_LENGTH('dbo.TTortaOpcion', 'Obligatorio') IS NULL
    ALTER TABLE dbo.TTortaOpcion ADD Obligatorio BIT NOT NULL CONSTRAINT DF_TTortaOpcion_Obligatorio DEFAULT (0);
IF COL_LENGTH('dbo.TTortaOpcion', 'Minimo') IS NULL
    ALTER TABLE dbo.TTortaOpcion ADD Minimo INT NULL;
IF COL_LENGTH('dbo.TTortaOpcion', 'Maximo') IS NULL
    ALTER TABLE dbo.TTortaOpcion ADD Maximo INT NULL;");
            await context.Database.ExecuteSqlRawAsync(@"
IF COL_LENGTH('dbo.TVenta', 'MontoPagado') IS NULL
    ALTER TABLE dbo.TVenta ADD MontoPagado DECIMAL(10,2) NOT NULL CONSTRAINT DF_TVenta_MontoPagado DEFAULT (0);
IF COL_LENGTH('dbo.TVenta', 'SaldoPendiente') IS NULL
    ALTER TABLE dbo.TVenta ADD SaldoPendiente DECIMAL(10,2) NOT NULL CONSTRAINT DF_TVenta_SaldoPendiente DEFAULT (0);
IF COL_LENGTH('dbo.TVenta', 'RequiereAnticipo') IS NULL
    ALTER TABLE dbo.TVenta ADD RequiereAnticipo BIT NOT NULL CONSTRAINT DF_TVenta_RequiereAnticipo DEFAULT (0);
IF COL_LENGTH('dbo.TVenta', 'CodigoEntrega') IS NULL
    ALTER TABLE dbo.TVenta ADD CodigoEntrega CHAR(6) NULL;
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'UX_TVenta_CodigoEntrega' AND object_id = OBJECT_ID('dbo.TVenta'))
    CREATE UNIQUE INDEX UX_TVenta_CodigoEntrega ON dbo.TVenta(CodigoEntrega) WHERE CodigoEntrega IS NOT NULL;");
            await context.Database.ExecuteSqlRawAsync(@"
IF COL_LENGTH('dbo.TEntregaDelivery', 'Latitud') IS NULL
    ALTER TABLE dbo.TEntregaDelivery ADD Latitud DECIMAL(10,7) NULL;
IF COL_LENGTH('dbo.TEntregaDelivery', 'Longitud') IS NULL
    ALTER TABLE dbo.TEntregaDelivery ADD Longitud DECIMAL(10,7) NULL;
IF COL_LENGTH('dbo.TEntregaDelivery', 'FechaAceptacion') IS NULL
    ALTER TABLE dbo.TEntregaDelivery ADD FechaAceptacion DATETIME NULL;
IF COL_LENGTH('dbo.TEntregaDelivery', 'FechaInicio') IS NULL
    ALTER TABLE dbo.TEntregaDelivery ADD FechaInicio DATETIME NULL;
IF COL_LENGTH('dbo.TEntregaDelivery', 'UsuarioAsignacion') IS NULL
    ALTER TABLE dbo.TEntregaDelivery ADD UsuarioAsignacion NVARCHAR(100) NULL;");
            await context.Database.ExecuteSqlRawAsync(@"
IF COL_LENGTH('dbo.TVentaDetalle', 'RellenoPersonalizado') IS NULL
    ALTER TABLE dbo.TVentaDetalle ADD RellenoPersonalizado NVARCHAR(100) NULL;
IF COL_LENGTH('dbo.TVentaDetalle', 'PisosPersonalizados') IS NULL
    ALTER TABLE dbo.TVentaDetalle ADD PisosPersonalizados INT NULL;
IF COL_LENGTH('dbo.TVentaDetalle', 'ColorDecoracionPersonalizada') IS NULL
    ALTER TABLE dbo.TVentaDetalle ADD ColorDecoracionPersonalizada NVARCHAR(100) NULL;
IF COL_LENGTH('dbo.TVentaDetalle', 'TamanoPersonalizado') IS NULL
    ALTER TABLE dbo.TVentaDetalle ADD TamanoPersonalizado NVARCHAR(50) NULL;
IF COL_LENGTH('dbo.TVentaDetalle', 'SaborPersonalizado') IS NULL
    ALTER TABLE dbo.TVentaDetalle ADD SaborPersonalizado NVARCHAR(100) NULL;
IF COL_LENGTH('dbo.TVentaDetalle', 'CoberturaPersonalizada') IS NULL
    ALTER TABLE dbo.TVentaDetalle ADD CoberturaPersonalizada NVARCHAR(100) NULL;
IF COL_LENGTH('dbo.TVentaDetalle', 'PorcionesPersonalizadas') IS NULL
    ALTER TABLE dbo.TVentaDetalle ADD PorcionesPersonalizadas INT NULL;
IF COL_LENGTH('dbo.TVentaDetalle', 'EventoPersonalizado') IS NULL
    ALTER TABLE dbo.TVentaDetalle ADD EventoPersonalizado NVARCHAR(100) NULL;
IF COL_LENGTH('dbo.TVentaDetalle', 'FechaEntregaSolicitada') IS NULL
    ALTER TABLE dbo.TVentaDetalle ADD FechaEntregaSolicitada DATETIME NULL;
IF COL_LENGTH('dbo.TVentaDetalle', 'ImagenReferencia') IS NULL
    ALTER TABLE dbo.TVentaDetalle ADD ImagenReferencia NVARCHAR(500) NULL;");
            await context.Database.ExecuteSqlRawAsync(@"
IF OBJECT_ID('dbo.TSolicitudPersonalizada', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.TSolicitudPersonalizada
    (
        Id INT IDENTITY(1,1) PRIMARY KEY, Codigo NVARCHAR(30) NOT NULL UNIQUE, IdPersona INT NOT NULL DEFAULT(0),
        NombreCliente NVARCHAR(150) NULL, EmailCliente NVARCHAR(150) NULL, TelefonoCliente NVARCHAR(30) NULL, TokenAcceso NVARCHAR(80) NOT NULL DEFAULT(''),
        Descripcion NVARCHAR(2000) NOT NULL, ImagenReferencia NVARCHAR(500) NULL, Evento NVARCHAR(100) NULL,
        Sabor NVARCHAR(100) NULL, Relleno NVARCHAR(100) NULL, Tamano NVARCHAR(50) NULL, Porciones INT NULL,
        Pisos INT NOT NULL DEFAULT(1), Cobertura NVARCHAR(100) NULL, Colores NVARCHAR(200) NULL,
        TextoDecorativo NVARCHAR(300) NULL, FechaEntregaSolicitada DATETIME NULL, PresupuestoMinimo DECIMAL(10,2) NULL,
        PresupuestoMaximo DECIMAL(10,2) NULL, EstimadoMinimo DECIMAL(10,2) NULL, EstimadoMaximo DECIMAL(10,2) NULL,
        Estado NVARCHAR(60) NOT NULL, Observaciones NVARCHAR(1000) NULL, Activo BIT NOT NULL DEFAULT(1),
        UsuarioCreacion NVARCHAR(100) NOT NULL, UsuarioModificacion NVARCHAR(100) NULL, FechaCreacion DATETIME NOT NULL, FechaModificacion DATETIME NULL
    );
END
IF COL_LENGTH('dbo.TSolicitudPersonalizada', 'NombreCliente') IS NULL ALTER TABLE dbo.TSolicitudPersonalizada ADD NombreCliente NVARCHAR(150) NULL;
IF COL_LENGTH('dbo.TSolicitudPersonalizada', 'EmailCliente') IS NULL ALTER TABLE dbo.TSolicitudPersonalizada ADD EmailCliente NVARCHAR(150) NULL;
IF COL_LENGTH('dbo.TSolicitudPersonalizada', 'TelefonoCliente') IS NULL ALTER TABLE dbo.TSolicitudPersonalizada ADD TelefonoCliente NVARCHAR(30) NULL;
IF COL_LENGTH('dbo.TSolicitudPersonalizada', 'TokenAcceso') IS NULL ALTER TABLE dbo.TSolicitudPersonalizada ADD TokenAcceso NVARCHAR(80) NOT NULL CONSTRAINT DF_TSolicitudToken DEFAULT('');
IF OBJECT_ID('dbo.TCotizacionPersonalizada', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.TCotizacionPersonalizada
    (
        Id INT IDENTITY(1,1) PRIMARY KEY, IdSolicitud INT NOT NULL, Version INT NOT NULL, PrecioFinal DECIMAL(10,2) NOT NULL,
        Adelanto DECIMAL(10,2) NOT NULL, CostoDelivery DECIMAL(10,2) NOT NULL, FechaEntrega DATETIME NOT NULL,
        HoraEntrega TIME NULL, FechaVencimiento DATETIME NOT NULL, Observaciones NVARCHAR(1000) NULL, Estado NVARCHAR(40) NOT NULL,
        Activo BIT NOT NULL DEFAULT(1), UsuarioCreacion NVARCHAR(100) NOT NULL, UsuarioModificacion NVARCHAR(100) NULL,
        FechaCreacion DATETIME NOT NULL, FechaModificacion DATETIME NULL,
        CONSTRAINT FK_TCotizacionSolicitud FOREIGN KEY (IdSolicitud) REFERENCES dbo.TSolicitudPersonalizada(Id)
    );
END
IF OBJECT_ID('dbo.TSolicitudHistorial', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.TSolicitudHistorial
    (
        Id INT IDENTITY(1,1) PRIMARY KEY, IdSolicitud INT NOT NULL, EstadoAnterior NVARCHAR(60) NULL, EstadoNuevo NVARCHAR(60) NOT NULL,
        Comentario NVARCHAR(1000) NULL, Activo BIT NOT NULL DEFAULT(1), UsuarioCreacion NVARCHAR(100) NOT NULL,
        UsuarioModificacion NVARCHAR(100) NULL, FechaCreacion DATETIME NOT NULL, FechaModificacion DATETIME NULL,
        CONSTRAINT FK_TSolicitudHistorialSolicitud FOREIGN KEY (IdSolicitud) REFERENCES dbo.TSolicitudPersonalizada(Id)
    );
END");
            Console.WriteLine("✅ Conexión a SQL Server exitosa.");
        }
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
//if (app.Environment.IsDevelopment())
//{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "H.API.PRINCIPAL v1");
    });
//}

app.UseHttpsRedirection();

app.UseCors("UI-FrontEnd");

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
