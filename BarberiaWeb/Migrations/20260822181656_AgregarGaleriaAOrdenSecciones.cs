using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BarberiaWeb.Migrations
{
    /// <inheritdoc />
    public partial class AgregarGaleriaAOrdenSecciones : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Negocios",
                keyColumn: "Id",
                keyValue: 1,
                column: "OrdenSeccionesJson",
                value: "[\"servicios\",\"nosotros\",\"galeria\",\"contacto\"]");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Negocios",
                keyColumn: "Id",
                keyValue: 1,
                column: "OrdenSeccionesJson",
                value: "[\"servicios\",\"nosotros\",\"contacto\"]");
        }
    }
}
