using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BarberiaWeb.Models
{
    public class Negocio
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [StringLength(50)]
        public string Rubro { get; set; } = "Barberia";

        [StringLength(300)]
        public string? LogoUrl { get; set; }

        [StringLength(300)]
        public string? HeroImagenUrl { get; set; }

        // Tema
        [StringLength(9)]
        public string ColorPrimario { get; set; } = "#1a1a1a";

        [StringLength(9)]
        public string ColorAcento { get; set; } = "#d4af37";

        [StringLength(9)]
        public string ColorTexto { get; set; } = "#333333";

        [StringLength(9)]
        public string ColorTextoClaro { get; set; } = "#666666";

        [StringLength(9)]
        public string ColorFondo { get; set; } = "#ffffff";

        [StringLength(9)]
        public string ColorFondoClaro { get; set; } = "#f8f8f8";

        [StringLength(50)]
        public string? ThemePresetId { get; set; }

        // Contacto
        [StringLength(200)]
        public string Direccion { get; set; } = string.Empty;

        [StringLength(30)]
        public string Telefono { get; set; } = string.Empty;

        [StringLength(30)]
        public string WhatsApp { get; set; } = string.Empty;

        [StringLength(100)]
        public string Email { get; set; } = string.Empty;

        [StringLength(500)]
        public string? MapaEmbedUrl { get; set; }

        // Contenido
        [StringLength(2000)]
        public string? DescripcionNosotros { get; set; }

        [Column(TypeName = "nvarchar(500)")]
        public string OrdenSeccionesJson { get; set; } = "[\"servicios\",\"nosotros\",\"galeria\",\"reservar\",\"contacto\"]";

        public DateTime FechaActualizacion { get; set; } = DateTime.Now;

        public virtual ICollection<NegocioHorario> Horarios { get; set; } = new List<NegocioHorario>();
        public virtual ICollection<NegocioGaleriaImagen> Galeria { get; set; } = new List<NegocioGaleriaImagen>();
    }
}
