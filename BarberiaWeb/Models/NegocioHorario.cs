using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BarberiaWeb.Models
{
    public class NegocioHorario
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int NegocioId { get; set; }
        [ForeignKey("NegocioId")]
        public virtual Negocio Negocio { get; set; } = null!;

        [Required]
        public DayOfWeek DiaSemana { get; set; }

        public bool Abierto { get; set; }

        [StringLength(5)]
        public string? HoraApertura { get; set; } // Formato: "09:00"

        [StringLength(5)]
        public string? HoraCierre { get; set; } // Formato: "20:00"
    }
}
