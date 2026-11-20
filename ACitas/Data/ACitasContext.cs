using System;
using System.Collections.Generic;
using ACitas.Models;
using Microsoft.EntityFrameworkCore;

namespace ACitas.Data;

public partial class ACitasContext : DbContext
{
    public ACitasContext(DbContextOptions<ACitasContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Cita> Citas { get; set; }

    public virtual DbSet<Cliente> Clientes { get; set; }

    public virtual DbSet<HistorialCita> HistorialCitas { get; set; }

    public virtual DbSet<Horario> Horarios { get; set; }

    public virtual DbSet<Notificacione> Notificaciones { get; set; }

    public virtual DbSet<Servicio> Servicios { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Cita>(entity =>
        {
            entity.HasIndex(e => new { e.ClienteId, e.FechaCreacion }, "IX_Citas_Cliente").IsDescending(false, true);

            entity.HasIndex(e => new { e.FechaHoraInicio, e.Estado }, "IX_Citas_FechaEstado");

            entity.HasIndex(e => e.Folio, "UQ_Citas_Folio").IsUnique();

            entity.Property(e => e.ComentarioCliente).HasMaxLength(500);
            entity.Property(e => e.Estado)
                .HasMaxLength(15)
                .HasDefaultValue("Pendiente");
            entity.Property(e => e.FechaActualizacion)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.FechaCreacion)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.FechaHoraInicio).HasPrecision(0);
            entity.Property(e => e.Folio).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.NotaDueno).HasMaxLength(500);

            entity.HasOne(d => d.Cliente).WithMany(p => p.Cita)
                .HasForeignKey(d => d.ClienteId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Citas_Clientes");

            entity.HasOne(d => d.Servicio).WithMany(p => p.Cita)
                .HasForeignKey(d => d.ServicioId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Citas_Servicios");
        });

        modelBuilder.Entity<Cliente>(entity =>
        {
            entity.HasIndex(e => e.Telefono, "IX_Clientes_Telefono");

            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.Correo).HasMaxLength(254);
            entity.Property(e => e.FechaRegistro)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.Nombre).HasMaxLength(120);
            entity.Property(e => e.Telefono).HasMaxLength(20);
        });

        modelBuilder.Entity<HistorialCita>(entity =>
        {
            entity.HasKey(e => e.HistorialId);

            entity.HasIndex(e => new { e.CitaId, e.FechaCambio }, "IX_HistorialCitas_CitaFecha").IsDescending(false, true);

            entity.Property(e => e.EstadoAnterior).HasMaxLength(15);
            entity.Property(e => e.EstadoNuevo).HasMaxLength(15);
            entity.Property(e => e.FechaCambio)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.Nota).HasMaxLength(500);
            entity.Property(e => e.Usuario).HasMaxLength(256);

            entity.HasOne(d => d.Cita).WithMany(p => p.HistorialCita)
                .HasForeignKey(d => d.CitaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_HistorialCitas_Citas");
        });

        modelBuilder.Entity<Horario>(entity =>
        {
            entity.HasIndex(e => new { e.DiaSemana, e.Activo }, "IX_Horarios_Dia_Activo");

            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.HoraFin).HasPrecision(0);
            entity.Property(e => e.HoraInicio).HasPrecision(0);
        });

        modelBuilder.Entity<Notificacione>(entity =>
        {
            entity.HasKey(e => e.NotificacionId);

            entity.HasIndex(e => new { e.EstadoEnvio, e.FechaCreacion }, "IX_Notificaciones_EstadoFecha");

            entity.Property(e => e.Canal)
                .HasMaxLength(20)
                .HasDefaultValue("WhatsApp");
            entity.Property(e => e.Destino).HasMaxLength(30);
            entity.Property(e => e.DetalleError).HasMaxLength(500);
            entity.Property(e => e.EstadoEnvio)
                .HasMaxLength(15)
                .HasDefaultValue("Pendiente");
            entity.Property(e => e.FechaCreacion)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.FechaEnvio).HasPrecision(0);
            entity.Property(e => e.Mensaje).HasMaxLength(1000);

            entity.HasOne(d => d.Cita).WithMany(p => p.Notificaciones)
                .HasForeignKey(d => d.CitaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Notificaciones_Citas");
        });

        modelBuilder.Entity<Servicio>(entity =>
        {
            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.Descripcion).HasMaxLength(500);
            entity.Property(e => e.Nombre).HasMaxLength(100);
            entity.Property(e => e.Precio).HasColumnType("decimal(10, 2)");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
