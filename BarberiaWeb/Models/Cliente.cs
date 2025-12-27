using System.ComponentModel.DataAnnotations;

namespace BarberiaWeb.Models
{
    public class Cliente
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string Telefono { get; set; } = string.Empty;

        [EmailAddress]
        [StringLength(100)]
        public string? Email { get; set; }

        public DateTime FechaRegistro { get; set; } = DateTime.Now;

        // Relación con Turnos
        public virtual ICollection<Turno> Turnos { get; set; } = new List<Turno>();
    }
}
