using System.ComponentModel.DataAnnotations;

namespace BarberiaWeb.DTOs
{
    public class CrearTurnoDto
    {
        [Required(ErrorMessage = "El nombre es requerido")]
        [StringLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "El teléfono es requerido")]
        [Phone]
        [StringLength(20)]
        public string Telefono { get; set; } = string.Empty;

        [EmailAddress]
        public string? Email { get; set; }

        [Required(ErrorMessage = "El servicio es requerido")]
        public int ServicioId { get; set; }

        [Required(ErrorMessage = "La fecha es requerida")]
        public DateTime FechaTurno { get; set; }

        [Required(ErrorMessage = "La hora es requerida")]
        public string HoraTurno { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Comentarios { get; set; }
    }

    public class TurnoDto
    {
        public int Id { get; set; }
        public string NombreCliente { get; set; } = string.Empty;
        public string TelefonoCliente { get; set; } = string.Empty;
        public string EmailCliente { get; set; } = string.Empty;
        public string NombreServicio { get; set; } = string.Empty;
        public decimal PrecioServicio { get; set; }
        public DateTime FechaTurno { get; set; }
        public string HoraTurno { get; set; } = string.Empty;
        public string? Comentarios { get; set; }
        public string Estado { get; set; } = string.Empty;
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaConfirmacion { get; set; }
        public DateTime? FechaCancelacion { get; set; }
        public string? MotivoCancelacion { get; set; }
    }

    public class ServicioDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public decimal Precio { get; set; }
        public int DuracionMinutos { get; set; }
    }

    public class DisponibilidadDto
    {
        public DateTime Fecha { get; set; }
        public List<string> HorariosDisponibles { get; set; } = new();
    }

    public class EstadisticasTurnosDto
    {
        public int TotalTurnos { get; set; }
        public int TurnosPendientes { get; set; }
        public int TurnosConfirmados { get; set; }
        public int TurnosCancelados { get; set; }
        public int TurnosCompletados { get; set; }
        public decimal IngresosTotales { get; set; }
        public decimal IngresosEstimados { get; set; }
    }
}
