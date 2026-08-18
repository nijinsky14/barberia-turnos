using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BarberiaWeb.Models
{
    public class NegocioGaleriaImagen
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int NegocioId { get; set; }
        [ForeignKey("NegocioId")]
        public virtual Negocio Negocio { get; set; } = null!;

        [Required]
        [StringLength(300)]
        public string Url { get; set; } = string.Empty;

        public int Orden { get; set; }
    }
}
