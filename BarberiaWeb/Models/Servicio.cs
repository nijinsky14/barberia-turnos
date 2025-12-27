using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BarberiaWeb.Models
{
    public class Servicio
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Descripcion { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal Precio { get; set; }

        public int DuracionMinutos { get; set; } = 30;

        public bool Activo { get; set; } = true;

        // Relación con Turnos
        public virtual ICollection<Turno> Turnos { get; set; } = new List<Turno>();
    }
}
