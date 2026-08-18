using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BarberiaWeb.Data;
using BarberiaWeb.DTOs;
using BarberiaWeb.Models;
using BarberiaWeb.Services;

namespace BarberiaWeb.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ServiciosController : ControllerBase
    {
        private readonly BarberiaDbContext _context;
        private readonly INegocioContextService _negocioContext;

        public ServiciosController(BarberiaDbContext context, INegocioContextService negocioContext)
        {
            _context = context;
            _negocioContext = negocioContext;
        }

        [HttpGet]
        public async Task<ActionResult<List<ServicioDto>>> ObtenerServicios()
        {
            var negocioId = _negocioContext.NegocioActualId;
            var servicios = await _context.Servicios
                .Where(s => s.NegocioId == negocioId && s.Activo)
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
            var negocioId = _negocioContext.NegocioActualId;
            var servicio = await _context.Servicios.FindAsync(id);

            if (servicio == null || servicio.NegocioId != negocioId || !servicio.Activo)
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

        [Authorize]
        [HttpGet("admin")]
        public async Task<ActionResult<List<ServicioAdminDto>>> ObtenerServiciosAdmin()
        {
            var negocioId = _negocioContext.NegocioActualId;

            var servicios = await _context.Servicios
                .Where(s => s.NegocioId == negocioId)
                .Select(s => new ServicioAdminDto
                {
                    Id = s.Id,
                    Nombre = s.Nombre,
                    Descripcion = s.Descripcion,
                    Precio = s.Precio,
                    DuracionMinutos = s.DuracionMinutos,
                    Activo = s.Activo,
                    TieneTurnos = s.Turnos.Any()
                })
                .OrderBy(s => s.Nombre)
                .ToListAsync();

            return Ok(servicios);
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<ServicioAdminDto>> CrearServicio([FromBody] CrearServicioDto dto)
        {
            var negocioId = _negocioContext.NegocioActualId;

            var servicio = new Servicio
            {
                NegocioId = negocioId,
                Nombre = dto.Nombre,
                Descripcion = dto.Descripcion,
                Precio = dto.Precio,
                DuracionMinutos = dto.DuracionMinutos,
                Activo = true
            };

            _context.Servicios.Add(servicio);
            await _context.SaveChangesAsync();

            return Ok(new ServicioAdminDto
            {
                Id = servicio.Id,
                Nombre = servicio.Nombre,
                Descripcion = servicio.Descripcion,
                Precio = servicio.Precio,
                DuracionMinutos = servicio.DuracionMinutos,
                Activo = servicio.Activo,
                TieneTurnos = false
            });
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<ActionResult<ServicioAdminDto>> ActualizarServicio(int id, [FromBody] ActualizarServicioDto dto)
        {
            var negocioId = _negocioContext.NegocioActualId;
            var servicio = await _context.Servicios.FirstOrDefaultAsync(s => s.Id == id && s.NegocioId == negocioId);

            if (servicio == null) return NotFound();

            servicio.Nombre = dto.Nombre;
            servicio.Descripcion = dto.Descripcion;
            servicio.Precio = dto.Precio;
            servicio.DuracionMinutos = dto.DuracionMinutos;
            servicio.Activo = dto.Activo;

            await _context.SaveChangesAsync();

            var tieneTurnos = await _context.Turnos.AnyAsync(t => t.ServicioId == id);

            return Ok(new ServicioAdminDto
            {
                Id = servicio.Id,
                Nombre = servicio.Nombre,
                Descripcion = servicio.Descripcion,
                Precio = servicio.Precio,
                DuracionMinutos = servicio.DuracionMinutos,
                Activo = servicio.Activo,
                TieneTurnos = tieneTurnos
            });
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<ActionResult> EliminarServicio(int id)
        {
            var negocioId = _negocioContext.NegocioActualId;
            var servicio = await _context.Servicios.FirstOrDefaultAsync(s => s.Id == id && s.NegocioId == negocioId);

            if (servicio == null) return NotFound();

            var tieneTurnos = await _context.Turnos.AnyAsync(t => t.ServicioId == id);
            if (tieneTurnos)
            {
                return BadRequest(new { mensaje = "Este servicio tiene turnos asociados y no se puede eliminar. Podés desactivarlo para que no se siga ofreciendo." });
            }

            _context.Servicios.Remove(servicio);
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Servicio eliminado" });
        }
    }
}
