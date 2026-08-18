namespace BarberiaWeb.Services
{
    public interface INegocioContextService
    {
        int NegocioActualId { get; }
    }

    public class NegocioContextService : INegocioContextService
    {
        public int NegocioActualId => 1;
    }
}
