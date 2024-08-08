
-- -----------------------------------------------------
-- Schema cerverica
-- -----------------------------------------------------
DROP SCHEMA IF EXISTS `cerverica` ;

-- -----------------------------------------------------
-- Schema cerverica
-- -----------------------------------------------------
CREATE SCHEMA IF NOT EXISTS `cerverica` DEFAULT CHARACTER SET utf8 ;
USE `cerverica` ;

-- -----------------------------------------------------
-- Table `cerverica`.`usuarios`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `cerverica`.`usuarios` ;

CREATE TABLE IF NOT EXISTS `cerverica`.`usuarios` (
  `id` INT NOT NULL AUTO_INCREMENT,
  `nombre` VARCHAR(45) NOT NULL,
  `rol` TINYINT(10) NOT NULL,
  `correo` VARCHAR(45) NOT NULL,
  `contrasenia` VARCHAR(45) NOT NULL,
  `token` VARCHAR(45) NULL,
  PRIMARY KEY (`id`),
  UNIQUE INDEX `correo_UNIQUE` (`correo` ASC) VISIBLE);


-- -----------------------------------------------------
-- Table `cerverica`.`proveedores`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `cerverica`.`proveedores` ;

CREATE TABLE IF NOT EXISTS `cerverica`.`proveedores` (
  `id` INT NOT NULL,
  `empresa` VARCHAR(45) NOT NULL,
  `direccion` VARCHAR(45) NOT NULL,
  `nombre_proveedor` VARCHAR(45) NOT NULL,
  PRIMARY KEY (`id`));


-- -----------------------------------------------------
-- Table `cerverica`.`insumos`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `cerverica`.`insumos` ;

CREATE TABLE IF NOT EXISTS `cerverica`.`insumos` (
  `id` INT NOT NULL AUTO_INCREMENT,
  `nombre` VARCHAR(45) NOT NULL,
  `descripcion` VARCHAR(45) NOT NULL,
  `unidad_medida` FLOAT NOT NULL,
  `cantidad_maxima` FLOAT NOT NULL,
  `cantidad_minima` FLOAT NOT NULL,
  `merma` FLOAT NOT NULL,
  PRIMARY KEY (`id`));


-- -----------------------------------------------------
-- Table `cerverica`.`lotes_insumos`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `cerverica`.`lotes_insumos` ;

CREATE TABLE IF NOT EXISTS `cerverica`.`lotes_insumos` (
  `id` INT NOT NULL AUTO_INCREMENT,
  `idProveedor` INT NOT NULL,
  `idInsumo` INT NOT NULL,
  `fecha_caducidad` DATE NOT NULL,
  `cantidad` FLOAT UNSIGNED NOT NULL,
  `fecha_compra` DATE NOT NULL,
  `precio_unidad` FLOAT NOT NULL,
  PRIMARY KEY (`id`, `idProveedor`, `idInsumo`),
  INDEX `fk_Lotes insumos_proveedores_idx` (`idProveedor` ASC) VISIBLE,
  INDEX `fk_Lotes insumos_insumos1_idx` (`idInsumo` ASC) VISIBLE,
  CONSTRAINT `fk_Lotes insumos_proveedores`
    FOREIGN KEY (`idProveedor`)
    REFERENCES `cerverica`.`proveedores` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_Lotes insumos_insumos1`
    FOREIGN KEY (`idInsumo`)
    REFERENCES `cerverica`.`insumos` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION);


-- -----------------------------------------------------
-- Table `cerverica`.`receta`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `cerverica`.`receta` ;

CREATE TABLE IF NOT EXISTS `cerverica`.`receta` (
  `id` INT NOT NULL AUTO_INCREMENT,
  `litros_estimados` FLOAT NOT NULL,
  `utilidad` FLOAT NULL,
  `piezas_estimadas` INT NOT NULL,
  `descripción` VARCHAR(500) NULL,
  `nombre` VARCHAR(50) NOT NULL,
  PRIMARY KEY (`id`));


-- -----------------------------------------------------
-- Table `cerverica`.`ingredientes_receta`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `cerverica`.`ingredientes_receta` ;

CREATE TABLE IF NOT EXISTS `cerverica`.`ingredientes_receta` (
  `idReceta` INT NOT NULL,
  `idInsumo` INT NOT NULL,
  `cantidad` FLOAT NOT NULL,
  PRIMARY KEY (`idReceta`, `idInsumo`),
  INDEX `fk_ingredientes_galleta_insumos1_idx` (`idInsumo` ASC) VISIBLE,
  CONSTRAINT `fk_ingredientes_galleta_receta1`
    FOREIGN KEY (`idReceta`)
    REFERENCES `cerverica`.`receta` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_ingredientes_galleta_insumos1`
    FOREIGN KEY (`idInsumo`)
    REFERENCES `cerverica`.`insumos` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION);


