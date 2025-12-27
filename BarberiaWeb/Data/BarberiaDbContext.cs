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

            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                base.OnModelCreating(modelBuilder);

                modelBuilder.Entity<Cliente>(entity =>
                {
                    entity.HasIndex(e => e.Telefono);
                    entity.HasIndex(e => e.Email);
                });

                modelBuilder.Entity<Turno>(entity =>
                {
                    entity.HasIndex(e => e.FechaTurno);
                    entity.HasIndex(e => e.Estado);

                    entity.HasOne(t => t.Cliente)
                        .WithMany(c => c.Turnos)
                        .HasForeignKey(t => t.ClienteId)
                        .OnDelete(DeleteBehavior.Restrict);

                    entity.HasOne(t => t.Servicio)
                        .WithMany(s => s.Turnos)
                        .HasForeignKey(t => t.ServicioId)
                        .OnDelete(DeleteBehavior.Restrict);
                });

                modelBuilder.Entity<Servicio>().HasData(
                    new Servicio { Id = 1, Nombre = "Corte Clásico", Descripcion = "Corte profesional", Precio = 5000, DuracionMinutos = 30, Activo = true },
                    new Servicio { Id = 2, Nombre = "Corte + Barba", Descripcion = "Combo completo", Precio = 7500, DuracionMinutos = 45, Activo = true },
                    new Servicio { Id = 3, Nombre = "Barba", Descripcion = "Perfilado de barba", Precio = 3500, DuracionMinutos = 20, Activo = true },
                    new Servicio { Id = 4, Nombre = "Corte Niños", Descripcion = "Corte para niños", Precio = 4000, DuracionMinutos = 25, Activo = true }
                );
            }
        }
    }


