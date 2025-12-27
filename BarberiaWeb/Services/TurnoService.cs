using Microsoft.EntityFrameworkCore;
using BarberiaWeb.Data;
using BarberiaWeb.DTOs;
using BarberiaWeb.Models;

namespace BarberiaWeb.Services
{
    public class TurnoService : ITurnoService
    {
        private readonly BarberiaDbContext _context;
        private readonly List<string> _horariosDisponibles = new()
        {
            "09:00", "10:00", "11:00", "12:00", "13:00", "14:00",
            "15:00", "16:00", "17:00", "18:00", "19:00"
        };

        public TurnoService(BarberiaDbContext context)
        {
            _context = context;
        }

        public async Task<TurnoDto?> CrearTurno(CrearTurnoDto dto)
        {
            // Verificar disponibilidad
            var disponibilidad = await ObtenerDisponibilidad(dto.FechaTurno);
            if (!disponibilidad.HorariosDisponibles.Contains(dto.HoraTurno))
            {
                return null; // El horario está ocupado por un turno Confirmado o Completado
            }

            var cliente = await _context.Clientes
                .FirstOrDefaultAsync(c => c.Telefono == dto.Telefono);

            if (cliente == null)
            {
                cliente = new Cliente
                {
                    Nombre = dto.Nombre,
                    Telefono = dto.Telefono,
                    Email = dto.Email
                };
                _context.Clientes.Add(cliente);
                await _context.SaveChangesAsync();
            }

            var turno = new Turno
            {
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
            var turnos = await _context.Turnos
                .Include(t => t.Cliente)
                .Include(t => t.Servicio)
                .Where(t => t.FechaTurno.Date == fecha.Date && t.Estado != EstadoTurno.Cancelado)
                .OrderBy(t => t.HoraTurno)
                .ToListAsync();

            return turnos.Select(MapearATurnoDto).ToList();
        }

        public async Task<List<TurnoDto>> ObtenerTodosLosTurnos(DateTime? desde = null, DateTime? hasta = null)
        {
            var query = _context.Turnos
                .Include(t => t.Cliente)
                .Include(t => t.Servicio)
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
            var query = _context.Turnos
                .Include(t => t.Servicio)
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

        public async Task<DisponibilidadDto> ObtenerDisponibilidad(DateTime fecha)
        {
            // Obtener horarios ocupados SOLO por turnos Confirmados o Completados
            var turnosOcupados = await _context.Turnos
                .Where(t => t.FechaTurno.Date == fecha.Date && 
                           (t.Estado == EstadoTurno.Confirmado || t.Estado == EstadoTurno.Completado))
                .Select(t => t.HoraTurno)
                .ToListAsync();

            var horariosDisponibles = _horariosDisponibles
                .Where(h => !turnosOcupados.Contains(h))
                .ToList();

            return new DisponibilidadDto
            {
                Fecha = fecha.Date,
                HorariosDisponibles = horariosDisponibles
            };
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
