using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CERVERICA.Migrations
{
    /// <inheritdoc />
    public partial class _2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProduccionLoteInsumos_Recetas_RecetaId",
                table: "ProduccionLoteInsumos");

            migrationBuilder.DropIndex(
                name: "IX_ProduccionLoteInsumos_RecetaId",
                table: "ProduccionLoteInsumos");

            migrationBuilder.DropColumn(
                name: "RecetaId",
                table: "ProduccionLoteInsumos");

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaSolicitud",
                table: "Producciones",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RecetaId",
                table: "ProduccionLoteInsumos",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaSolicitud",
                table: "Producciones",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.CreateIndex(
                name: "IX_ProduccionLoteInsumos_RecetaId",
                table: "ProduccionLoteInsumos",
                column: "RecetaId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProduccionLoteInsumos_Recetas_RecetaId",
                table: "ProduccionLoteInsumos",
                column: "RecetaId",
                principalTable: "Recetas",
                principalColumn: "Id");
        }
    }
}
