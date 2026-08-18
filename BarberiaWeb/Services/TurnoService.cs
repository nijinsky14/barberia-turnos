using Microsoft.EntityFrameworkCore;
using BarberiaWeb.Data;
using BarberiaWeb.DTOs;
using BarberiaWeb.Models;

namespace BarberiaWeb.Services
{
    public class TurnoService : ITurnoService
    {
        private readonly BarberiaDbContext _context;
        private readonly INegocioContextService _negocioContext;

        public TurnoService(BarberiaDbContext context, INegocioContextService negocioContext)
        {
            _context = context;
            _negocioContext = negocioContext;
        }

        public async Task<TurnoDto?> CrearTurno(CrearTurnoDto dto)
        {
            var negocioId = _negocioContext.NegocioActualId;

            // Verificar disponibilidad
            var disponibilidad = await ObtenerDisponibilidad(dto.FechaTurno, dto.ServicioId);
            if (!disponibilidad.HorariosDisponibles.Contains(dto.HoraTurno))
            {
                return null; // El horario está ocupado por un turno Confirmado o Completado
            }

            var cliente = await _context.Clientes
                .FirstOrDefaultAsync(c => c.NegocioId == negocioId && c.Telefono == dto.Telefono);

            if (cliente == null)
            {
                cliente = new Cliente
                {
                    NegocioId = negocioId,
                    Nombre = dto.Nombre,
                    Telefono = dto.Telefono,
                    Email = dto.Email
                };
                _context.Clientes.Add(cliente);
                await _context.SaveChangesAsync();
            }

            var turno = new Turno
            {
                NegocioId = negocioId,
                ClienteId = cliente.Id,
                ServicioId = dto.ServicioId,
                FechaTurno = dto.FechaTurno.Date,
                HoraTurno = dto.HoraTurno,
                Comentarios = dto.Comentarios,
                Estado = EstadoTurno.Pendiente
            };

            _context.Turnos.Add(turno);
            await _context.SaveChangesAsync();

            await _context.Entry(turno).Reference(t => t.Cliente).LoadAsync();
            await _context.Entry(turno).Reference(t => t.Servicio).LoadAsync();

            return MapearATurnoDto(turno);
        }

        public async Task<List<TurnoDto>> ObtenerTurnosPorFecha(DateTime fecha)
        {
            var negocioId = _negocioContext.NegocioActualId;
            var turnos = await _context.Turnos
                .Include(t => t.Cliente)
                .Include(t => t.Servicio)
                .Where(t => t.NegocioId == negocioId && t.FechaTurno.Date == fecha.Date && t.Estado != EstadoTurno.Cancelado)
                .OrderBy(t => t.HoraTurno)
                .ToListAsync();

            return turnos.Select(MapearATurnoDto).ToList();
        }

        public async Task<List<TurnoDto>> ObtenerTodosLosTurnos(DateTime? desde = null, DateTime? hasta = null)
        {
            var negocioId = _negocioContext.NegocioActualId;
            var query = _context.Turnos
                .Include(t => t.Cliente)
                .Include(t => t.Servicio)
                .Where(t => t.NegocioId == negocioId)
                .AsQueryable();

            if (desde.HasValue)
                query = query.Where(t => t.FechaTurno >= desde.Value.Date);

            if (hasta.HasValue)
                query = query.Where(t => t.FechaTurno <= hasta.Value.Date);

            var turnos = await query
                .OrderByDescending(t => t.FechaTurno)
                .ThenBy(t => t.HoraTurno)
                .ToListAsync();

            return turnos.Select(MapearATurnoDto).ToList();
        }

        public async Task<EstadisticasTurnosDto> ObtenerEstadisticas(DateTime? fecha = null)
        {
            var negocioId = _negocioContext.NegocioActualId;
            var query = _context.Turnos
                .Include(t => t.Servicio)
                .Where(t => t.NegocioId == negocioId)
                .AsQueryable();

            if (fecha.HasValue)
                query = query.Where(t => t.FechaTurno.Date == fecha.Value.Date);

            var turnos = await query.ToListAsync();

            return new EstadisticasTurnosDto
            {
                TotalTurnos = turnos.Count,
                TurnosPendientes = turnos.Count(t => t.Estado == EstadoTurno.Pendiente),
                TurnosConfirmados = turnos.Count(t => t.Estado == EstadoTurno.Confirmado),
                TurnosCancelados = turnos.Count(t => t.Estado == EstadoTurno.Cancelado),
                TurnosCompletados = turnos.Count(t => t.Estado == EstadoTurno.Completado),
                IngresosTotales = turnos
                    .Where(t => t.Estado == EstadoTurno.Completado)
                    .Sum(t => t.Servicio.Precio),
                IngresosEstimados = turnos
                    .Where(t => t.Estado == EstadoTurno.Pendiente || t.Estado == EstadoTurno.Confirmado)
                    .Sum(t => t.Servicio.Precio)
            };
        }

