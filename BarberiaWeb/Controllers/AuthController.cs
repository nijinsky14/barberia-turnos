using Microsoft.AspNetCore.Mvc;
using BarberiaWeb.Models;

namespace BarberiaWeb.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public AuthController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            // Obtener credenciales desde appsettings.json
            var adminUser = _configuration["Admin:Usuario"];
            var adminPass = _configuration["Admin:Contrasena"];

            if (request.Usuario == adminUser && request.Contrasena == adminPass)
            {
                // Crear cookie de sesión
                Response.Cookies.Append("admin_auth", "authenticated", new CookieOptions
                {
                    HttpOnly = true,
                    Secure = false, // Cambiar a true en producción con HTTPS
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTimeOffset.UtcNow.AddHours(8) // Sesión de 8 horas
                });

                return Ok(new LoginResponse
                {
                    Exitoso = true,
                    Mensaje = "Login exitoso"
                });
            }

            return Unauthorized(new LoginResponse
            {
                Exitoso = false,
                Mensaje = "Usuario o contraseña incorrectos"
            });
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("admin_auth");
            return Ok(new { mensaje = "Sesión cerrada exitosamente" });
        }

        [HttpGet("verificar")]
        public IActionResult VerificarSesion()
        {
            var authCookie = Request.Cookies["admin_auth"];

            if (authCookie == "authenticated")
            {
                return Ok(new { autenticado = true });
            }

            return Unauthorized(new { autenticado = false });
        }
    }
}
