-- -----------------------------------------------------
-- Schema cerverica
-- -----------------------------------------------------
IF EXISTS (SELECT * FROM sys.schemas WHERE name = 'cerverica')
BEGIN
    DROP SCHEMA cerverica;
END;
GO

CREATE SCHEMA cerverica;
GO

-- -----------------------------------------------------
-- Table 1 cerverica.proveedores
-- -----------------------------------------------------
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'cerverica' AND TABLE_NAME = 'proveedores')
BEGIN
    DROP TABLE cerverica.proveedores;
END;
GO

CREATE TABLE cerverica.proveedores (
  id INT NOT NULL PRIMARY KEY IDENTITY,
  empresa VARCHAR(45) NOT NULL,
  direccion VARCHAR(45) NOT NULL,
  nombre_proveedor VARCHAR(45) NOT NULL
);
GO

-- -----------------------------------------------------
-- Table 2 cerverica.insumos
-- -----------------------------------------------------
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'cerverica' AND TABLE_NAME = 'insumos')
BEGIN
    DROP TABLE cerverica.insumos;
END;
GO

CREATE TABLE cerverica.insumos (
  id INT NOT NULL PRIMARY KEY IDENTITY,
  nombre VARCHAR(45) NOT NULL,
  descripcion VARCHAR(45) NOT NULL,
  unidad_medida FLOAT NOT NULL,
  cantidad_maxima FLOAT NOT NULL,
  cantidad_minima FLOAT NOT NULL,
  merma FLOAT NOT NULL
);
GO

-- -----------------------------------------------------
-- Table 3 cerverica.lotes_insumos
-- -----------------------------------------------------
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'cerverica' AND TABLE_NAME = 'lotes_insumos')
BEGIN
    DROP TABLE cerverica.lotes_insumos;
END;
GO

CREATE TABLE cerverica.lotes_insumos (
  id INT NOT NULL PRIMARY KEY IDENTITY,
  idProveedor INT NOT NULL,
  idInsumo INT NOT NULL,
  fecha_caducidad DATE NOT NULL,
  cantidad FLOAT NOT NULL,
  fecha_compra DATE NOT NULL,
  precio_unidad FLOAT NOT NULL,
  CONSTRAINT fk_lotes_insumos_proveedores FOREIGN KEY (idProveedor)
    REFERENCES cerverica.proveedores (id)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT fk_lotes_insumos_insumos FOREIGN KEY (idInsumo)
    REFERENCES cerverica.insumos (id)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION
);
GO

-- -----------------------------------------------------
-- Table 4 cerverica.receta
-- -----------------------------------------------------
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'cerverica' AND TABLE_NAME = 'receta')
BEGIN
    DROP TABLE cerverica.receta;
END;
GO

CREATE TABLE cerverica.receta (
  id INT NOT NULL PRIMARY KEY IDENTITY,
  litros_estimados FLOAT NOT NULL,
  utilidad FLOAT NULL,
  piezas_estimadas INT NOT NULL,
  descripcion VARCHAR(500) NULL,
  nombre VARCHAR(50) NOT NULL
);
GO

-- -----------------------------------------------------
-- Table 5 cerverica.ingredientes_receta
-- -----------------------------------------------------
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'cerverica' AND TABLE_NAME = 'ingredientes_receta')
BEGIN
    DROP TABLE cerverica.ingredientes_receta;
END;
GO

CREATE TABLE cerverica.ingredientes_receta (
  idReceta INT NOT NULL,
  idInsumo INT NOT NULL,
  cantidad FLOAT NOT NULL,
  PRIMARY KEY (idReceta, idInsumo),
  CONSTRAINT fk_ingredientes_receta_receta FOREIGN KEY (idReceta)
    REFERENCES cerverica.receta (id)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT fk_ingredientes_receta_insumos FOREIGN KEY (idInsumo)
    REFERENCES cerverica.insumos (id)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION
);
GO

-- -----------------------------------------------------
-- Table 6 cerverica.producciones
-- -----------------------------------------------------
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'cerverica' AND TABLE_NAME = 'producciones')
BEGIN
    DROP TABLE cerverica.producciones;
END;
GO

CREATE TABLE cerverica.producciones (
  id INT NOT NULL PRIMARY KEY IDENTITY,
  fecha_produccion DATE NOT NULL,
  mensaje VARCHAR(50) NULL,
  status TINYINT NOT NULL,
  tandas INT NOT NULL,
  litros_finales FLOAT NULL,
  idReceta INT NOT NULL,
  fecha_solicitud DATE NULL,
  idUsuarioSolicitud NVARCHAR(450) NOT NULL,
  idUsuarioProduccion NVARCHAR(450) NOT NULL,
  paso INT NOT NULL,
  merma_litros FLOAT NULL,
  CONSTRAINT fk_producciones_receta FOREIGN KEY (idReceta)
    REFERENCES cerverica.receta (id)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT fk_producciones_usuarios1 FOREIGN KEY (idUsuarioSolicitud)
    REFERENCES dbo.AspNetUsers (Id)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT fk_producciones_usuarios2 FOREIGN KEY (idUsuarioProduccion)
    REFERENCES dbo.AspNetUsers (Id)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION
);
GO

