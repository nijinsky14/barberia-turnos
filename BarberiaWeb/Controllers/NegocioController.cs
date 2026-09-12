using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BarberiaWeb.Data;
using BarberiaWeb.DTOs;
using BarberiaWeb.Models;
using BarberiaWeb.Services;

namespace BarberiaWeb.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NegocioController : ControllerBase
    {
        private static readonly HashSet<string> ExtensionesPermitidas = new(StringComparer.OrdinalIgnoreCase)
        {
            ".jpg", ".jpeg", ".png", ".webp"
        };

        private const int MaxFotosGaleria = 12;

        private readonly BarberiaDbContext _context;
        private readonly INegocioContextService _negocioContext;
        private readonly IWebHostEnvironment _env;
        private readonly IConfiguration _configuration;

        public NegocioController(BarberiaDbContext context, INegocioContextService negocioContext, IWebHostEnvironment env, IConfiguration configuration)
        {
            _context = context;
            _negocioContext = negocioContext;
            _env = env;
            _configuration = configuration;
        }

        [HttpGet]
        public async Task<ActionResult<NegocioDto>> ObtenerNegocio()
        {
            var negocio = await ObtenerNegocioActualAsync();
            if (negocio == null) return NotFound();

            return Ok(MapearADto(negocio));
        }

        [Authorize]
        [HttpPut]
        public async Task<ActionResult<NegocioDto>> ActualizarNegocio([FromBody] NegocioUpdateDto dto)
        {
            var negocio = await ObtenerNegocioActualAsync();
            if (negocio == null) return NotFound();

            negocio.Nombre = dto.Nombre;
            negocio.Rubro = dto.Rubro;
            negocio.ColorPrimario = dto.ColorPrimario;
            negocio.ColorAcento = dto.ColorAcento;
            negocio.ColorTexto = dto.ColorTexto;
            negocio.ColorTextoClaro = dto.ColorTextoClaro;
            negocio.ColorFondo = dto.ColorFondo;
            negocio.ColorFondoClaro = dto.ColorFondoClaro;
            negocio.ThemePresetId = dto.ThemePresetId;
            negocio.Direccion = dto.Direccion;
            negocio.Telefono = dto.Telefono;
            negocio.WhatsApp = dto.WhatsApp;
            negocio.Email = dto.Email;
            negocio.MapaEmbedUrl = dto.MapaEmbedUrl;
            negocio.DescripcionNosotros = dto.DescripcionNosotros;

            var seccionesValidas = new HashSet<string> { "servicios", "nosotros", "galeria", "reservar", "contacto" };
            var orden = dto.OrdenSecciones.Where(s => seccionesValidas.Contains(s)).Distinct().ToList();
            foreach (var s in seccionesValidas)
                if (!orden.Contains(s)) orden.Add(s);
            negocio.OrdenSeccionesJson = JsonSerializer.Serialize(orden);

            negocio.FechaActualizacion = DateTime.Now;

            foreach (var horarioDto in dto.Horarios)
            {
                if (!Enum.TryParse<DayOfWeek>(horarioDto.Dia, true, out var dia)) continue;

                var horario = negocio.Horarios.FirstOrDefault(h => h.DiaSemana == dia);
                if (horario == null)
                {
                    horario = new NegocioHorario { NegocioId = negocio.Id, DiaSemana = dia };
                    _context.NegocioHorarios.Add(horario);
                }

                horario.Abierto = horarioDto.Abierto;
                horario.HoraApertura = horarioDto.Abierto ? horarioDto.Apertura : null;
                horario.HoraCierre = horarioDto.Abierto ? horarioDto.Cierre : null;
            }

            await _context.SaveChangesAsync();

            var actualizado = await ObtenerNegocioActualAsync();
            return Ok(MapearADto(actualizado!));
        }

        [Authorize]
        [HttpPost("logo")]
        public async Task<ActionResult> SubirLogo(IFormFile archivo)
        {
            var maxMb = _configuration.GetValue<int?>("Uploads:MaxLogoSizeMb") ?? 3;
            var validacion = ValidarImagen(archivo, maxMb);
            if (validacion != null) return BadRequest(new { mensaje = validacion });

            var negocio = await ObtenerNegocioActualAsync();
            if (negocio == null) return NotFound();

            var url = await GuardarArchivoAsync(archivo, "logo");
            BorrarArchivoFisico(negocio.LogoUrl);
            negocio.LogoUrl = url;
            negocio.FechaActualizacion = DateTime.Now;
            await _context.SaveChangesAsync();

            return Ok(new { url });
        }

        [Authorize]
        [HttpPost("hero")]
        public async Task<ActionResult> SubirHero(IFormFile archivo)
        {
            var maxMb = _configuration.GetValue<int?>("Uploads:MaxImageSizeMb") ?? 6;
            var validacion = ValidarImagen(archivo, maxMb);
            if (validacion != null) return BadRequest(new { mensaje = validacion });

            var negocio = await ObtenerNegocioActualAsync();
            if (negocio == null) return NotFound();

            var url = await GuardarArchivoAsync(archivo, "hero");
            BorrarArchivoFisico(negocio.HeroImagenUrl);
            negocio.HeroImagenUrl = url;
            negocio.FechaActualizacion = DateTime.Now;
            await _context.SaveChangesAsync();

            return Ok(new { url });
        }

        [Authorize]
        [HttpPost("galeria")]
        public async Task<ActionResult> AgregarFotoGaleria(IFormFile archivo)
        {
            var maxMb = _configuration.GetValue<int?>("Uploads:MaxImageSizeMb") ?? 6;
            var validacion = ValidarImagen(archivo, maxMb);
            if (validacion != null) return BadRequest(new { mensaje = validacion });

            var negocio = await ObtenerNegocioActualAsync();
            if (negocio == null) return NotFound();

            if (negocio.Galeria.Count >= MaxFotosGaleria)
                return BadRequest(new { mensaje = $"Se alcanzó el máximo de {MaxFotosGaleria} fotos en la galería" });

            var url = await GuardarArchivoAsync(archivo, "galeria");
            var siguienteOrden = negocio.Galeria.Count == 0 ? 0 : negocio.Galeria.Max(g => g.Orden) + 1;

            var imagen = new NegocioGaleriaImagen
            {
                NegocioId = negocio.Id,
                Url = url,
                Orden = siguienteOrden
            };
            _context.NegocioGaleriaImagenes.Add(imagen);
            await _context.SaveChangesAsync();

            return Ok(new GaleriaImagenDto { Id = imagen.Id, Url = imagen.Url, Orden = imagen.Orden });
        }

        [Authorize]
        [HttpDelete("galeria/{id}")]
        public async Task<ActionResult> EliminarFotoGaleria(int id)
        {
            var negocioId = _negocioContext.NegocioActualId;
            var imagen = await _context.NegocioGaleriaImagenes
                .FirstOrDefaultAsync(g => g.Id == id && g.NegocioId == negocioId);

            if (imagen == null) return NotFound();

            BorrarArchivoFisico(imagen.Url);
            _context.NegocioGaleriaImagenes.Remove(imagen);
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Foto eliminada" });
        }

        [Authorize]
        [HttpPut("galeria/orden")]
        public async Task<ActionResult> ReordenarGaleria([FromBody] ReordenarGaleriaDto dto)
        {
            var negocioId = _negocioContext.NegocioActualId;
            var imagenes = await _context.NegocioGaleriaImagenes
                .Where(g => g.NegocioId == negocioId)
                .ToListAsync();

            for (int i = 0; i < dto.IdsEnOrden.Count; i++)
            {
                var imagen = imagenes.FirstOrDefault(g => g.Id == dto.IdsEnOrden[i]);
                if (imagen != null) imagen.Orden = i;
            }

            await _context.SaveChangesAsync();
            return Ok(new { mensaje = "Orden actualizado" });
        }

        private async Task<Negocio?> ObtenerNegocioActualAsync()
        {
            var negocioId = _negocioContext.NegocioActualId;
            return await _context.Negocios
                .Include(n => n.Horarios)
                .Include(n => n.Galeria)
                .FirstOrDefaultAsync(n => n.Id == negocioId);
        }

        private static NegocioDto MapearADto(Negocio negocio)
        {
            List<string> orden;
            try
            {
                orden = JsonSerializer.Deserialize<List<string>>(negocio.OrdenSeccionesJson) ?? new();
            }
            catch (JsonException)
            {
                orden = new() { "servicios", "nosotros", "contacto" };
            }

            return new NegocioDto
            {
                Id = negocio.Id,
                Nombre = negocio.Nombre,
                Rubro = negocio.Rubro,
                LogoUrl = negocio.LogoUrl,
                HeroImagenUrl = negocio.HeroImagenUrl,
                ColorPrimario = negocio.ColorPrimario,
                ColorAcento = negocio.ColorAcento,
                ColorTexto = negocio.ColorTexto,
                ColorTextoClaro = negocio.ColorTextoClaro,
                ColorFondo = negocio.ColorFondo,
                ColorFondoClaro = negocio.ColorFondoClaro,
                ThemePresetId = negocio.ThemePresetId,
                Direccion = negocio.Direccion,
                Telefono = negocio.Telefono,
                WhatsApp = negocio.WhatsApp,
                Email = negocio.Email,
                MapaEmbedUrl = negocio.MapaEmbedUrl,
                DescripcionNosotros = negocio.DescripcionNosotros,
                OrdenSecciones = orden,
                Horarios = negocio.Horarios
                    .OrderBy(h => (int)h.DiaSemana)
                    .Select(h => new HorarioDto
                    {
                        Dia = h.DiaSemana.ToString(),
                        Abierto = h.Abierto,
                        Apertura = h.HoraApertura,
                        Cierre = h.HoraCierre
                    }).ToList(),
                Galeria = negocio.Galeria
                    .OrderBy(g => g.Orden)
                    .Select(g => new GaleriaImagenDto { Id = g.Id, Url = g.Url, Orden = g.Orden })
                    .ToList()
            };
        }

        private static string? ValidarImagen(IFormFile? archivo, int maxSizeMb)
        {
            if (archivo == null || archivo.Length == 0)
                return "No se recibió ningún archivo";

            var extension = Path.GetExtension(archivo.FileName);
            if (!ExtensionesPermitidas.Contains(extension))
                return "Formato no permitido. Usá JPG, PNG o WEBP";

            if (!archivo.ContentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase))
                return "El archivo no es una imagen válida";

            if (archivo.Length > maxSizeMb * 1024L * 1024L)
                return $"El archivo supera el tamaño máximo permitido ({maxSizeMb}MB)";

            return null;
        }

        private async Task<string> GuardarArchivoAsync(IFormFile archivo, string subcarpeta)
        {
            var carpeta = Path.Combine(_env.WebRootPath, "uploads", subcarpeta);
            Directory.CreateDirectory(carpeta);

            var extension = Path.GetExtension(archivo.FileName);
            var nombreArchivo = $"{Guid.NewGuid()}{extension}";
            var rutaFisica = Path.Combine(carpeta, nombreArchivo);

            using (var stream = new FileStream(rutaFisica, FileMode.Create))
            {
                await archivo.CopyToAsync(stream);
            }

            return $"/uploads/{subcarpeta}/{nombreArchivo}";
        }

        private void BorrarArchivoFisico(string? url)
        {
            if (string.IsNullOrEmpty(url)) return;

            var rutaRelativa = url.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
            var rutaFisica = Path.Combine(_env.WebRootPath, rutaRelativa);

            if (System.IO.File.Exists(rutaFisica))
            {
                try { System.IO.File.Delete(rutaFisica); } catch (IOException) { /* archivo en uso, se ignora */ }
            }
        }
    }
}
