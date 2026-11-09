using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EcommerceGym.Api.Migrations
{
    /// <inheritdoc />
    public partial class CorregirFkCarritoItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CarritoItems_Productos_ProductoIdProducto",
                table: "CarritoItems");

            migrationBuilder.DropForeignKey(
                name: "FK_CarritoItems_Usuarios_UsuarioIdUsuario",
                table: "CarritoItems");

            migrationBuilder.DropIndex(
                name: "IX_CarritoItems_ProductoIdProducto",
                table: "CarritoItems");

            migrationBuilder.DropIndex(
                name: "IX_CarritoItems_UsuarioIdUsuario",
                table: "CarritoItems");

            migrationBuilder.DropColumn(
                name: "ProductoIdProducto",
                table: "CarritoItems");

            migrationBuilder.DropColumn(
                name: "UsuarioIdUsuario",
                table: "CarritoItems");

            migrationBuilder.CreateIndex(
                name: "IX_CarritoItems_IdProducto",
                table: "CarritoItems",
                column: "IdProducto");

            migrationBuilder.AddForeignKey(
                name: "FK_CarritoItems_Productos_IdProducto",
                table: "CarritoItems",
                column: "IdProducto",
                principalTable: "Productos",
                principalColumn: "IdProducto",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CarritoItems_Usuarios_IdUsuario",
                table: "CarritoItems",
                column: "IdUsuario",
                principalTable: "Usuarios",
                principalColumn: "IdUsuario",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CarritoItems_Productos_IdProducto",
                table: "CarritoItems");

            migrationBuilder.DropForeignKey(
                name: "FK_CarritoItems_Usuarios_IdUsuario",
                table: "CarritoItems");

            migrationBuilder.DropIndex(
                name: "IX_CarritoItems_IdProducto",
                table: "CarritoItems");

            migrationBuilder.AddColumn<int>(
                name: "ProductoIdProducto",
                table: "CarritoItems",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UsuarioIdUsuario",
                table: "CarritoItems",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_CarritoItems_ProductoIdProducto",
                table: "CarritoItems",
                column: "ProductoIdProducto");

            migrationBuilder.CreateIndex(
                name: "IX_CarritoItems_UsuarioIdUsuario",
                table: "CarritoItems",
                column: "UsuarioIdUsuario");

            migrationBuilder.AddForeignKey(
                name: "FK_CarritoItems_Productos_ProductoIdProducto",
                table: "CarritoItems",
                column: "ProductoIdProducto",
                principalTable: "Productos",
                principalColumn: "IdProducto",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CarritoItems_Usuarios_UsuarioIdUsuario",
                table: "CarritoItems",
                column: "UsuarioIdUsuario",
                principalTable: "Usuarios",
                principalColumn: "IdUsuario",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