-- -----------------------------------------------------
-- Table 7 cerverica.stock
-- -----------------------------------------------------
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'cerverica' AND TABLE_NAME = 'stock')
BEGIN
    DROP TABLE cerverica.stock;
END;
GO

CREATE TABLE cerverica.stock (
  id INT NOT NULL PRIMARY KEY IDENTITY,
  fecha_entrada DATE NULL,
  cantidad INT NOT NULL,
  merma INT NULL,
  idProduccion INT NOT NULL,
  idReceta INT NOT NULL,
  idUsuario NVARCHAR(450) NOT NULL,
  CONSTRAINT fk_stock_producciones FOREIGN KEY (idProduccion)
    REFERENCES cerverica.producciones (id)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT fk_stock_receta FOREIGN KEY (idReceta)
    REFERENCES cerverica.receta (id)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT fk_stock_usuarios FOREIGN KEY (idUsuario)
    REFERENCES dbo.AspNetUsers (Id)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION
);
GO

-- -----------------------------------------------------
-- Table 8 cerverica.ventas
-- -----------------------------------------------------
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'cerverica' AND TABLE_NAME = 'ventas')
BEGIN
    DROP TABLE cerverica.ventas;
END;
GO

CREATE TABLE cerverica.ventas (
  id INT NOT NULL PRIMARY KEY IDENTITY,
  idUsuario NVARCHAR(450) NOT NULL,
  fecha_venta DATE NOT NULL,
  total FLOAT NOT NULL,
  tipo_venta INT NOT NULL,
  CONSTRAINT fk_ventas_usuarios FOREIGN KEY (idUsuario)
    REFERENCES dbo.AspNetUsers (Id)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION
);
GO

-- -----------------------------------------------------
-- Table 9 cerverica.detalle_venta
-- -----------------------------------------------------
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'cerverica' AND TABLE_NAME = 'detalle_venta')
BEGIN
    DROP TABLE cerverica.detalle_venta;
END;
GO
 
CREATE TABLE cerverica.detalle_venta (
  idVenta INT NOT NULL,
  idStock INT NOT NULL,
  precio FLOAT NOT NULL,
  cantidad INT NULL,
  PRIMARY KEY (idVenta, idStock),
  CONSTRAINT fk_detalle_venta_ventas FOREIGN KEY (idVenta)
    REFERENCES cerverica.ventas (id)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT fk_detalle_venta_stock FOREIGN KEY (idStock)
    REFERENCES cerverica.stock (id)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION
);
GO

-- -----------------------------------------------------
-- Table 10 cerverica.compras
-- -----------------------------------------------------
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'cerverica' AND TABLE_NAME = 'compras')
BEGIN
    DROP TABLE cerverica.compras;
END;
GO

CREATE TABLE cerverica.compras (
  idUsuario NVARCHAR(450) NOT NULL,
  lotes_insumos_id INT NOT NULL,
  lotes_insumos_idProveedor INT NOT NULL,
  lotes_insumos_idInsumo INT NOT NULL,
  pago_proveedor FLOAT NOT NULL,
  PRIMARY KEY (idUsuario, lotes_insumos_id, lotes_insumos_idProveedor, lotes_insumos_idInsumo),
  CONSTRAINT fk_compras_usuarios FOREIGN KEY (idUsuario)
    REFERENCES dbo.AspNetUsers (Id)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT fk_compras_lotes_insumos FOREIGN KEY (lotes_insumos_id)
    REFERENCES cerverica.lotes_insumos (id)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION
);
GO

-- -----------------------------------------------------
-- Table 11 cerverica.logs_login
-- -----------------------------------------------------
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'cerverica' AND TABLE_NAME = 'logs_login')
BEGIN
    DROP TABLE cerverica.logs_login;
END;
GO

CREATE TABLE cerverica.logs_login (
  id INT NOT NULL PRIMARY KEY IDENTITY,
  idUsuario NVARCHAR(450) NOT NULL,
  fecha DATETIME NOT NULL,
  CONSTRAINT fk_logs_login_usuarios FOREIGN KEY (idUsuario)
    REFERENCES dbo.AspNetUsers (Id)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION
);
GO

-- -----------------------------------------------------
-- Table 12 cerverica.clientes
-- -----------------------------------------------------
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'cerverica' AND TABLE_NAME = 'clientes')
BEGIN
    DROP TABLE cerverica.clientes;
END;
GO

CREATE TABLE cerverica.clientes (
  id INT NOT NULL PRIMARY KEY IDENTITY,
  nombre VARCHAR(45) NOT NULL,
  correo VARCHAR(45) NOT NULL,
  telefono VARCHAR(45) NOT NULL,
  direccion VARCHAR(45) NOT NULL
);
GO

-- -----------------------------------------------------
-- Table 13 cerverica.logs
-- -----------------------------------------------------
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'cerverica' AND TABLE_NAME = 'logs')
BEGIN
    DROP TABLE cerverica.logs;
END;
GO

CREATE TABLE cerverica.logs (
  id INT NOT NULL PRIMARY KEY IDENTITY,
  idUsuario NVARCHAR(450) NOT NULL,
  fecha DATETIME NOT NULL,
  mensaje VARCHAR(255) NOT NULL,
  CONSTRAINT fk_logs_usuarios FOREIGN KEY (idUsuario)
    REFERENCES dbo.AspNetUsers (Id)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION
);
GO


#TEST
