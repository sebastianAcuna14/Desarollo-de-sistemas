USE master;

-- Crear la base de datos
CREATE DATABASE FerreteriaBD;
GO

USE FerreteriaBD;
GO

/*******************              TABLAS DEL SISTEMA                        ***********************/

-- Tabla: Cliente
CREATE TABLE Cliente (
	idCliente int IDENTITY(1,1),
    Cedula VARCHAR(15) PRIMARY KEY,
    Nombre VARCHAR(50),
    Apellido VARCHAR(50),
    Telefonos VARCHAR(50),
    Correo VARCHAR(100),
	contrasena varchar(50)
);
GO


-- ROL
CREATE TABLE ROL (
    IDRol INT IDENTITY(1,1) PRIMARY KEY,
    NombreRol NVARCHAR(50)
);
GO
select * from rol
----- INSERTS DE ROLES -----
INSERT INTO ROL (NombreRol) VALUES ('Administrador');
INSERT INTO ROL (NombreRol) VALUES ('Cliente');
INSERT INTO ROL (NombreRol) VALUES ('Empleado');



-- Tabla: Empleado
CREATE TABLE Empleado (
    IdEmpleado INT IDENTITY(1,1) PRIMARY KEY,
    Nombre VARCHAR(50),
    Apellido VARCHAR(50),
    Telefono VARCHAR(20),
    Correo VARCHAR(100),
    IdRol INT,
    FOREIGN KEY (IdRol) REFERENCES Rol(IdRol)
);
GO


--Tabla PROVEEDOR
CREATE TABLE Proveedor (
    IdProveedor INT PRIMARY KEY IDENTITY(1,1),
    NombreEmpresa NVARCHAR(100) NOT NULL,
    Correo NVARCHAR(100),
    Telefono NVARCHAR(20),
    Estado NVARCHAR(20) DEFAULT 'Activo'
);


--Tabla de Categoria
CREATE TABLE Categoria (
    IdCategoria INT IDENTITY(1,1) PRIMARY KEY,
    Nombre VARCHAR(100) NOT NULL,
    Descripcion VARCHAR(255)
);


-- Tabla: Inventario
CREATE TABLE INVENTARIO (
  IdProducto INT IDENTITY(1,1) PRIMARY KEY,
  Nombre VARCHAR(100),
  Descripcion VARCHAR(255),
  Cantidad INT,
  Precio DECIMAL(10,2),
  IdCategoria INT,
  IdProveedor INT,
  FOREIGN KEY (IdProveedor) REFERENCES Proveedor(IDProveedor),
  FOREIGN KEY (IdCategoria) REFERENCES Categoria(IdCategoria)
);
GO
ALTER TABLE INVENTARIO
ADD EnCatalogo BIT NOT NULL DEFAULT 1;

-- Tabla de Reparacion
CREATE TABLE Reparacion (
    IdReparacion INT IDENTITY(1,1) PRIMARY KEY,
    Fecha_Salida DATE,
    Fecha_Ingreso DATE,
    Descripcion VARCHAR(255),
    Estado VARCHAR(50),
    IdCliente VARCHAR(15),
    FOREIGN KEY (IdCliente) REFERENCES Cliente(Cedula)
);
GO


-- Tabla: Reporte
CREATE TABLE Reporte (
    IdReporte INT IDENTITY(1,1) PRIMARY KEY,
    Fecha_Salida DATE,
    Fecha_Ingreso DATE,
    Mensaje VARCHAR(255),
    Estado VARCHAR(50),
    IdCliente VARCHAR(15),
    IdProducto INT,
    FOREIGN KEY (IdCliente) REFERENCES Cliente(Cedula),
    FOREIGN KEY (IdProducto) REFERENCES Inventario(IdProducto)
);
GO


-- Tabla: Notificacion
CREATE TABLE Notificacion (
    IdNotificacion INT IDENTITY(1,1) PRIMARY KEY,
    IdCliente VARCHAR(15),
    IdRepara INT,
    Fecha_Envio DATE,
    Mensaje VARCHAR(255),
    FOREIGN KEY (IdCliente) REFERENCES Cliente(Cedula),
    FOREIGN KEY (IdRepara) REFERENCES Reparacion(IdReparacion)
);
GO

