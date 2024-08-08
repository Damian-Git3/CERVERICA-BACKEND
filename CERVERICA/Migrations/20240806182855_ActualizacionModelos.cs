using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CERVERICA.Migrations
{
    /// <inheritdoc />
    public partial class ActualizacionModelos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Especificaciones",
                table: "Recetas",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaRegistrado",
                table: "Recetas",
                type: "Date",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<float>(
                name: "PrecioPaquete1",
                table: "Recetas",
                type: "real",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "PrecioPaquete12",
                table: "Recetas",
                type: "real",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "PrecioPaquete24",
                table: "Recetas",
                type: "real",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "PrecioPaquete6",
                table: "Recetas",
                type: "real",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RutaFondo",
                table: "Recetas",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "ProductosCarrito",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdUsuario = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    IdReceta = table.Column<int>(type: "int", nullable: false),
                    CantidadLote = table.Column<int>(type: "int", nullable: false),
                    Cantidad = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductosCarrito", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductosCarrito_AspNetUsers_IdUsuario",
                        column: x => x.IdUsuario,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductosCarrito_Recetas_IdReceta",
                        column: x => x.IdReceta,
                        principalTable: "Recetas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductosCarrito_IdReceta",
                table: "ProductosCarrito",
                column: "IdReceta");

            migrationBuilder.CreateIndex(
                name: "IX_ProductosCarrito_IdUsuario",
                table: "ProductosCarrito",
                column: "IdUsuario");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductosCarrito");

            migrationBuilder.DropColumn(
                name: "Especificaciones",
                table: "Recetas");

            migrationBuilder.DropColumn(
                name: "FechaRegistrado",
                table: "Recetas");

            migrationBuilder.DropColumn(
                name: "PrecioPaquete1",
                table: "Recetas");

            migrationBuilder.DropColumn(
                name: "PrecioPaquete12",
                table: "Recetas");

            migrationBuilder.DropColumn(
                name: "PrecioPaquete24",
                table: "Recetas");

            migrationBuilder.DropColumn(
                name: "PrecioPaquete6",
                table: "Recetas");

            migrationBuilder.DropColumn(
                name: "RutaFondo",
                table: "Recetas");
        }
    }
}
