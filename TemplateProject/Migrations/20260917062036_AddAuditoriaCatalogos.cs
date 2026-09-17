using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TemplateProject.Migrations
{
    /// <inheritdoc />
    public partial class AddAuditoriaCatalogos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "FechaCreacion",
                table: "UnidadesMedida",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETUTCDATE()");

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaModificacion",
                table: "UnidadesMedida",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UsuarioCreacionId",
                table: "UnidadesMedida",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaCreacion",
                table: "CategoriasIngredientes",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETUTCDATE()");

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaModificacion",
                table: "CategoriasIngredientes",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UsuarioCreacionId",
                table: "CategoriasIngredientes",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_UnidadesMedida_UsuarioCreacionId",
                table: "UnidadesMedida",
                column: "UsuarioCreacionId");

            migrationBuilder.CreateIndex(
                name: "IX_CategoriasIngredientes_UsuarioCreacionId",
                table: "CategoriasIngredientes",
                column: "UsuarioCreacionId");

            migrationBuilder.AddForeignKey(
                name: "FK_CategoriasIngredientes_AspNetUsers_UsuarioCreacionId",
                table: "CategoriasIngredientes",
                column: "UsuarioCreacionId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UnidadesMedida_AspNetUsers_UsuarioCreacionId",
                table: "UnidadesMedida",
                column: "UsuarioCreacionId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CategoriasIngredientes_AspNetUsers_UsuarioCreacionId",
                table: "CategoriasIngredientes");

            migrationBuilder.DropForeignKey(
                name: "FK_UnidadesMedida_AspNetUsers_UsuarioCreacionId",
                table: "UnidadesMedida");

            migrationBuilder.DropIndex(
                name: "IX_UnidadesMedida_UsuarioCreacionId",
                table: "UnidadesMedida");

            migrationBuilder.DropIndex(
                name: "IX_CategoriasIngredientes_UsuarioCreacionId",
                table: "CategoriasIngredientes");

            migrationBuilder.DropColumn(
                name: "FechaCreacion",
                table: "UnidadesMedida");

            migrationBuilder.DropColumn(
                name: "FechaModificacion",
                table: "UnidadesMedida");

            migrationBuilder.DropColumn(
                name: "UsuarioCreacionId",
                table: "UnidadesMedida");

            migrationBuilder.DropColumn(
                name: "FechaCreacion",
                table: "CategoriasIngredientes");

            migrationBuilder.DropColumn(
                name: "FechaModificacion",
                table: "CategoriasIngredientes");

            migrationBuilder.DropColumn(
                name: "UsuarioCreacionId",
                table: "CategoriasIngredientes");
        }
    }
}
