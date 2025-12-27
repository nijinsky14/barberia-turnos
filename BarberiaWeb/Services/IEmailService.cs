namespace BarberiaWeb.Services
{
    public interface IEmailService
    {
        Task<bool> EnviarEmailConfirmacionReserva(string emailDestino, string nombreCliente, string servicio, DateTime fechaTurno, string horaTurno, decimal precio);
    }
}
