using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BarberiaWeb.Migrations
{
    /// <inheritdoc />
    public partial class AgregarNegocioYWhiteLabel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "NegocioId",
                table: "Turnos",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<int>(
                name: "NegocioId",
                table: "Servicios",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<int>(
                name: "NegocioId",
                table: "Clientes",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.CreateTable(
                name: "Negocios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Rubro = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    LogoUrl = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    HeroImagenUrl = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    ColorPrimario = table.Column<string>(type: "nvarchar(9)", maxLength: 9, nullable: false),
                    ColorAcento = table.Column<string>(type: "nvarchar(9)", maxLength: 9, nullable: false),
                    ColorTexto = table.Column<string>(type: "nvarchar(9)", maxLength: 9, nullable: false),
                    ColorTextoClaro = table.Column<string>(type: "nvarchar(9)", maxLength: 9, nullable: false),
                    ColorFondo = table.Column<string>(type: "nvarchar(9)", maxLength: 9, nullable: false),
                    ColorFondoClaro = table.Column<string>(type: "nvarchar(9)", maxLength: 9, nullable: false),
                    ThemePresetId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Direccion = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Telefono = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    WhatsApp = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    MapaEmbedUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    DescripcionNosotros = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    OrdenSeccionesJson = table.Column<string>(type: "nvarchar(500)", nullable: false),
                    FechaActualizacion = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Negocios", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NegocioGaleriaImagenes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NegocioId = table.Column<int>(type: "int", nullable: false),
                    Url = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    Orden = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NegocioGaleriaImagenes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NegocioGaleriaImagenes_Negocios_NegocioId",
                        column: x => x.NegocioId,
                        principalTable: "Negocios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NegocioHorarios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NegocioId = table.Column<int>(type: "int", nullable: false),
                    DiaSemana = table.Column<int>(type: "int", nullable: false),
                    Abierto = table.Column<bool>(type: "bit", nullable: false),
                    HoraApertura = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: true),
                    HoraCierre = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NegocioHorarios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NegocioHorarios_Negocios_NegocioId",
                        column: x => x.NegocioId,
                        principalTable: "Negocios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NegocioId = table.Column<int>(type: "int", nullable: false),
                    NombreUsuario = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UltimoLogin = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Usuarios_Negocios_NegocioId",
                        column: x => x.NegocioId,
                        principalTable: "Negocios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Negocios",
                columns: new[] { "Id", "ColorAcento", "ColorFondo", "ColorFondoClaro", "ColorPrimario", "ColorTexto", "ColorTextoClaro", "DescripcionNosotros", "Direccion", "Email", "FechaActualizacion", "HeroImagenUrl", "LogoUrl", "MapaEmbedUrl", "Nombre", "OrdenSeccionesJson", "Rubro", "Telefono", "ThemePresetId", "WhatsApp" },
                values: new object[] { 1, "#d4af37", "#ffffff", "#f8f8f8", "#1a1a1a", "#333333", "#666666", "En LUCK BARBER, creemos que cada corte es una obra de arte. Desde 2018, nos dedicamos a ofrecer más que un servicio de barbería: creamos experiencias únicas donde el estilo y la personalidad de cada cliente se encuentran.", "Av. Pellegrini 1234, Rosario, Santa Fe", "luckbarber@gmail.com", new DateTime(2025, 12, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, "LUCK BARBER", "[\"servicios\",\"nosotros\",\"contacto\"]", "Barberia", "+54 341 690-1109", null, "5493416901109" });

            migrationBuilder.UpdateData(
                table: "Servicios",
                keyColumn: "Id",
                keyValue: 1,
                column: "NegocioId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Servicios",
                keyColumn: "Id",
                keyValue: 2,
                column: "NegocioId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Servicios",
                keyColumn: "Id",
                keyValue: 3,
                column: "NegocioId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Servicios",
                keyColumn: "Id",
                keyValue: 4,
                column: "NegocioId",
                value: 1);

            migrationBuilder.InsertData(
                table: "NegocioHorarios",
                columns: new[] { "Id", "Abierto", "DiaSemana", "HoraApertura", "HoraCierre", "NegocioId" },
                values: new object[,]
                {
                    { 1, true, 1, "09:00", "20:00", 1 },
                    { 2, true, 2, "09:00", "20:00", 1 },
                    { 3, true, 3, "09:00", "20:00", 1 },
                    { 4, true, 4, "09:00", "20:00", 1 },
                    { 5, true, 5, "09:00", "20:00", 1 },
                    { 6, true, 6, "09:00", "18:00", 1 },
                    { 7, false, 0, null, null, 1 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Turnos_NegocioId",
                table: "Turnos",
                column: "NegocioId");

            migrationBuilder.CreateIndex(
                name: "IX_Servicios_NegocioId",
                table: "Servicios",
                column: "NegocioId");

            migrationBuilder.CreateIndex(
                name: "IX_Clientes_NegocioId",
                table: "Clientes",
                column: "NegocioId");

            migrationBuilder.CreateIndex(
                name: "IX_NegocioGaleriaImagenes_NegocioId",
                table: "NegocioGaleriaImagenes",
                column: "NegocioId");

            migrationBuilder.CreateIndex(
                name: "IX_NegocioHorarios_NegocioId",
                table: "NegocioHorarios",
                column: "NegocioId");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_NegocioId",
                table: "Usuarios",
                column: "NegocioId");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_NombreUsuario",
                table: "Usuarios",
                column: "NombreUsuario",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Clientes_Negocios_NegocioId",
                table: "Clientes",
                column: "NegocioId",
                principalTable: "Negocios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Servicios_Negocios_NegocioId",
                table: "Servicios",
                column: "NegocioId",
                principalTable: "Negocios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Turnos_Negocios_NegocioId",
                table: "Turnos",
                column: "NegocioId",
                principalTable: "Negocios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Clientes_Negocios_NegocioId",
                table: "Clientes");

            migrationBuilder.DropForeignKey(
                name: "FK_Servicios_Negocios_NegocioId",
                table: "Servicios");

            migrationBuilder.DropForeignKey(
                name: "FK_Turnos_Negocios_NegocioId",
                table: "Turnos");

            migrationBuilder.DropTable(
                name: "NegocioGaleriaImagenes");

            migrationBuilder.DropTable(
                name: "NegocioHorarios");

            migrationBuilder.DropTable(
                name: "Usuarios");

            migrationBuilder.DropTable(
                name: "Negocios");

            migrationBuilder.DropIndex(
                name: "IX_Turnos_NegocioId",
                table: "Turnos");

            migrationBuilder.DropIndex(
                name: "IX_Servicios_NegocioId",
                table: "Servicios");

            migrationBuilder.DropIndex(
                name: "IX_Clientes_NegocioId",
                table: "Clientes");

            migrationBuilder.DropColumn(
                name: "NegocioId",
                table: "Turnos");

            migrationBuilder.DropColumn(
                name: "NegocioId",
                table: "Servicios");

            migrationBuilder.DropColumn(
                name: "NegocioId",
                table: "Clientes");
        }
    }
}