-- Tabla Carrito
CREATE TABLE Carrito (
    Id INT IDENTITY PRIMARY KEY,
    ProductoId INT NOT NULL,
    Cantidad INT NOT NULL,
    Nombre_Producto NVARCHAR(100) NOT NULL,
    Precio DECIMAL(18,2) NOT NULL
);

INSERT INTO Carrito (ProductoId, Cantidad, Nombre_Producto, Precio)
VALUES 
(1, 2, 'Martillo de acero', 8.50),
(2, 1, 'Taladro eléctrico', 59.99);


-- Tabla de movimientos financieros
CREATE TABLE Finanza (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Fecha DATETIME NOT NULL DEFAULT GETDATE(),
    Descripcion NVARCHAR(500) NOT NULL,
    Monto DECIMAL(10,2) NOT NULL,
    Tipo NVARCHAR(30) NOT NULL,
    FechaVencimiento DATE NULL,
    Pagada BIT NOT NULL DEFAULT 0,
    Anulada BIT NOT NULL DEFAULT 0
);
GO

-- Tabla de Notas de Crédito
CREATE TABLE NotaCredito (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Fecha DATETIME NOT NULL DEFAULT GETDATE(),
    Monto DECIMAL(18,2) NOT NULL,
    Comentario NVARCHAR(MAX) NULL
);
GO

-- Tabla principal de ventas
CREATE TABLE Venta (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Fecha DATETIME NOT NULL,
    NotaCreditoId INT NULL,
    FOREIGN KEY (NotaCreditoId) REFERENCES NotaCredito(Id)
);
GO

ALTER TABLE Venta
ADD 
    MetodoPago NVARCHAR(50) NULL,
    MontoTotal DECIMAL(18,2) NULL,
    Contacto NVARCHAR(100) NULL,
    Telefono NVARCHAR(20) NULL,
    Direccion NVARCHAR(200) NULL,
    ProvinciaId INT NULL,
    DepartamentoId INT NULL,
    DistritoId INT NULL,
    PaypalOrderId NVARCHAR(100) NULL;

-- Tabla de productos vendidos
CREATE TABLE ItemsVendidos (
    Id INT PRIMARY KEY IDENTITY(1,1),
    VentaId INT NOT NULL,
    Producto NVARCHAR(100) NOT NULL,
    Cantidad INT NOT NULL,
    PrecioUnitario DECIMAL(10,2) NOT NULL,
    FOREIGN KEY (VentaId) REFERENCES Venta(Id) ON DELETE CASCADE
);
GO

-- Tabla de métodos de pago
CREATE TABLE MetodosPago (
    Id INT PRIMARY KEY IDENTITY(1,1),
    VentaId INT NOT NULL,
    Monto DECIMAL(10,2) NOT NULL,
    Tipo NVARCHAR(20) NOT NULL,
    FOREIGN KEY (VentaId) REFERENCES Venta(Id) ON DELETE CASCADE
);
GO

-- Tabla de devoluciones
CREATE TABLE Devoluciones (
    Id INT PRIMARY KEY IDENTITY(1,1),
    VentaId INT NOT NULL,
    Fecha DATETIME NOT NULL DEFAULT GETDATE(),
    Motivo NVARCHAR(255),
    FOREIGN KEY (VentaId) REFERENCES Venta(Id) ON DELETE CASCADE
);
GO

-- Productos devueltos por devolución
CREATE TABLE ItemsDevueltos (
    Id INT PRIMARY KEY IDENTITY(1,1),
    DevolucionId INT NOT NULL,
    Producto NVARCHAR(100) NOT NULL,
    Cantidad INT NOT NULL,
    Observaciones NVARCHAR(255),
    FOREIGN KEY (DevolucionId) REFERENCES Devoluciones(Id) ON DELETE CASCADE
);
GO