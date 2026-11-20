
/* Base inicial de ACitas para SQL Server 2019.
   Ejecuta este archivo en SQL Server Management Studio.
   La autenticación del dueño se implementa con ASP.NET Core Identity,
   no con una tabla de contraseñas creada manualmente aquí.
*/
IF DB_ID(N'ACitasDB') IS NULL
BEGIN
    CREATE DATABASE ACitasDB;
END
GO

USE ACitasDB;
GO

IF OBJECT_ID(N'dbo.Clientes', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Clientes
    (
        ClienteId       INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Clientes PRIMARY KEY,
        Nombre          NVARCHAR(120) NOT NULL,
        Telefono        NVARCHAR(20) NOT NULL,
        Correo          NVARCHAR(254) NULL,
        FechaRegistro   DATETIME2(0) NOT NULL CONSTRAINT DF_Clientes_FechaRegistro DEFAULT SYSDATETIME(),
        Activo          BIT NOT NULL CONSTRAINT DF_Clientes_Activo DEFAULT 1
    );
    CREATE INDEX IX_Clientes_Telefono ON dbo.Clientes(Telefono);
END
GO

IF OBJECT_ID(N'dbo.Servicios', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Servicios
    (
        ServicioId      INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Servicios PRIMARY KEY,
        Nombre          NVARCHAR(100) NOT NULL,
        Descripcion     NVARCHAR(500) NULL,
        DuracionMinutos SMALLINT NOT NULL,
        Precio          DECIMAL(10,2) NULL,
        Activo          BIT NOT NULL CONSTRAINT DF_Servicios_Activo DEFAULT 1,
        CONSTRAINT CK_Servicios_Duracion CHECK (DuracionMinutos BETWEEN 5 AND 1440),
        CONSTRAINT CK_Servicios_Precio CHECK (Precio IS NULL OR Precio >= 0)
    );
END
GO

IF OBJECT_ID(N'dbo.Horarios', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Horarios
    (
        HorarioId       INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Horarios PRIMARY KEY,
        DiaSemana       TINYINT NOT NULL, -- 1=lunes ... 7=domingo
        HoraInicio      TIME(0) NOT NULL,
        HoraFin         TIME(0) NOT NULL,
        Activo          BIT NOT NULL CONSTRAINT DF_Horarios_Activo DEFAULT 1,
        CONSTRAINT CK_Horarios_Dia CHECK (DiaSemana BETWEEN 1 AND 7),
        CONSTRAINT CK_Horarios_Rango CHECK (HoraInicio < HoraFin)
    );
    CREATE INDEX IX_Horarios_Dia_Activo ON dbo.Horarios(DiaSemana, Activo);
END
GO

IF OBJECT_ID(N'dbo.Citas', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Citas
    (
        CitaId              INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Citas PRIMARY KEY,
        Folio               UNIQUEIDENTIFIER NOT NULL CONSTRAINT DF_Citas_Folio DEFAULT NEWSEQUENTIALID(),
        ClienteId           INT NOT NULL,
        ServicioId          INT NOT NULL,
        FechaHoraInicio     DATETIME2(0) NOT NULL,
        Estado              NVARCHAR(15) NOT NULL CONSTRAINT DF_Citas_Estado DEFAULT N'Pendiente',
        ComentarioCliente   NVARCHAR(500) NULL,
        NotaDueno           NVARCHAR(500) NULL,
        FechaCreacion       DATETIME2(0) NOT NULL CONSTRAINT DF_Citas_FechaCreacion DEFAULT SYSDATETIME(),
        FechaActualizacion  DATETIME2(0) NOT NULL CONSTRAINT DF_Citas_FechaActualizacion DEFAULT SYSDATETIME(),
        CONSTRAINT UQ_Citas_Folio UNIQUE(Folio),
        CONSTRAINT FK_Citas_Clientes FOREIGN KEY (ClienteId) REFERENCES dbo.Clientes(ClienteId),
        CONSTRAINT FK_Citas_Servicios FOREIGN KEY (ServicioId) REFERENCES dbo.Servicios(ServicioId),
        CONSTRAINT CK_Citas_Estado CHECK (Estado IN (N'Pendiente', N'Aceptada', N'Rechazada', N'Pospuesta', N'Cancelada'))
    );
    CREATE INDEX IX_Citas_FechaEstado ON dbo.Citas(FechaHoraInicio, Estado);
    CREATE INDEX IX_Citas_Cliente ON dbo.Citas(ClienteId, FechaCreacion DESC);
END
GO

IF OBJECT_ID(N'dbo.HistorialCitas', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.HistorialCitas
    (
        HistorialId     BIGINT IDENTITY(1,1) NOT NULL CONSTRAINT PK_HistorialCitas PRIMARY KEY,
        CitaId          INT NOT NULL,
        EstadoAnterior  NVARCHAR(15) NULL,
        EstadoNuevo     NVARCHAR(15) NOT NULL,
        FechaCambio     DATETIME2(0) NOT NULL CONSTRAINT DF_HistorialCitas_FechaCambio DEFAULT SYSDATETIME(),
        Nota            NVARCHAR(500) NULL,
        Usuario         NVARCHAR(256) NULL,
        CONSTRAINT FK_HistorialCitas_Citas FOREIGN KEY (CitaId) REFERENCES dbo.Citas(CitaId),
        CONSTRAINT CK_HistorialCitas_EstadoNuevo CHECK (EstadoNuevo IN (N'Pendiente', N'Aceptada', N'Rechazada', N'Pospuesta', N'Cancelada'))
    );
    CREATE INDEX IX_HistorialCitas_CitaFecha ON dbo.HistorialCitas(CitaId, FechaCambio DESC);
END
GO

IF OBJECT_ID(N'dbo.Notificaciones', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Notificaciones
    (
        NotificacionId BIGINT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Notificaciones PRIMARY KEY,
        CitaId          INT NOT NULL,
        Canal           NVARCHAR(20) NOT NULL CONSTRAINT DF_Notificaciones_Canal DEFAULT N'WhatsApp',
        Destino         NVARCHAR(30) NOT NULL,
        Mensaje         NVARCHAR(1000) NOT NULL,
        EstadoEnvio     NVARCHAR(15) NOT NULL CONSTRAINT DF_Notificaciones_Estado DEFAULT N'Pendiente',
        FechaCreacion   DATETIME2(0) NOT NULL CONSTRAINT DF_Notificaciones_Fecha DEFAULT SYSDATETIME(),
        FechaEnvio      DATETIME2(0) NULL,
        DetalleError    NVARCHAR(500) NULL,
        CONSTRAINT FK_Notificaciones_Citas FOREIGN KEY (CitaId) REFERENCES dbo.Citas(CitaId),
        CONSTRAINT CK_Notificaciones_Canal CHECK (Canal IN (N'WhatsApp')),
        CONSTRAINT CK_Notificaciones_Estado CHECK (EstadoEnvio IN (N'Pendiente', N'Enviada', N'Error'))
    );
    CREATE INDEX IX_Notificaciones_EstadoFecha ON dbo.Notificaciones(EstadoEnvio, FechaCreacion);
END
GO

/* Ejemplos opcionales: quita los comentarios para insertar servicios de prueba.
INSERT INTO dbo.Servicios (Nombre, Descripcion, DuracionMinutos, Precio)
VALUES (N'Consulta inicial', N'Primera consulta', 30, NULL),
       (N'Seguimiento', N'Cita de seguimiento', 20, NULL);

-- Lunes a viernes, 9:00 a 17:00
INSERT INTO dbo.Horarios (DiaSemana, HoraInicio, HoraFin)
VALUES (1,'09:00','17:00'), (2,'09:00','17:00'), (3,'09:00','17:00'),
       (4,'09:00','17:00'), (5,'09:00','17:00');
*/
