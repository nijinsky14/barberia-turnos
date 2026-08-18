using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BarberiaWeb.Migrations
{
    /// <inheritdoc />
    public partial class ActualizarDescripcionNosotrosSeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Negocios",
                keyColumn: "Id",
                keyValue: 1,
                column: "DescripcionNosotros",
                value: "En LUCK BARBER, creemos que cada corte es una obra de arte. Desde 2018, nos dedicamos a ofrecer más que un servicio de barbería: creamos experiencias únicas donde el estilo y la personalidad de cada cliente se encuentran.\n\nNuestro equipo de barberos profesionales está capacitado en las últimas tendencias y técnicas clásicas, garantizando resultados impecables en cada visita. Utilizamos productos premium y trabajamos con atención al detalle para que salgas luciendo exactamente como lo imaginás.\n\nUbicados en el corazón de Rosario, nos hemos convertido en el punto de encuentro para quienes buscan calidad, confianza y un ambiente relajado. Ya sea un corte clásico, un fade moderno o un arreglo de barba profesional, en LUCK BARBER encontrás tu mejor versión.");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Negocios",
                keyColumn: "Id",
                keyValue: 1,
                column: "DescripcionNosotros",
                value: "En LUCK BARBER, creemos que cada corte es una obra de arte. Desde 2018, nos dedicamos a ofrecer más que un servicio de barbería: creamos experiencias únicas donde el estilo y la personalidad de cada cliente se encuentran.");
        }
    }
}
