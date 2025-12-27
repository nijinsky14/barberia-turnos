using Microsoft.AspNetCore.Mvc;
using BarberiaWeb.DTOs;
using BarberiaWeb.Services;

namespace BarberiaWeb.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TurnosController : ControllerBase
    {
        private readonly ITurnoService _turnoService;
        private readonly IEmailService _emailService;
        private readonly ILogger<TurnosController> _logger;

        public TurnosController(ITurnoService turnoService, IEmailService emailService, ILogger<TurnosController> logger)
        {
            _turnoService = turnoService;
            _emailService = emailService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<ActionResult<TurnoDto>> CrearTurno([FromBody] CrearTurnoDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var turno = await _turnoService.CrearTurno(dto);

            if (turno == null)
                return BadRequest(new { mensaje = "El horario seleccionado no está disponible" });

            // Enviar email de confirmación si el cliente proporcionó un email
            if (!string.IsNullOrEmpty(dto.Email))
            {
                try
                {
                    var emailEnviado = await _emailService.EnviarEmailConfirmacionReserva(
                        dto.Email,
                        dto.Nombre,
                        turno.NombreServicio,
                        turno.FechaTurno,
                        turno.HoraTurno,
                        turno.PrecioServicio
                    );

                    if (emailEnviado)
                    {
                        _logger.LogInformation($"Email de confirmación enviado a {dto.Email} para el turno #{turno.Id}");
                    }
                    else
                    {
                        _logger.LogWarning($"No se pudo enviar el email de confirmación a {dto.Email} para el turno #{turno.Id}");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error al enviar email de confirmación: {ex.Message}");
                    // No fallar la creación del turno si falla el envío del email
                }
            }

            return CreatedAtAction(nameof(CrearTurno), new { id = turno.Id }, turno);
        }

        [HttpGet]
        public async Task<ActionResult<List<TurnoDto>>> ObtenerTurnos(
            [FromQuery] DateTime? desde = null, 
            [FromQuery] DateTime? hasta = null)
        {
            var turnos = await _turnoService.ObtenerTodosLosTurnos(desde, hasta);
            return Ok(turnos);
        }

        [HttpGet("disponibilidad")]
        public async Task<ActionResult<DisponibilidadDto>> ObtenerDisponibilidad([FromQuery] DateTime fecha)
        {
            var disponibilidad = await _turnoService.ObtenerDisponibilidad(fecha);
            return Ok(disponibilidad);
        }

        [HttpGet("fecha/{fecha}")]
        public async Task<ActionResult<List<TurnoDto>>> ObtenerTurnosPorFecha(DateTime fecha)
        {
            var turnos = await _turnoService.ObtenerTurnosPorFecha(fecha);
            return Ok(turnos);
        }

        [HttpGet("estadisticas")]
        public async Task<ActionResult<EstadisticasTurnosDto>> ObtenerEstadisticas([FromQuery] DateTime? fecha = null)
        {
            var estadisticas = await _turnoService.ObtenerEstadisticas(fecha);
            return Ok(estadisticas);
        }

        [HttpPut("{id}/confirmar")]
        public async Task<ActionResult> ConfirmarTurno(int id)
        {
            var resultado = await _turnoService.ConfirmarTurno(id);

            if (!resultado)
                return NotFound(new { mensaje = "Turno no encontrado" });

            return Ok(new { mensaje = "Turno confirmado exitosamente" });
        }

        [HttpPut("{id}/completar")]
        public async Task<ActionResult> CompletarTurno(int id)
        {
            var resultado = await _turnoService.CompletarTurno(id);

            if (!resultado)
                return NotFound(new { mensaje = "Turno no encontrado" });

            return Ok(new { mensaje = "Turno completado exitosamente" });
        }

        [HttpPut("{id}/cancelar")]
        public async Task<ActionResult> CancelarTurno(int id, [FromBody] CancelarTurnoDto dto)
        {
            var resultado = await _turnoService.CancelarTurno(id, dto.Motivo);

            if (!resultado)
                return NotFound(new { mensaje = "Turno no encontrado" });

            return Ok(new { mensaje = "Turno cancelado exitosamente" });
        }

        [HttpGet("calendario")]
        public async Task<ActionResult> ObtenerEventosCalendario(
            [FromQuery] DateTime? desde = null,
            [FromQuery] DateTime? hasta = null)
        {
            var turnos = await _turnoService.ObtenerTodosLosTurnos(desde, hasta);

            var eventos = turnos.Select(t => new
            {
                id = t.Id,
                title = $"{t.NombreCliente} - {t.NombreServicio}",
                start = $"{t.FechaTurno:yyyy-MM-dd}T{t.HoraTurno}",
                backgroundColor = t.Estado switch
                {
                    "Completado" => "#28a745",
                    "Confirmado" => "#17a2b8",
                    "Pendiente" => "#ffc107",
                    "Cancelado" => "#dc3545",
                    _ => "#6c757d"
                },
                borderColor = t.Estado switch
                {
                    "Completado" => "#218838",
                    "Confirmado" => "#138496",
                    "Pendiente" => "#e0a800",
                    "Cancelado" => "#bd2130",
                    _ => "#5a6268"
                },
                extendedProps = new
                {
                    telefono = t.TelefonoCliente,
                    precio = t.PrecioServicio,
                    estado = t.Estado,
                    comentarios = t.Comentarios
                }
            }).ToList();

            return Ok(eventos);
        }
    }

    public class CancelarTurnoDto
    {
        public string Motivo { get; set; } = string.Empty;
    }
}
