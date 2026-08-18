using System.ComponentModel.DataAnnotations;

namespace BarberiaWeb.DTOs
{
    public class ServicioAdminDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public decimal Precio { get; set; }
        public int DuracionMinutos { get; set; }
        public bool Activo { get; set; }
        public bool TieneTurnos { get; set; }
    }

    public class CrearServicioDto
    {
        [Required(ErrorMessage = "El nombre es requerido")]
        [StringLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Descripcion { get; set; }

        [Range(0, 9999999, ErrorMessage = "El precio debe ser mayor o igual a 0")]
        public decimal Precio { get; set; }

        [Range(5, 480, ErrorMessage = "La duración debe estar entre 5 y 480 minutos")]
        public int DuracionMinutos { get; set; } = 30;
    }

    public class ActualizarServicioDto : CrearServicioDto
    {
        public bool Activo { get; set; } = true;
    }
}
