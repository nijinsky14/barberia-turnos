using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BarberiaWeb.Models
{
    public class Turno
    {
        [Key]
        public int Id { get; set; }

        // Relaciones
        [Required]
        public int ClienteId { get; set; }
        [ForeignKey("ClienteId")]
        public virtual Cliente Cliente { get; set; } = null!;

        [Required]
        public int ServicioId { get; set; }
        [ForeignKey("ServicioId")]
        public virtual Servicio Servicio { get; set; } = null!;

        // Información del turno
        [Required]
        public DateTime FechaTurno { get; set; }

        [Required]
        [StringLength(5)]
        public string HoraTurno { get; set; } = string.Empty; // Formato: "09:00"

        [StringLength(500)]
        public string? Comentarios { get; set; }

        // Estado del turno
        public EstadoTurno Estado { get; set; } = EstadoTurno.Pendiente;

        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        public DateTime? FechaConfirmacion { get; set; }

        public DateTime? FechaCancelacion { get; set; }

        [StringLength(500)]
        public string? MotivoCancelacion { get; set; }
    }

    public enum EstadoTurno
    {
        Pendiente = 0,
        Confirmado = 1,
        Cancelado = 2,
        Completado = 3,
        NoAsistio = 4
    }
}
