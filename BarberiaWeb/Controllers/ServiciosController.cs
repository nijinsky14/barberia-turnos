using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BarberiaWeb.Data;
using BarberiaWeb.DTOs;

namespace BarberiaWeb.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ServiciosController : ControllerBase
    {
        private readonly BarberiaDbContext _context;

        public ServiciosController(BarberiaDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<ServicioDto>>> ObtenerServicios()
        {
            var servicios = await _context.Servicios
                .Where(s => s.Activo)
                .Select(s => new ServicioDto
                {
                    Id = s.Id,
                    Nombre = s.Nombre,
                    Descripcion = s.Descripcion,
                    Precio = s.Precio,
                    DuracionMinutos = s.DuracionMinutos
                })
                .ToListAsync();

            return Ok(servicios);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ServicioDto>> ObtenerServicio(int id)
        {
            var servicio = await _context.Servicios.FindAsync(id);

            if (servicio == null || !servicio.Activo)
                return NotFound();

            return Ok(new ServicioDto
            {
                Id = servicio.Id,
                Nombre = servicio.Nombre,
                Descripcion = servicio.Descripcion,
                Precio = servicio.Precio,
                DuracionMinutos = servicio.DuracionMinutos
            });
        }
    }
}