        public async Task<DisponibilidadDto> ObtenerDisponibilidad(DateTime fecha, int servicioId)
        {
            var negocioId = _negocioContext.NegocioActualId;

            var servicio = await _context.Servicios
                .FirstOrDefaultAsync(s => s.Id == servicioId && s.NegocioId == negocioId);

            if (servicio == null)
            {
                return new DisponibilidadDto { Fecha = fecha.Date, HorariosDisponibles = new List<string>() };
            }

            var horario = await _context.NegocioHorarios
                .FirstOrDefaultAsync(h => h.NegocioId == negocioId && h.DiaSemana == fecha.DayOfWeek);

            if (horario == null || !horario.Abierto || horario.HoraApertura == null || horario.HoraCierre == null)
            {
                return new DisponibilidadDto { Fecha = fecha.Date, HorariosDisponibles = new List<string>() };
            }

            var apertura = TimeSpan.Parse(horario.HoraApertura);
            var cierre = TimeSpan.Parse(horario.HoraCierre);

            // Turnos que ya ocupan un hueco ese día (solo Confirmado/Completado bloquean),
            // con la duración real de CADA turno (no la del servicio que se está consultando)
            var turnosDelDia = await _context.Turnos
                .Include(t => t.Servicio)
                .Where(t => t.NegocioId == negocioId && t.FechaTurno.Date == fecha.Date &&
                           (t.Estado == EstadoTurno.Confirmado || t.Estado == EstadoTurno.Completado))
                .ToListAsync();

            var ocupados = turnosDelDia
                .Select(t => (Inicio: TimeSpan.Parse(t.HoraTurno), DuracionMinutos: t.Servicio.DuracionMinutos))
                .ToList();

            var horariosDisponibles = GenerarSlotsDisponibles(apertura, cierre, servicio.DuracionMinutos, ocupados);

            return new DisponibilidadDto
            {
                Fecha = fecha.Date,
                HorariosDisponibles = horariosDisponibles
            };
        }

        // Genera los horarios de inicio posibles entre apertura y cierre, en incrementos
        // de la duración del servicio consultado, descartando los que se solapan con un
        // turno ya ocupado (considerando la duración real de ESE turno) o que no alcanzan
        // a terminar antes del cierre.
        private static List<string> GenerarSlotsDisponibles(TimeSpan apertura, TimeSpan cierre, int duracionMinutos, List<(TimeSpan Inicio, int DuracionMinutos)> ocupados)
        {
            var slots = new List<string>();
            var duracion = TimeSpan.FromMinutes(duracionMinutos);
            var actual = apertura;

            while (actual + duracion <= cierre)
            {
                var finActual = actual + duracion;

                var seSolapa = ocupados.Any(o =>
                {
                    var finOcupado = o.Inicio + TimeSpan.FromMinutes(o.DuracionMinutos);
                    return actual < finOcupado && o.Inicio < finActual;
                });

                if (!seSolapa)
                {
                    slots.Add(actual.ToString(@"hh\:mm"));
                }

                actual += duracion;
            }

            return slots;
        }

        public async Task<bool> CancelarTurno(int turnoId, string motivo)
        {
            var turno = await _context.Turnos.FindAsync(turnoId);
            if (turno == null) return false;

            turno.Estado = EstadoTurno.Cancelado;
            turno.FechaCancelacion = DateTime.Now;
            turno.MotivoCancelacion = motivo;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ConfirmarTurno(int turnoId)
        {
            var turno = await _context.Turnos.FindAsync(turnoId);
            if (turno == null) return false;

            turno.Estado = EstadoTurno.Confirmado;
            turno.FechaConfirmacion = DateTime.Now;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> CompletarTurno(int turnoId)
        {
            var turno = await _context.Turnos.FindAsync(turnoId);
            if (turno == null) return false;

            turno.Estado = EstadoTurno.Completado;

            await _context.SaveChangesAsync();
            return true;
        }

        private TurnoDto MapearATurnoDto(Turno turno)
        {
            return new TurnoDto
            {
                Id = turno.Id,
                NombreCliente = turno.Cliente.Nombre,
                TelefonoCliente = turno.Cliente.Telefono,
                EmailCliente = turno.Cliente.Email ?? string.Empty,
                NombreServicio = turno.Servicio.Nombre,
                PrecioServicio = turno.Servicio.Precio,
                FechaTurno = turno.FechaTurno,
                HoraTurno = turno.HoraTurno,
                Comentarios = turno.Comentarios,
                Estado = turno.Estado.ToString(),
                FechaCreacion = turno.FechaCreacion,
                FechaConfirmacion = turno.FechaConfirmacion,
                FechaCancelacion = turno.FechaCancelacion,
                MotivoCancelacion = turno.MotivoCancelacion
            };
        }
    }
}
