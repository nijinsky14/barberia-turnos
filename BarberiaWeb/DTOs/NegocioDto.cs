using System.ComponentModel.DataAnnotations;

namespace BarberiaWeb.DTOs
{
    public class HorarioDto
    {
        public string Dia { get; set; } = string.Empty; // nombre del DayOfWeek en inglés, ej. "Monday"
        public bool Abierto { get; set; }
        public string? Apertura { get; set; }
        public string? Cierre { get; set; }
    }

    public class NegocioDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Rubro { get; set; } = string.Empty;
        public string? LogoUrl { get; set; }
        public string? HeroImagenUrl { get; set; }

        public string ColorPrimario { get; set; } = string.Empty;
        public string ColorAcento { get; set; } = string.Empty;
        public string ColorTexto { get; set; } = string.Empty;
        public string ColorTextoClaro { get; set; } = string.Empty;
        public string ColorFondo { get; set; } = string.Empty;
        public string ColorFondoClaro { get; set; } = string.Empty;
        public string? ThemePresetId { get; set; }

        public string Direccion { get; set; } = string.Empty;
        public string WhatsApp { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? MapaEmbedUrl { get; set; }

        public string? DescripcionNosotros { get; set; }
        public List<string> OrdenSecciones { get; set; } = new();

        public List<HorarioDto> Horarios { get; set; } = new();
        public List<GaleriaImagenDto> Galeria { get; set; } = new();
    }

    public class GaleriaImagenDto
    {
        public int Id { get; set; }
        public string Url { get; set; } = string.Empty;
        public int Orden { get; set; }
    }

    public class NegocioUpdateDto
    {
        [Required, StringLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [StringLength(50)]
        public string Rubro { get; set; } = "Barberia";

        [StringLength(9)] public string ColorPrimario { get; set; } = "#1a1a1a";
        [StringLength(9)] public string ColorAcento { get; set; } = "#d4af37";
        [StringLength(9)] public string ColorTexto { get; set; } = "#333333";
        [StringLength(9)] public string ColorTextoClaro { get; set; } = "#666666";
        [StringLength(9)] public string ColorFondo { get; set; } = "#ffffff";
        [StringLength(9)] public string ColorFondoClaro { get; set; } = "#f8f8f8";
        [StringLength(50)] public string? ThemePresetId { get; set; }

        [StringLength(200)] public string Direccion { get; set; } = string.Empty;
        [StringLength(30)] public string WhatsApp { get; set; } = string.Empty;
        [StringLength(100)] public string Email { get; set; } = string.Empty;
        [StringLength(500)] public string? MapaEmbedUrl { get; set; }

        [StringLength(2000)] public string? DescripcionNosotros { get; set; }

        public List<string> OrdenSecciones { get; set; } = new() { "servicios", "nosotros", "contacto" };

        public List<HorarioDto> Horarios { get; set; } = new();
    }

    public class ReordenarGaleriaDto
    {
        public List<int> IdsEnOrden { get; set; } = new();
    }
}
