using System.Net;
using System.Net.Mail;

namespace BarberiaWeb.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<bool> EnviarEmailConfirmacionReserva(string emailDestino, string nombreCliente, string servicio, DateTime fechaTurno, string horaTurno, decimal precio)
        {
            try
            {
                var fechaFormateada = fechaTurno.ToString("dddd, dd 'de' MMMM 'de' yyyy", new System.Globalization.CultureInfo("es-AR"));

                var asunto = "✂️ Confirmación de Reserva - LUCK BARBER";
                
                var cuerpo = $@"
                    <html>
                    <head>
                        <style>
                            body {{ font-family: Arial, sans-serif; color: #333; background-color: #f5f5f5; margin: 0; padding: 0; }}
                            .container {{ max-width: 600px; margin: 20px auto; background: white; border-radius: 10px; overflow: hidden; box-shadow: 0 4px 12px rgba(0,0,0,0.1); }}
                            .header {{ background: linear-gradient(135deg, #1a1a1a 0%, #2d2d2d 100%); color: white; padding: 30px; text-align: center; }}
                            .header h1 {{ margin: 0; font-size: 28px; color: #d4af37; }}
                            .content {{ padding: 30px; }}
                            .details {{ background: #f8f9fa; padding: 20px; border-radius: 8px; margin: 20px 0; border-left: 4px solid #d4af37; }}
                            .details p {{ margin: 10px 0; font-size: 16px; }}
                            .details strong {{ color: #1a1a1a; }}
                            .precio {{ font-size: 24px; color: #28a745; font-weight: bold; }}
                            .info {{ background: #fff3cd; padding: 15px; border-radius: 8px; margin: 20px 0; }}
                            .footer {{ background: #f8f9fa; padding: 20px; text-align: center; color: #666; font-size: 14px; }}
                            .btn {{ display: inline-block; background: #d4af37; color: white; padding: 12px 30px; text-decoration: none; border-radius: 5px; margin: 20px 0; }}
                        </style>
                    </head>
                    <body>
                        <div class='container'>
                            <div class='header'>
                                <h1>✂️ LUCK BARBER</h1>
                                <p style='margin: 10px 0 0 0; color: #d4af37;'>Tu barbería de confianza</p>
                            </div>
                            <div class='content'>
                                <h2 style='color: #1a1a1a;'>¡Hola {nombreCliente}! 🎉</h2>
                                <p style='font-size: 16px; line-height: 1.6;'>
                                    ¡Gracias por elegirnos! Tu turno ha sido <strong style='color: #28a745;'>reservado exitosamente</strong>.
                                </p>
                                
                                <div class='details'>
                                    <h3 style='color: #d4af37; margin-top: 0;'>📋 Detalles de tu reserva:</h3>
                                    <p><strong>👤 Cliente:</strong> {nombreCliente}</p>
                                    <p><strong>💇 Servicio:</strong> {servicio}</p>
                                    <p><strong>📅 Fecha:</strong> {fechaFormateada}</p>
                                    <p><strong>⏰ Hora:</strong> {horaTurno}</p>
                                    <p><strong>💰 Precio:</strong> <span class='precio'>${precio:N0}</span></p>
                                </div>

                                <div class='info'>
                                    <p style='margin: 5px 0;'><strong>📍 Dirección:</strong> Av. Pellegrini 1234, Rosario, Santa Fe</p>
                                    <p style='margin: 5px 0;'><strong>📞 Teléfono:</strong> +54 341 123-4567</p>
                                </div>

                                <p style='color: #dc3545; font-weight: bold; margin: 20px 0;'>
                                    ⚠️ Recordá llegar 5 minutos antes de tu turno.
                                </p>

                                <p style='font-size: 14px; color: #666; line-height: 1.6;'>
                                    Si necesitás cancelar o reprogramar tu turno, por favor contactanos con al menos 24 horas de anticipación.
                                </p>

                                <div style='text-align: center; margin: 30px 0;'>
                                    <a href='https://wa.me/5493416901109' class='btn' style='color: white;'>
                                        📱 Contactar por WhatsApp
                                    </a>
                                </div>
                            </div>
                            <div class='footer'>
                                <p style='margin: 5px 0;'>¡Te esperamos!</p>
                                <p style='margin: 5px 0;'>© 2025 LUCK BARBER - Todos los derechos reservados</p>
                                <p style='margin: 5px 0; font-size: 12px;'>
                                    Este es un correo automático, por favor no respondas a este mensaje.
                                </p>
                            </div>
                        </div>
                    </body>
                    </html>
                ";

                return await EnviarEmail(emailDestino, asunto, cuerpo);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al enviar email de confirmación de reserva: {ex.Message}");
                return false;
            }
        }

        private async Task<bool> EnviarEmail(string destinatario, string asunto, string cuerpoHtml)
        {
            try
            {
                var smtpHost = _configuration["Email:SmtpHost"];
                var smtpPort = int.Parse(_configuration["Email:SmtpPort"] ?? "587");
                var smtpUser = _configuration["Email:SmtpUser"];
                var smtpPass = _configuration["Email:SmtpPassword"];
                var fromEmail = _configuration["Email:FromEmail"];
                var fromName = _configuration["Email:FromName"] ?? "LUCK BARBER";

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
