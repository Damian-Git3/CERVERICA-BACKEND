﻿// <auto-generated />
using System;
using CERVERICA.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace CERVERICA.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20240806134022_3")]
    partial class _3
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.7")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("CERVERICA.Models.ApplicationUser", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("int");

                    b.Property<bool>("Activo")
                        .HasColumnType("bit");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Email")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("bit");

                    b.Property<string>("FullName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("bit");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("bit");

                    b.Property<string>("RefreshToken")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("RefreshTokenExpiryTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("bit");

                    b.Property<string>("UserName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasDatabaseName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasDatabaseName("UserNameIndex")
                        .HasFilter("[NormalizedUserName] IS NOT NULL");

                    b.ToTable("AspNetUsers", (string)null);
                });

            modelBuilder.Entity("CERVERICA.Models.DetalleVenta", b =>
                {
                    b.Property<int>("IdVenta")
                        .HasColumnType("int")
                        .HasColumnOrder(0);

                    b.Property<int>("IdStock")
                        .HasColumnType("int")
                        .HasColumnOrder(1);

                    b.Property<int?>("Cantidad")
                        .HasColumnType("int");

                    b.Property<float>("MontoVenta")
                        .HasColumnType("real");

                    b.HasKey("IdVenta", "IdStock");

                    b.HasIndex("IdStock");

                    b.ToTable("DetallesVenta");
                });

            modelBuilder.Entity("CERVERICA.Models.IngredienteReceta", b =>
                {
                    b.Property<int>("IdReceta")
                        .HasColumnType("int")
                        .HasColumnOrder(0);

                    b.Property<int>("IdInsumo")
                        .HasColumnType("int")
                        .HasColumnOrder(1);

                    b.Property<float>("Cantidad")
                        .HasColumnType("real");

                    b.HasKey("IdReceta", "IdInsumo");

                    b.HasIndex("IdInsumo");

                    b.ToTable("IngredientesReceta");
                });

            modelBuilder.Entity("CERVERICA.Models.Insumo", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<bool>("Activo")
                        .HasColumnType("bit");

                    b.Property<float>("CantidadMaxima")
                        .HasColumnType("real");

                    b.Property<float>("CantidadMinima")
                        .HasColumnType("real");

                    b.Property<float>("CostoUnitario")
                        .HasColumnType("real");

                    b.Property<string>("Descripcion")
                        .IsRequired()
                        .HasMaxLength(450)
                        .HasColumnType("nvarchar(450)");

                    b.Property<float>("Merma")
                        .HasColumnType("real");

                    b.Property<string>("Nombre")
                        .IsRequired()
                        .HasMaxLength(45)
                        .HasColumnType("nvarchar(45)");

                    b.Property<string>("UnidadMedida")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Insumos");
                });

            modelBuilder.Entity("CERVERICA.Models.Log", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Descripcion")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<DateTime>("Fecha")
                        .HasColumnType("datetime2");

                    b.Property<string>("IdUsuario")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UsuarioId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("UsuarioId");

                    b.ToTable("Logs");
                });

            modelBuilder.Entity("CERVERICA.Models.LogLogin", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<bool>("Exitoso")
                        .HasColumnType("bit");

                    b.Property<DateTime>("Fecha")
                        .HasColumnType("datetime2");

                    b.Property<string>("IdUsuario")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UsuarioId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("UsuarioId");

                    b.ToTable("LogsLogin");
                });

            modelBuilder.Entity("CERVERICA.Models.LoteInsumo", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<float?>("Caducado")
                        .IsRequired()
                        .HasColumnType("real");

                    b.Property<float>("Cantidad")
                        .HasColumnType("real");

                    b.Property<DateTime>("FechaCaducidad")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("FechaCompra")
                        .HasColumnType("datetime2");

                    b.Property<int>("IdInsumo")
                        .HasColumnType("int");

                    b.Property<int>("IdProveedor")
                        .HasColumnType("int");

                    b.Property<string>("IdUsuario")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<float>("Merma")
                        .HasColumnType("real");

                    b.Property<float>("MontoCompra")
                        .HasColumnType("real");

                    b.Property<float>("PrecioUnidad")
                        .HasColumnType("real");

                    b.HasKey("Id");

                    b.HasIndex("IdInsumo");

                    b.HasIndex("IdProveedor");

                    b.ToTable("LotesInsumos");
                });

            modelBuilder.Entity("CERVERICA.Models.PasosReceta", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Descripcion")
                        .IsRequired()
                        .HasMaxLength(2500)
                        .HasColumnType("nvarchar(2500)");

                    b.Property<int>("IdReceta")
                        .HasColumnType("int");

                    b.Property<int>("Orden")
                        .HasColumnType("int");

                    b.Property<int>("RecetaId")
                        .HasColumnType("int");

                    b.Property<int>("Tiempo")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("RecetaId");

                    b.ToTable("PasosRecetas");
                });

            modelBuilder.Entity("CERVERICA.Models.Produccion", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<float?>("CostoProduccion")
                        .HasColumnType("real");

                    b.Property<byte>("Estatus")
                        .HasColumnType("tinyint");

                    b.Property<DateTime>("FechaProduccion")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("FechaProximoPaso")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("FechaSolicitud")
                        .HasColumnType("datetime2");

                    b.Property<int>("IdReceta")
                        .HasColumnType("int");

                    b.Property<string>("IdUsuarioProduccion")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("IdUsuarioSolicitud")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<float?>("LitrosFinales")
                        .HasColumnType("real");

                    b.Property<string>("Mensaje")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<float?>("MermaLitros")
                        .HasColumnType("real");

                    b.Property<int>("NumeroTandas")
                        .HasColumnType("int");

                    b.Property<int>("Paso")
                        .HasColumnType("int");

                    b.Property<string>("UsuarioProduccionId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("UsuarioSolicitudId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("IdReceta");

                    b.HasIndex("UsuarioProduccionId");

                    b.HasIndex("UsuarioSolicitudId");

                    b.ToTable("Producciones");
                });

            modelBuilder.Entity("CERVERICA.Models.ProduccionLoteInsumo", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<float>("Cantidad")
                        .HasColumnType("real");

                    b.Property<int>("IdLoteInsumo")
                        .HasColumnType("int");

                    b.Property<int>("IdProduccion")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("IdLoteInsumo");

                    b.HasIndex("IdProduccion");

                    b.ToTable("ProduccionLoteInsumos");
                });

            modelBuilder.Entity("CERVERICA.Models.Proveedor", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<bool>("Activo")
                        .HasColumnType("bit");

                    b.Property<string>("Direccion")
                        .IsRequired()
                        .HasMaxLength(45)
                        .HasColumnType("nvarchar(45)");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("Empresa")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("NombreContacto")
                        .IsRequired()
                        .HasMaxLength(45)
                        .HasColumnType("nvarchar(45)");

                    b.Property<string>("Telefono")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Proveedores");
                });

            modelBuilder.Entity("CERVERICA.Models.Receta", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<bool>("Activo")
                        .HasColumnType("bit");

                    b.Property<float>("CostoProduccion")
                        .HasColumnType("real");

                    b.Property<string>("Descripcion")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Imagen")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<float>("LitrosEstimados")
                        .HasColumnType("real");

                    b.Property<string>("Nombre")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<float?>("PrecioLitro")
                        .HasColumnType("real");

                    b.HasKey("Id");

                    b.ToTable("Recetas");
                });

            modelBuilder.Entity("CERVERICA.Models.Stock", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("Cantidad")
                        .HasColumnType("int");

                    b.Property<DateTime?>("FechaEntrada")
                        .HasColumnType("datetime2");

                    b.Property<int>("IdProduccion")
                        .HasColumnType("int");

                    b.Property<int>("IdReceta")
                        .HasColumnType("int");

                    b.Property<string>("IdUsuario")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("MedidaEnvase")
                        .HasColumnType("int");

                    b.Property<int?>("Merma")
                        .HasColumnType("int");

                    b.Property<string>("TipoEnvase")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UsuarioId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("IdProduccion");

                    b.HasIndex("IdReceta");

                    b.HasIndex("UsuarioId");

                    b.ToTable("Stocks");
                });

            modelBuilder.Entity("CERVERICA.Models.Venta", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("FechaVenta")
                        .HasColumnType("datetime2");

                    b.Property<string>("IdUsuario")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("TipoVenta")
                        .HasColumnType("int");

                    b.Property<float>("Total")
                        .HasColumnType("real");

                    b.Property<string>("UsuarioId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("UsuarioId");

                    b.ToTable("Ventas");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasDatabaseName("RoleNameIndex")
                        .HasFilter("[NormalizedName] IS NOT NULL");

                    b.ToTable("AspNetRoles", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RoleId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ProviderKey")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ProviderDisplayName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("RoleId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("LoginProvider")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Value")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens", (string)null);
                });

            modelBuilder.Entity("CERVERICA.Models.DetalleVenta", b =>
                {
                    b.HasOne("CERVERICA.Models.Stock", "Stock")
                        .WithMany()
                        .HasForeignKey("IdStock")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("CERVERICA.Models.Venta", "Venta")
                        .WithMany()
                        .HasForeignKey("IdVenta")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Stock");

                    b.Navigation("Venta");
                });

            modelBuilder.Entity("CERVERICA.Models.IngredienteReceta", b =>
                {
                    b.HasOne("CERVERICA.Models.Insumo", "Insumo")
                        .WithMany("IngredientesReceta")
                        .HasForeignKey("IdInsumo")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("CERVERICA.Models.Receta", "Receta")
                        .WithMany("IngredientesReceta")
                        .HasForeignKey("IdReceta")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Insumo");

                    b.Navigation("Receta");
                });

            modelBuilder.Entity("CERVERICA.Models.Log", b =>
                {
                    b.HasOne("CERVERICA.Models.ApplicationUser", "Usuario")
                        .WithMany()
                        .HasForeignKey("UsuarioId");

                    b.Navigation("Usuario");
                });

            modelBuilder.Entity("CERVERICA.Models.LogLogin", b =>
                {
                    b.HasOne("CERVERICA.Models.ApplicationUser", "Usuario")
                        .WithMany()
                        .HasForeignKey("UsuarioId");

                    b.Navigation("Usuario");
                });

            modelBuilder.Entity("CERVERICA.Models.LoteInsumo", b =>
                {
                    b.HasOne("CERVERICA.Models.Insumo", "Insumo")
                        .WithMany("LotesInsumos")
                        .HasForeignKey("IdInsumo")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("CERVERICA.Models.Proveedor", "Proveedor")
                        .WithMany("LotesInsumos")
                        .HasForeignKey("IdProveedor")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Insumo");

                    b.Navigation("Proveedor");
                });

            modelBuilder.Entity("CERVERICA.Models.PasosReceta", b =>
                {
                    b.HasOne("CERVERICA.Models.Receta", "Receta")
                        .WithMany("PasosReceta")
                        .HasForeignKey("RecetaId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Receta");
                });

            modelBuilder.Entity("CERVERICA.Models.Produccion", b =>
                {
                    b.HasOne("CERVERICA.Models.Receta", "Receta")
                        .WithMany("Producciones")
                        .HasForeignKey("IdReceta")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("CERVERICA.Models.ApplicationUser", "UsuarioProduccion")
                        .WithMany()
                        .HasForeignKey("UsuarioProduccionId");

                    b.HasOne("CERVERICA.Models.ApplicationUser", "UsuarioSolicitud")
                        .WithMany()
                        .HasForeignKey("UsuarioSolicitudId");

                    b.Navigation("Receta");

                    b.Navigation("UsuarioProduccion");

                    b.Navigation("UsuarioSolicitud");
                });

            modelBuilder.Entity("CERVERICA.Models.ProduccionLoteInsumo", b =>
                {
                    b.HasOne("CERVERICA.Models.LoteInsumo", "LoteInsumo")
                        .WithMany("ProduccionLoteInsumos")
                        .HasForeignKey("IdLoteInsumo")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("CERVERICA.Models.Produccion", "Produccion")
                        .WithMany("ProduccionLoteInsumos")
                        .HasForeignKey("IdProduccion")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("LoteInsumo");

                    b.Navigation("Produccion");
                });

            modelBuilder.Entity("CERVERICA.Models.Stock", b =>
                {
                    b.HasOne("CERVERICA.Models.Produccion", "Produccion")
                        .WithMany()
                        .HasForeignKey("IdProduccion")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("CERVERICA.Models.Receta", "Receta")
                        .WithMany("Stocks")
                        .HasForeignKey("IdReceta")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("CERVERICA.Models.ApplicationUser", "Usuario")
                        .WithMany()
                        .HasForeignKey("UsuarioId");

                    b.Navigation("Produccion");

                    b.Navigation("Receta");

                    b.Navigation("Usuario");
                });

            modelBuilder.Entity("CERVERICA.Models.Venta", b =>
                {
                    b.HasOne("CERVERICA.Models.ApplicationUser", "Usuario")
                        .WithMany()
                        .HasForeignKey("UsuarioId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Usuario");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("CERVERICA.Models.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("CERVERICA.Models.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("CERVERICA.Models.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("CERVERICA.Models.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("CERVERICA.Models.Insumo", b =>
                {
                    b.Navigation("IngredientesReceta");

                    b.Navigation("LotesInsumos");
                });

            modelBuilder.Entity("CERVERICA.Models.LoteInsumo", b =>
                {
                    b.Navigation("ProduccionLoteInsumos");
                });

            modelBuilder.Entity("CERVERICA.Models.Produccion", b =>
                {
                    b.Navigation("ProduccionLoteInsumos");
                });

            modelBuilder.Entity("CERVERICA.Models.Proveedor", b =>
                {
                    b.Navigation("LotesInsumos");
                });

            modelBuilder.Entity("CERVERICA.Models.Receta", b =>
                {
                    b.Navigation("IngredientesReceta");

                    b.Navigation("PasosReceta");

                    b.Navigation("Producciones");

                    b.Navigation("Stocks");
                });
#pragma warning restore 612, 618
        }
    }
}
