using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CERVERICA.Migrations
{
    /// <inheritdoc />
    public partial class favoritosusuario : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FavoritosUsuarios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdUsuario = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    IdReceta = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FavoritosUsuarios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FavoritosUsuarios_AspNetUsers_IdUsuario",
                        column: x => x.IdUsuario,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FavoritosUsuarios_Recetas_IdReceta",
                        column: x => x.IdReceta,
                        principalTable: "Recetas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FavoritosUsuarios_IdReceta",
                table: "FavoritosUsuarios",
                column: "IdReceta");

            migrationBuilder.CreateIndex(
                name: "IX_FavoritosUsuarios_IdUsuario",
                table: "FavoritosUsuarios",
                column: "IdUsuario");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FavoritosUsuarios");
        }
    }
}
