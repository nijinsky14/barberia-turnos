using BarberiaWeb.Models;
using Microsoft.EntityFrameworkCore;



namespace BarberiaWeb.Data
    {
        public class BarberiaDbContext : DbContext
        {
            public BarberiaDbContext(DbContextOptions<BarberiaDbContext> options)
                : base(options)
            {
            }

            public DbSet<Cliente> Clientes { get; set; }
            public DbSet<Servicio> Servicios { get; set; }
            public DbSet<Turno> Turnos { get; set; }
            public DbSet<Negocio> Negocios { get; set; }
            public DbSet<NegocioHorario> NegocioHorarios { get; set; }
            public DbSet<NegocioGaleriaImagen> NegocioGaleriaImagenes { get; set; }
            public DbSet<Usuario> Usuarios { get; set; }

            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                base.OnModelCreating(modelBuilder);

                modelBuilder.Entity<Cliente>(entity =>
                {
                    entity.HasIndex(e => e.Telefono);
                    entity.HasIndex(e => e.Email);
                    entity.Property(e => e.NegocioId).HasDefaultValue(1);

                    entity.HasOne(c => c.Negocio)
                        .WithMany()
                        .HasForeignKey(c => c.NegocioId)
                        .OnDelete(DeleteBehavior.Restrict);
                });

                modelBuilder.Entity<Servicio>(entity =>
                {
                    entity.Property(e => e.NegocioId).HasDefaultValue(1);

                    entity.HasOne(s => s.Negocio)
                        .WithMany()
                        .HasForeignKey(s => s.NegocioId)
                        .OnDelete(DeleteBehavior.Restrict);
                });

                modelBuilder.Entity<Turno>(entity =>
                {
                    entity.HasIndex(e => e.FechaTurno);
                    entity.HasIndex(e => e.Estado);
                    entity.Property(e => e.NegocioId).HasDefaultValue(1);

                    entity.HasOne(t => t.Cliente)
                        .WithMany(c => c.Turnos)
                        .HasForeignKey(t => t.ClienteId)
                        .OnDelete(DeleteBehavior.Restrict);

                    entity.HasOne(t => t.Servicio)
                        .WithMany(s => s.Turnos)
                        .HasForeignKey(t => t.ServicioId)
                        .OnDelete(DeleteBehavior.Restrict);

                    entity.HasOne(t => t.Negocio)
                        .WithMany()
                        .HasForeignKey(t => t.NegocioId)
                        .OnDelete(DeleteBehavior.Restrict);
                });

                modelBuilder.Entity<NegocioHorario>(entity =>
                {
                    entity.HasOne(h => h.Negocio)
                        .WithMany(n => n.Horarios)
                        .HasForeignKey(h => h.NegocioId)
                        .OnDelete(DeleteBehavior.Cascade);
                });

                modelBuilder.Entity<NegocioGaleriaImagen>(entity =>
                {
                    entity.HasOne(g => g.Negocio)
                        .WithMany(n => n.Galeria)
                        .HasForeignKey(g => g.NegocioId)
                        .OnDelete(DeleteBehavior.Cascade);
                });

                modelBuilder.Entity<Usuario>(entity =>
                {
                    entity.HasIndex(e => e.NombreUsuario).IsUnique();

                    entity.HasOne(u => u.Negocio)
                        .WithMany()
                        .HasForeignKey(u => u.NegocioId)
                        .OnDelete(DeleteBehavior.Cascade);
                });

                modelBuilder.Entity<Negocio>().HasData(
                    new Negocio
                    {
                        Id = 1,
                        Nombre = "LUCK BARBER",
                        Rubro = "Barberia",
                        ColorPrimario = "#1a1a1a",
                        ColorAcento = "#d4af37",
                        ColorTexto = "#333333",
                        ColorTextoClaro = "#666666",
                        ColorFondo = "#ffffff",
                        ColorFondoClaro = "#f8f8f8",
                        Direccion = "Av. Pellegrini 1234, Rosario, Santa Fe",
                        WhatsApp = "5493416901109",
                        Email = "luckbarber@gmail.com",
                        DescripcionNosotros = "En LUCK BARBER, creemos que cada corte es una obra de arte. Desde 2018, nos dedicamos a ofrecer más que un servicio de barbería: creamos experiencias únicas donde el estilo y la personalidad de cada cliente se encuentran.\n\nNuestro equipo de barberos profesionales está capacitado en las últimas tendencias y técnicas clásicas, garantizando resultados impecables en cada visita. Utilizamos productos premium y trabajamos con atención al detalle para que salgas luciendo exactamente como lo imaginás.\n\nUbicados en el corazón de Rosario, nos hemos convertido en el punto de encuentro para quienes buscan calidad, confianza y un ambiente relajado. Ya sea un corte clásico, un fade moderno o un arreglo de barba profesional, en LUCK BARBER encontrás tu mejor versión.",
                        OrdenSeccionesJson = "[\"servicios\",\"nosotros\",\"contacto\"]",
                        FechaActualizacion = new DateTime(2025, 12, 26)
                    }
                );

                modelBuilder.Entity<NegocioHorario>().HasData(
                    new NegocioHorario { Id = 1, NegocioId = 1, DiaSemana = DayOfWeek.Monday, Abierto = true, HoraApertura = "09:00", HoraCierre = "20:00" },
                    new NegocioHorario { Id = 2, NegocioId = 1, DiaSemana = DayOfWeek.Tuesday, Abierto = true, HoraApertura = "09:00", HoraCierre = "20:00" },
                    new NegocioHorario { Id = 3, NegocioId = 1, DiaSemana = DayOfWeek.Wednesday, Abierto = true, HoraApertura = "09:00", HoraCierre = "20:00" },
                    new NegocioHorario { Id = 4, NegocioId = 1, DiaSemana = DayOfWeek.Thursday, Abierto = true, HoraApertura = "09:00", HoraCierre = "20:00" },
                    new NegocioHorario { Id = 5, NegocioId = 1, DiaSemana = DayOfWeek.Friday, Abierto = true, HoraApertura = "09:00", HoraCierre = "20:00" },
                    new NegocioHorario { Id = 6, NegocioId = 1, DiaSemana = DayOfWeek.Saturday, Abierto = true, HoraApertura = "09:00", HoraCierre = "18:00" },
                    new NegocioHorario { Id = 7, NegocioId = 1, DiaSemana = DayOfWeek.Sunday, Abierto = false, HoraApertura = null, HoraCierre = null }
                );

                modelBuilder.Entity<Servicio>().HasData(
                    new Servicio { Id = 1, NegocioId = 1, Nombre = "Corte Clásico", Descripcion = "Corte profesional", Precio = 5000, DuracionMinutos = 30, Activo = true },
                    new Servicio { Id = 2, NegocioId = 1, Nombre = "Corte + Barba", Descripcion = "Combo completo", Precio = 7500, DuracionMinutos = 45, Activo = true },
                    new Servicio { Id = 3, NegocioId = 1, Nombre = "Barba", Descripcion = "Perfilado de barba", Precio = 3500, DuracionMinutos = 20, Activo = true },
                    new Servicio { Id = 4, NegocioId = 1, Nombre = "Corte Niños", Descripcion = "Corte para niños", Precio = 4000, DuracionMinutos = 25, Activo = true }
                );
            }
        }
    }


