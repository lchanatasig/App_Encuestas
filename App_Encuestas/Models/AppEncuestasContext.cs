using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace App_Encuestas.Models;

public partial class AppEncuestasContext : DbContext
{
    public AppEncuestasContext()
    {
    }

    public AppEncuestasContext(DbContextOptions<AppEncuestasContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Aseguradora> Aseguradoras { get; set; }

    public virtual DbSet<Encuestum> Encuesta { get; set; }

    public virtual DbSet<Pregunta> Preguntas { get; set; }

    public virtual DbSet<Respuestum> Respuesta { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=localhost;Database=app_encuestas;User Id=sa;Password=Sur2o22--;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Aseguradora>(entity =>
        {
            entity.HasKey(e => e.AseguradoraId).HasName("PK__asegurad__A3D1452AD76AB7B3");

            entity.ToTable("aseguradora");

            entity.Property(e => e.AseguradoraId).HasColumnName("aseguradora_id");
            entity.Property(e => e.Estado)
                .HasDefaultValue(true)
                .HasColumnName("estado");
            entity.Property(e => e.Nombre)
                .HasMaxLength(150)
                .HasColumnName("nombre");
        });

        modelBuilder.Entity<Encuestum>(entity =>
        {
            entity.HasKey(e => e.EncuestaId).HasName("PK__encuesta__8F3A1FC947EAE62B");

            entity.ToTable("encuesta");

            entity.HasIndex(e => e.Token, "UQ__encuesta__CA90DA7A7B75D382").IsUnique();

            entity.Property(e => e.EncuestaId).HasColumnName("encuesta_id");
            entity.Property(e => e.AseguradoraId).HasColumnName("aseguradora_id");
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("fecha_creacion");
            entity.Property(e => e.NumeroCaso)
                .HasMaxLength(50)
                .HasColumnName("numero_caso");
            entity.Property(e => e.Respondido)
                .HasDefaultValue(false)
                .HasColumnName("respondido");
            entity.Property(e => e.Token)
                .HasMaxLength(100)
                .HasColumnName("token");

            entity.HasOne(d => d.Aseguradora).WithMany(p => p.Encuesta)
                .HasForeignKey(d => d.AseguradoraId)
                .HasConstraintName("FK_Encuesta_Aseguradora");
        });

        modelBuilder.Entity<Pregunta>(entity =>
        {
            entity.HasKey(e => e.PreguntaId).HasName("PK__pregunta__B40C97E006DAABDE");

            entity.ToTable("preguntas");

            entity.Property(e => e.PreguntaId).HasColumnName("pregunta_id");
            entity.Property(e => e.Orden).HasColumnName("orden");
            entity.Property(e => e.Texto)
                .HasMaxLength(500)
                .HasColumnName("texto");
            entity.Property(e => e.Tipo)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("tipo");
        });

        modelBuilder.Entity<Respuestum>(entity =>
        {
            entity.HasKey(e => e.RespuestaId).HasName("PK__respuest__5D54E93D1EBE9B70");

            entity.ToTable("respuesta");

            entity.Property(e => e.RespuestaId).HasColumnName("respuesta_id");
            entity.Property(e => e.EncuestaId).HasColumnName("encuesta_id");
            entity.Property(e => e.PreguntaId).HasColumnName("pregunta_id");
            entity.Property(e => e.Valor).HasColumnName("valor");

            entity.HasOne(d => d.Encuesta).WithMany(p => p.Respuesta)
                .HasForeignKey(d => d.EncuestaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Respuesta_Encuesta");

            entity.HasOne(d => d.Pregunta).WithMany(p => p.Respuesta)
                .HasForeignKey(d => d.PreguntaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Respuesta_Pregunta");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
