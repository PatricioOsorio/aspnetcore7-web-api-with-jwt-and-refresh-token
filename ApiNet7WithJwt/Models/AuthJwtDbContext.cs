using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace ApiNet7WithJwt.Models;

public partial class AuthJwtDbContext : DbContext
{
  public AuthJwtDbContext()
  {
  }

  public AuthJwtDbContext(DbContextOptions<AuthJwtDbContext> options)
      : base(options)
  {
  }

  public virtual DbSet<HistorialRefreshToken> HistorialRefreshTokens { get; set; }

  public virtual DbSet<Usuario> Usuarios { get; set; }

  protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
  { }

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    modelBuilder.Entity<HistorialRefreshToken>(entity =>
    {
      entity.HasKey(e => e.Id).HasName("PK__Historia__3214EC07645B3978");

      entity.ToTable("HistorialRefreshToken");

      entity.Property(e => e.EsActivo).HasComputedColumnSql("(case when [FechaExpiracion]<getdate() then CONVERT([bit],(0)) else CONVERT([bit],(1)) end)", false);
      entity.Property(e => e.FechaCreacion).HasColumnType("datetime");
      entity.Property(e => e.FechaExpiracion).HasColumnType("datetime");
      entity.Property(e => e.RefreshToken)
              .HasMaxLength(300)
              .IsUnicode(false);
      entity.Property(e => e.Token)
              .HasMaxLength(500)
              .IsUnicode(false);

      entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.HistorialRefreshTokens)
              .HasForeignKey(d => d.IdUsuario)
              .HasConstraintName("FK__Historial__IdUsu__628FA481");
    });

    modelBuilder.Entity<Usuario>(entity =>
    {
      entity.HasKey(e => e.Id).HasName("PK__Usuario__3214EC0706C4E073");

      entity.ToTable("Usuario");

      entity.Property(e => e.Contrasena)
              .HasMaxLength(20)
              .IsUnicode(false);
      entity.Property(e => e.Nombre)
              .HasMaxLength(50)
              .IsUnicode(false);
    });

    OnModelCreatingPartial(modelBuilder);
  }

  partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
