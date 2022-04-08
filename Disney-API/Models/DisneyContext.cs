using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Disney_API.Models
{
    public partial class DisneyContext : DbContext
    {
        public DisneyContext()
        {
        }

        public DisneyContext(DbContextOptions<DisneyContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Genero> Generos { get; set; } = null!;
        public virtual DbSet<GeneroPelicula> GeneroPeliculas { get; set; } = null!;
        public virtual DbSet<Participacion> Participacions { get; set; } = null!;
        public virtual DbSet<Pelicula> Peliculas { get; set; } = null!;
        public virtual DbSet<Personaje> Personajes { get; set; } = null!;
        public virtual DbSet<Usuario> Usuarios { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {

                optionsBuilder.UseSqlServer("Server=JOHAN; Database=Disney; Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Genero>(entity =>
            {
                entity.HasKey(e => e.Idgenero)
                    .HasName("PK_GENERO");

                entity.ToTable("Genero");

                entity.Property(e => e.Idgenero)
                    .ValueGeneratedNever()
                    .HasColumnName("IDGenero");

                entity.Property(e => e.Imagen).HasColumnType("text");

                entity.Property(e => e.Nombre).HasColumnType("text");
            });

            modelBuilder.Entity<GeneroPelicula>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("GeneroPelicula");

                entity.Property(e => e.Idgenero).HasColumnName("IDGenero");

                entity.Property(e => e.Idpelicula).HasColumnName("IDPelicula");

                entity.HasOne(d => d.IdgeneroNavigation)
                    .WithMany()
                    .HasForeignKey(d => d.Idgenero)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("GeneroPelicula_fk0");

                entity.HasOne(d => d.IdpeliculaNavigation)
                    .WithMany()
                    .HasForeignKey(d => d.Idpelicula)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("GeneroPelicula_fk1");
            });

            modelBuilder.Entity<Participacion>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("Participacion");

                entity.Property(e => e.Idpelicula).HasColumnName("IDPelicula");

                entity.Property(e => e.Idpersonaje).HasColumnName("IDPersonaje");

                entity.HasOne(d => d.IdpeliculaNavigation)
                    .WithMany()
                    .HasForeignKey(d => d.Idpelicula)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Participa_fk0");

                entity.HasOne(d => d.IdpersonajeNavigation)
                    .WithMany()
                    .HasForeignKey(d => d.Idpersonaje)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Participa_fk1");
            });

            modelBuilder.Entity<Pelicula>(entity =>
            {
                entity.HasKey(e => e.Idpelicula)
                    .HasName("PK_PELICULA");

                entity.ToTable("Pelicula");

                entity.Property(e => e.Idpelicula)
                    .ValueGeneratedNever()
                    .HasColumnName("IDPelicula");

                entity.Property(e => e.Fecha).HasColumnType("date");

                entity.Property(e => e.Imagen).HasMaxLength(100);

                entity.Property(e => e.Titulo).HasMaxLength(50);

                entity.Property(e => e.Calificacion).HasColumnType("float");
            });

            modelBuilder.Entity<Personaje>(entity =>
            {
                entity.HasKey(e => e.Idpersonaje)
                    .HasName("PK_PERSONAJE");

                entity.ToTable("Personaje");

                entity.Property(e => e.Idpersonaje)
                    .ValueGeneratedNever()
                    .HasColumnName("IDPersonaje");

                entity.Property(e => e.Historia).HasMaxLength(int.MaxValue);

                entity.Property(e => e.Imagen).HasMaxLength(10);

                entity.Property(e => e.Nombre).HasMaxLength(50);
            });

            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.ToTable("Usuario");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("ID");

                entity.Property(e => e.Email).HasMaxLength(50);

                entity.Property(e => e.Password).HasMaxLength(129);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
