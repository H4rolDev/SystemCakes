IF OBJECT_ID('dbo.TConfiguracionDelivery', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.TConfiguracionDelivery
    (
        Id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        CostoBase DECIMAL(10,2) NOT NULL CONSTRAINT DF_TConfiguracionDelivery_CostoBase DEFAULT (5.00),
        CostoPorKilometro DECIMAL(10,2) NOT NULL CONSTRAINT DF_TConfiguracionDelivery_CostoKm DEFAULT (1.50),
        LatitudCentro DECIMAL(10,6) NOT NULL CONSTRAINT DF_TConfiguracionDelivery_Latitud DEFAULT (-13.531950),
        LongitudCentro DECIMAL(10,6) NOT NULL CONSTRAINT DF_TConfiguracionDelivery_Longitud DEFAULT (-71.967460),
        RadioMaximoKm DECIMAL(10,2) NOT NULL CONSTRAINT DF_TConfiguracionDelivery_Radio DEFAULT (35.00),
        Activo BIT NOT NULL CONSTRAINT DF_TConfiguracionDelivery_Activo DEFAULT (1),
        UsuarioCreacion NVARCHAR(100) NOT NULL CONSTRAINT DF_TConfiguracionDelivery_Usuario DEFAULT ('sistema'),
        UsuarioModificacion NVARCHAR(100) NULL,
        FechaCreacion DATETIME NOT NULL CONSTRAINT DF_TConfiguracionDelivery_Fecha DEFAULT (GETDATE()),
        FechaModificacion DATETIME NULL
    );
END;

IF NOT EXISTS (SELECT 1 FROM dbo.TConfiguracionDelivery WHERE Activo = 1)
BEGIN
    INSERT INTO dbo.TConfiguracionDelivery
        (CostoBase, CostoPorKilometro, LatitudCentro, LongitudCentro, RadioMaximoKm,
         Activo, UsuarioCreacion, FechaCreacion)
    VALUES (5.00, 1.50, -13.531950, -71.967460, 35.00, 1, 'sistema', GETDATE());
END;

IF COL_LENGTH('dbo.TEntregaDelivery', 'Latitud') IS NULL
    ALTER TABLE dbo.TEntregaDelivery ADD Latitud DECIMAL(10,7) NULL;
IF COL_LENGTH('dbo.TEntregaDelivery', 'Longitud') IS NULL
    ALTER TABLE dbo.TEntregaDelivery ADD Longitud DECIMAL(10,7) NULL;
IF COL_LENGTH('dbo.TEntregaDelivery', 'FechaAceptacion') IS NULL
    ALTER TABLE dbo.TEntregaDelivery ADD FechaAceptacion DATETIME NULL;
IF COL_LENGTH('dbo.TEntregaDelivery', 'FechaInicio') IS NULL
    ALTER TABLE dbo.TEntregaDelivery ADD FechaInicio DATETIME NULL;
IF COL_LENGTH('dbo.TEntregaDelivery', 'UsuarioAsignacion') IS NULL
    ALTER TABLE dbo.TEntregaDelivery ADD UsuarioAsignacion NVARCHAR(100) NULL;
