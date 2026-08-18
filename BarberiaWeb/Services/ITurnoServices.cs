using BarberiaWeb.DTOs;

namespace BarberiaWeb.Services
{
    public interface ITurnoService
    {
        Task<TurnoDto?> CrearTurno(CrearTurnoDto dto);  
        Task<List<TurnoDto>> ObtenerTurnosPorFecha(DateTime fecha);
        Task<List<TurnoDto>> ObtenerTodosLosTurnos(DateTime? desde = null, DateTime? hasta = null);
        Task<EstadisticasTurnosDto> ObtenerEstadisticas(DateTime? fecha = null);
        Task<DisponibilidadDto> ObtenerDisponibilidad(DateTime fecha, int servicioId);
        Task<bool> CancelarTurno(int turnoId, string motivo);
        Task<bool> ConfirmarTurno(int turnoId);
        Task<bool> CompletarTurno(int turnoId);
    }
}