-- -----------------------------------------------------
-- Table `cerverica`.`producciones`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `cerverica`.`producciones` ;

CREATE TABLE IF NOT EXISTS `cerverica`.`producciones` (
  `id` INT NOT NULL,
  `fecha_produccion` DATE NOT NULL,
  `mensaje` VARCHAR(50) NULL,
  `status` TINYINT(10) NOT NULL,
  `tandas` INT NOT NULL,
  `litros_finales` INT NULL,
  `idReceta` INT NOT NULL,
  `fecha_solicitud` DATE NULL,
  `idUsuarioSolictud` INT NOT NULL,
  `idUsuarioProduccion` INT NOT NULL,
  `paso` INT NOT NULL,
  `merma_litros` INT NULL,
  PRIMARY KEY (`id`, `idReceta`, `idUsuarioSolictud`, `idUsuarioProduccion`),
  INDEX `fk_producciones_receta1_idx` (`idReceta` ASC) VISIBLE,
  INDEX `fk_producciones_usuarios1_idx` (`idUsuarioSolictud` ASC) VISIBLE,
  INDEX `fk_producciones_usuarios2_idx` (`idUsuarioProduccion` ASC) VISIBLE,
  CONSTRAINT `fk_producciones_receta1`
    FOREIGN KEY (`idReceta`)
    REFERENCES `cerverica`.`receta` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_producciones_usuarios1`
    FOREIGN KEY (`idUsuarioSolictud`)
    REFERENCES `cerverica`.`usuarios` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_producciones_usuarios2`
    FOREIGN KEY (`idUsuarioProduccion`)
    REFERENCES `cerverica`.`usuarios` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION);


-- -----------------------------------------------------
-- Table `cerverica`.`stock`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `cerverica`.`stock` ;

