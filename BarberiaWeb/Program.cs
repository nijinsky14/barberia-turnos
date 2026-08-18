using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using BarberiaWeb.Data;
using BarberiaWeb.Models;
using BarberiaWeb.Services;

var builder = WebApplication.CreateBuilder(args);

// Agregar servicios al contenedor
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configurar DbContext con SQL Server
builder.Services.AddDbContext<BarberiaDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlOptions => sqlOptions.EnableRetryOnFailure()
    )
);

// Registrar servicios personalizados
builder.Services.AddScoped<ITurnoService, TurnoService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddSingleton<INegocioContextService, NegocioContextService>();
builder.Services.AddSingleton<IPasswordHasher<Usuario>, PasswordHasher<Usuario>>();

// Autenticación del panel admin vía cookie
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = "admin_auth";
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.Strict;
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.SlidingExpiration = true;
        // Los endpoints son consumidos por fetch/JS: devolver 401/403 en vez de redirigir a una página de login HTML
        options.Events.OnRedirectToLogin = context =>
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return Task.CompletedTask;
        };
        options.Events.OnRedirectToAccessDenied = context =>
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            return Task.CompletedTask;
        };
    });
builder.Services.AddAuthorization();

// Configurar CORS para permitir el frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy =>
        {
            policy.WithOrigins("http://localhost:5173", "http://127.0.0.1:5500")
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

var app = builder.Build();

// Configurar el pipeline HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Habilitar CORS
app.UseCors("AllowFrontend");

// Habilitar archivos est�ticos (HTML, CSS, JS)
app.UseDefaultFiles();
app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Sembrar el usuario admin una única vez, tomando las credenciales iniciales de appsettings.json
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<BarberiaDbContext>();
    if (!await context.Usuarios.AnyAsync())
    {
        var negocioContext = scope.ServiceProvider.GetRequiredService<INegocioContextService>();
        var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher<Usuario>>();

        var adminUsuario = builder.Configuration["Admin:Usuario"] ?? "admin";
        var adminContrasena = builder.Configuration["Admin:Contrasena"] ?? "CAMBIAR_EN_PRODUCCION";

        var usuario = new Usuario
        {
            NegocioId = negocioContext.NegocioActualId,
            NombreUsuario = adminUsuario
        };
        usuario.PasswordHash = passwordHasher.HashPassword(usuario, adminContrasena);

        context.Usuarios.Add(usuario);
        await context.SaveChangesAsync();

        app.Logger.LogWarning("Se creó el usuario admin '{Usuario}' con la contraseña de appsettings.json. Cambiala desde el panel apenas puedas.", adminUsuario);
    }
}

app.Run();