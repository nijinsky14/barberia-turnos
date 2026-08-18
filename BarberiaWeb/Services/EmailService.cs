using System.Net;
using System.Net.Mail;
using BarberiaWeb.Data;
using Microsoft.EntityFrameworkCore;

namespace BarberiaWeb.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;
        private readonly BarberiaDbContext _context;
        private readonly INegocioContextService _negocioContext;
        private readonly IWebHostEnvironment _env;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger, BarberiaDbContext context, INegocioContextService negocioContext, IWebHostEnvironment env)
        {
            _configuration = configuration;
            _logger = logger;
            _context = context;
            _negocioContext = negocioContext;
            _env = env;
        }

        public async Task<bool> EnviarEmailConfirmacionReserva(string emailDestino, string nombreCliente, string servicio, DateTime fechaTurno, string horaTurno, decimal precio)
        {
            try
            {
                var negocio = await _context.Negocios
                    .FirstOrDefaultAsync(n => n.Id == _negocioContext.NegocioActualId);

                if (negocio == null)
                {
                    _logger.LogError("No se encontró la configuración del negocio para armar el email de confirmación.");
                    return false;
                }

                var fechaFormateada = fechaTurno.ToString("dddd, dd 'de' MMMM 'de' yyyy", new System.Globalization.CultureInfo("es-AR"));
                var asunto = $"Confirmación de Reserva - {negocio.Nombre}";

                var rutaPlantilla = Path.Combine(_env.ContentRootPath, "Templates", "EmailConfirmacionTurno.html");
                var cuerpo = await File.ReadAllTextAsync(rutaPlantilla);

                cuerpo = cuerpo
                    .Replace("{{NombreNegocio}}", negocio.Nombre)
                    .Replace("{{ColorPrimario}}", negocio.ColorPrimario)
                    .Replace("{{ColorAcento}}", negocio.ColorAcento)
                    .Replace("{{NombreCliente}}", nombreCliente)
                    .Replace("{{Servicio}}", servicio)
                    .Replace("{{Fecha}}", fechaFormateada)
                    .Replace("{{Hora}}", horaTurno)
                    .Replace("{{Precio}}", precio.ToString("N0"))
                    .Replace("{{Direccion}}", negocio.Direccion)
                    .Replace("{{WhatsApp}}", negocio.WhatsApp)
                    .Replace("{{Anio}}", DateTime.Now.Year.ToString());

                return await EnviarEmail(emailDestino, asunto, cuerpo, negocio.Nombre);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al enviar email de confirmación de reserva: {ex.Message}");
                return false;
            }
        }

        private async Task<bool> EnviarEmail(string destinatario, string asunto, string cuerpoHtml, string nombreNegocio)
        {
            try
            {
                var smtpHost = _configuration["Email:SmtpHost"];
                var smtpPort = int.Parse(_configuration["Email:SmtpPort"] ?? "587");
                var smtpUser = _configuration["Email:SmtpUser"];
                var smtpPass = _configuration["Email:SmtpPassword"];
                var fromEmail = _configuration["Email:FromEmail"];
                var fromName = _configuration["Email:FromName"] ?? nombreNegocio;

                if (string.IsNullOrEmpty(smtpHost) || string.IsNullOrEmpty(smtpUser) || string.IsNullOrEmpty(smtpPass))
                {
                    _logger.LogWarning("Configuración de email incompleta. Email no enviado.");
                    return false;
                }

                using var client = new SmtpClient(smtpHost, smtpPort)
                {
                    EnableSsl = true,
                    Credentials = new NetworkCredential(smtpUser, smtpPass)
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(fromEmail!, fromName),
                    Subject = asunto,
                    Body = cuerpoHtml,
                    IsBodyHtml = true
                };

                mailMessage.To.Add(destinatario);

                await client.SendMailAsync(mailMessage);
                _logger.LogInformation($"Email de confirmación enviado exitosamente a {destinatario}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al enviar email: {ex.Message}");
                return false;
            }
        }
    }
}