CREATE TABLE IF NOT EXISTS `cerverica`.`stock` (
  `id` INT NOT NULL,
  `fecha_entrada` DATE NULL,
  `cantidad` INT NOT NULL,
  `tipo_envase` VARCHAR(45) NOT NULL,
  `medida_envase` INT NOT NULL,
  `merma` INT NULL,
  `idProduccion` INT NOT NULL,
  `idReceta` INT NOT NULL,
  `tipo_venta` INT NOT NULL,
  `idUsuarios` INT NOT NULL,
  PRIMARY KEY (`id`, `idProduccion`, `idReceta`, `idUsuarios`),
  INDEX `fk_stock_producciones1_idx` (`idProduccion` ASC, `idReceta` ASC) VISIBLE,
  INDEX `fk_stock_usuarios1_idx` (`idUsuarios` ASC) VISIBLE,
  CONSTRAINT `fk_stock_producciones1`
    FOREIGN KEY (`idProduccion` , `idReceta`)
    REFERENCES `cerverica`.`producciones` (`id` , `idReceta`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_stock_usuarios1`
    FOREIGN KEY (`idUsuarios`)
    REFERENCES `cerverica`.`usuarios` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION);


-- -----------------------------------------------------
-- Table `cerverica`.`cortes_caja`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `cerverica`.`cortes_caja` ;

CREATE TABLE IF NOT EXISTS `cerverica`.`cortes_caja` (
  `id` INT NOT NULL,
  `monto_final` FLOAT NULL,
  `monto_inicial` FLOAT NULL,
  `fecha_corte` DATE NULL,
  PRIMARY KEY (`id`));


-- -----------------------------------------------------
-- Table `cerverica`.`transacciones_caja`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `cerverica`.`transacciones_caja` ;

CREATE TABLE IF NOT EXISTS `cerverica`.`transacciones_caja` (
  `id` INT NOT NULL,
  `monto_egreso` FLOAT NULL,
  `monto_ingreso` FLOAT NULL,
  `fecha_transaccion` DATE NULL,
  `idCorteCaja` INT NOT NULL,
  PRIMARY KEY (`id`, `idCorteCaja`),
  INDEX `fk_transacciones_caja_cortes_caja1_idx` (`idCorteCaja` ASC) VISIBLE,
  CONSTRAINT `fk_transacciones_caja_cortes_caja1`
    FOREIGN KEY (`idCorteCaja`)
    REFERENCES `cerverica`.`cortes_caja` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION);


-- -----------------------------------------------------
-- Table `cerverica`.`ventas`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `cerverica`.`ventas` ;

CREATE TABLE IF NOT EXISTS `cerverica`.`ventas` (
  `id` INT NOT NULL AUTO_INCREMENT,
  `idUsuario` INT NOT NULL,
  `fecha_venta` DATE NOT NULL,
  `total` FLOAT UNSIGNED NOT NULL,
  `idTransaccionCaja` INT NOT NULL,
  PRIMARY KEY (`id`, `idUsuario`, `idTransaccionCaja`),
  INDEX `fk_ventas_usuarios1_idx` (`idUsuario` ASC) VISIBLE,
  INDEX `fk_ventas_transacciones_caja1_idx` (`idTransaccionCaja` ASC) VISIBLE,
  CONSTRAINT `fk_ventas_usuarios1`
    FOREIGN KEY (`idUsuario`)
    REFERENCES `cerverica`.`usuarios` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_ventas_transacciones_caja1`
    FOREIGN KEY (`idTransaccionCaja`)
    REFERENCES `cerverica`.`transacciones_caja` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION);


-- -----------------------------------------------------
-- Table `cerverica`.`detalle_venta`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `cerverica`.`detalle_venta` ;

CREATE TABLE IF NOT EXISTS `cerverica`.`detalle_venta` (
  `idVenta` INT NOT NULL,
  `idStock` INT NOT NULL,
  `precio` FLOAT NOT NULL,
  `cantidad` INT NULL,
  PRIMARY KEY (`idVenta`, `idStock`),
  INDEX `fk_ventas_has_stock_stock1_idx` (`idStock` ASC) VISIBLE,
  INDEX `fk_ventas_has_stock_ventas1_idx` (`idVenta` ASC) VISIBLE,
  CONSTRAINT `fk_ventas_has_stock_ventas1`
    FOREIGN KEY (`idVenta`)
    REFERENCES `cerverica`.`ventas` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_ventas_has_stock_stock1`
    FOREIGN KEY (`idStock`)
    REFERENCES `cerverica`.`stock` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION);


-- -----------------------------------------------------
-- Table `cerverica`.`compras`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `cerverica`.`compras` ;

CREATE TABLE IF NOT EXISTS `cerverica`.`compras` (
  `idUsuario` INT NOT NULL,
  `lotes_insumos_id` INT NOT NULL,
  `lotes_insumos_idProveedor` INT NOT NULL,
  `lotes_insumos_idInsumo` INT NOT NULL,
  `pago_proveedor` FLOAT NOT NULL,
  `idTransaccionCaja` INT NULL,
  PRIMARY KEY (`idUsuario`, `lotes_insumos_id`, `lotes_insumos_idProveedor`, `lotes_insumos_idInsumo`),
  INDEX `fk_compras_usuarios1_idx` (`idUsuario` ASC) VISIBLE,
  INDEX `fk_compras_lotes_insumos1_idx` (`lotes_insumos_id` ASC, `lotes_insumos_idProveedor` ASC, `lotes_insumos_idInsumo` ASC) VISIBLE,
  INDEX `fk_compras_transacciones_caja1_idx` (`idTransaccionCaja` ASC) VISIBLE,
  CONSTRAINT `fk_compras_usuarios1`
    FOREIGN KEY (`idUsuario`)
    REFERENCES `cerverica`.`usuarios` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_compras_lotes_insumos1`
    FOREIGN KEY (`lotes_insumos_id` , `lotes_insumos_idProveedor` , `lotes_insumos_idInsumo`)
    REFERENCES `cerverica`.`lotes_insumos` (`id` , `idProveedor` , `idInsumo`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_compras_transacciones_caja1`
    FOREIGN KEY (`idTransaccionCaja`)
    REFERENCES `cerverica`.`transacciones_caja` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION);


-- -----------------------------------------------------
-- Table `cerverica`.`logs_login`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `cerverica`.`logs_login` ;

CREATE TABLE IF NOT EXISTS `cerverica`.`logs_login` (
  `id` INT UNSIGNED NOT NULL,
  `idUsuario` INT NOT NULL,
  `fecha` DATETIME NOT NULL,
  `exito` TINYINT NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE INDEX `id_UNIQUE` (`id` ASC) VISIBLE,
  CONSTRAINT `fk_logs_login_usuarios1`
    FOREIGN KEY (`idUsuario`)
    REFERENCES `cerverica`.`usuarios` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION);


-- -----------------------------------------------------
-- Table `cerverica`.`logs_acciones`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `cerverica`.`logs_acciones` ;

CREATE TABLE IF NOT EXISTS `cerverica`.`logs_acciones` (
  `id` INT UNSIGNED NOT NULL,
  `fecha` DATETIME NOT NULL,
  `modulo` VARCHAR(45) NOT NULL,
  `detalles` VARCHAR(200) NOT NULL,
  `idUsuario` INT NOT NULL,
  PRIMARY KEY (`id`, `idUsuario`),
  UNIQUE INDEX `d_UNIQUE` (`id` ASC) VISIBLE,
  INDEX `fk_logs_acciones_usuarios1_idx` (`idUsuario` ASC) VISIBLE,
  CONSTRAINT `fk_logs_acciones_usuarios1`
    FOREIGN KEY (`idUsuario`)
    REFERENCES `cerverica`.`usuarios` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION);


-- -----------------------------------------------------
-- Table `cerverica`.`roles`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `cerverica`.`roles` ;

CREATE TABLE IF NOT EXISTS `cerverica`.`roles` (
  `id` INT UNSIGNED NOT NULL,
  `nombre` VARCHAR(45) NOT NULL,
  `descripcion` VARCHAR(45) NOT NULL,
  `permisos` JSON NULL,
  PRIMARY KEY (`id`),
  UNIQUE INDEX `idRol_UNIQUE` (`id` ASC) VISIBLE);


-- -----------------------------------------------------
-- Table `cerverica`.`detalle_rol_usuario`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `cerverica`.`detalle_rol_usuario` ;

CREATE TABLE IF NOT EXISTS `cerverica`.`detalle_rol_usuario` (
  `idRol` INT UNSIGNED NOT NULL,
  `idUsuario` INT NOT NULL,
  PRIMARY KEY (`idRol`, `idUsuario`),
  INDEX `fk_roles_has_usuarios_usuarios1_idx` (`idUsuario` ASC) VISIBLE,
  INDEX `fk_roles_has_usuarios_roles1_idx` (`idRol` ASC) VISIBLE,
  CONSTRAINT `fk_roles_has_usuarios_roles1`
    FOREIGN KEY (`idRol`)
    REFERENCES `cerverica`.`roles` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_roles_has_usuarios_usuarios1`
    FOREIGN KEY (`idUsuario`)
    REFERENCES `cerverica`.`usuarios` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION);


-- -----------------------------------------------------
-- Table `cerverica`.`pasos`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `cerverica`.`pasos` ;

CREATE TABLE IF NOT EXISTS `cerverica`.`pasos` (
  `id` INT NOT NULL AUTO_INCREMENT,
  `descripción` VARCHAR(2500) NOT NULL,
  `tiempo` INT NOT NULL,
  `receta_id` INT NOT NULL,
  `secuencia` INT NOT NULL,
  PRIMARY KEY (`id`, `receta_id`),
  UNIQUE INDEX `id_UNIQUE` (`id` ASC) VISIBLE,
  INDEX `fk_pasos_receta1_idx` (`receta_id` ASC) VISIBLE,
  CONSTRAINT `fk_pasos_receta1`
    FOREIGN KEY (`receta_id`)
    REFERENCES `cerverica`.`receta` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION);