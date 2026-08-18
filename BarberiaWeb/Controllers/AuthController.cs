using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BarberiaWeb.Data;
using BarberiaWeb.Models;
using BarberiaWeb.Services;

namespace BarberiaWeb.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly BarberiaDbContext _context;
        private readonly INegocioContextService _negocioContext;
        private readonly IPasswordHasher<Usuario> _passwordHasher;

        public AuthController(BarberiaDbContext context, INegocioContextService negocioContext, IPasswordHasher<Usuario> passwordHasher)
        {
            _context = context;
            _negocioContext = negocioContext;
            _passwordHasher = passwordHasher;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var negocioId = _negocioContext.NegocioActualId;
            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.NegocioId == negocioId && u.NombreUsuario == request.Usuario);

            if (usuario == null)
            {
                return Unauthorized(new LoginResponse { Exitoso = false, Mensaje = "Usuario o contraseña incorrectos" });
            }

            var resultado = _passwordHasher.VerifyHashedPassword(usuario, usuario.PasswordHash, request.Contrasena);
            if (resultado == PasswordVerificationResult.Failed)
            {
                return Unauthorized(new LoginResponse { Exitoso = false, Mensaje = "Usuario o contraseña incorrectos" });
            }

            if (resultado == PasswordVerificationResult.SuccessRehashNeeded)
            {
                usuario.PasswordHash = _passwordHasher.HashPassword(usuario, request.Contrasena);
            }

            usuario.UltimoLogin = DateTime.Now;
            await _context.SaveChangesAsync();

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new(ClaimTypes.Name, usuario.NombreUsuario),
                new("NegocioId", usuario.NegocioId.ToString())
            };
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8)
            });

            return Ok(new LoginResponse
            {
                Exitoso = true,
                Mensaje = "Login exitoso"
            });
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Ok(new { mensaje = "Sesión cerrada exitosamente" });
        }

        [Authorize]
        [HttpGet("verificar")]
        public IActionResult VerificarSesion()
        {
            return Ok(new { autenticado = true });
        }
    }
}
