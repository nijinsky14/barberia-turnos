using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BarberiaWeb.Migrations
{
    /// <inheritdoc />
    public partial class EliminarTelefonoNegocio : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Telefono",
                table: "Negocios");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Telefono",
                table: "Negocios",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "Negocios",
                keyColumn: "Id",
                keyValue: 1,
                column: "Telefono",
                value: "+54 341 690-1109");
        }
    }
}
