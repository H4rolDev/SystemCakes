using System;
using H.DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace H.DataAccess;

public partial class sistemContext : DbContext
{
    public sistemContext()
    {
    }

    public sistemContext(DbContextOptions<sistemContext> options)
        : base(options)
    {
    }
    public virtual DbSet<TProducto> TProducto { get; set; }
    public virtual DbSet<TCategoria> TCategoria { get; set; }
    public virtual DbSet<TCliente> TCliente { get; set; }
    public virtual DbSet<TRol> TRol { get; set; }
    public virtual DbSet<TPersona> TPersona { get; set; }
    public virtual DbSet<TUsuario> TUsuario { get; set; }
    public virtual DbSet<TUsuarioRol> TUsuarioRol { get; set; }
    public virtual DbSet<TTipoUsuario> TTipoUsuario { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        modelBuilder.Entity<TProducto>(entity =>
        {
            entity.ToTable("TProducto");

            entity.Property(e => e.Id).HasComment("Identificador de registro");

            entity.Property(e => e.IdCategoria).HasComment("Identificador de categoria del registro");

            entity.Property(e => e.Nombre).HasComment("Nombre del producto");

            entity.Property(e => e.Descripcion).HasComment("Descripcion del producto");

            entity.Property(e => e.Stock).HasComment("Stock del producto");

            entity.Property(e => e.FechaVencimiento)
                .HasColumnType("datetime")
                .HasComment("Fecha de vencimiento del producto");

            entity.Property(e => e.CostoUnitario)
                .HasPrecision(18, 2)
                .HasComment("Costo unitario del producto");

            entity.Property(e => e.CostoTotal)
                .HasPrecision(18, 2)
                .HasComment("Costo total del producto");

            entity.Property(e => e.Igv)
                .HasPrecision(18, 2)
                .HasComment("Igv del producto");

            entity.Property(e => e.Estado).HasComment("Estado del registro");

            entity.Property(e => e.FechaCreacion)
                .HasColumnType("datetime")
                .HasComment("Fecha de creación del registro");

            entity.Property(e => e.FechaModificacion)
                .HasColumnType("datetime")
                .HasComment("Fecha de modificación del registro");

            entity.Property(e => e.UsuarioCreacion)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasComment("Usuario de creación del registro");

            entity.Property(e => e.UsuarioModificacion)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasComment("Usuario de modificación del registro");
        });

        modelBuilder.Entity<TCategoria>(entity =>
        {
            entity.ToTable("TCategoria");

            entity.Property(e => e.Id).HasComment("Identificador de registro");

            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasComment("");

            entity.Property(e => e.Descripcion)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasComment("");

            entity.Property(e => e.Estado).HasComment("Estado del registro");

            entity.Property(e => e.FechaCreacion)
                .HasColumnType("datetime")
                .HasComment("Fecha de creación del registro");

            entity.Property(e => e.FechaModificacion)
                .HasColumnType("datetime")
                .HasComment("Fecha de modificación del registro");

            entity.Property(e => e.UsuarioCreacion)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasComment("Usuario de creación del registro");

            entity.Property(e => e.UsuarioModificacion)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasComment("Usuario de modificación del registro");
        });

        modelBuilder.Entity<TCliente>(entity =>
        {
            entity.ToTable("TCliente");

            entity.Property(e => e.Id).HasComment("Identificador de registro");

            entity.Property(e => e.TipoDocumento)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasComment("");

            entity.Property(e => e.NumeroDocumento)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasComment("");

            entity.Property(e => e.NombresPersona)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasComment("");

            entity.Property(e => e.ApellidosPersona)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasComment("");

            entity.Property(e => e.RazonSocial)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasComment("");

            entity.Property(e => e.Estado).HasComment("Estado del registro");

            entity.Property(e => e.FechaCreacion)
                .HasColumnType("datetime")
                .HasComment("Fecha de creación del registro");

            entity.Property(e => e.FechaModificacion)
                .HasColumnType("datetime")
                .HasComment("Fecha de modificación del registro");

            entity.Property(e => e.UsuarioCreacion)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasComment("Usuario de creación del registro");

            entity.Property(e => e.UsuarioModificacion)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasComment("Usuario de modificación del registro");
        });

        modelBuilder.Entity<TRol>(entity =>
        {
            entity.ToTable("TRol");

            entity.Property(e => e.Id).HasComment("Identificador de registro");

            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasComment("");

            entity.Property(e => e.Estado).HasComment("Estado del registro");

            entity.Property(e => e.FechaCreacion)
                .HasColumnType("datetime")
                .HasComment("Fecha de creación del registro");

            entity.Property(e => e.FechaModificacion)
                .HasColumnType("datetime")
                .HasComment("Fecha de modificación del registro");

            entity.Property(e => e.UsuarioCreacion)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasComment("Usuario de creación del registro");

            entity.Property(e => e.UsuarioModificacion)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasComment("Usuario de modificación del registro");
        });

        modelBuilder.Entity<TPersona>(entity =>
        {
            entity.ToTable("TPersona");

            entity.Property(e => e.Id).HasComment("Identificador de registro");
            entity.Property(e => e.IdUsuario).HasComment("Identificador de usuario");

            entity.Property(e => e.Nombres)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasComment("");

            entity.Property(e => e.ApellidoMaterno)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasComment("");

            entity.Property(e => e.Estado).HasComment("Estado del registro");

            entity.Property(e => e.FechaCreacion)
                .HasColumnType("datetime")
                .HasComment("Fecha de creación del registro");

            entity.Property(e => e.FechaModificacion)
                .HasColumnType("datetime")
                .HasComment("Fecha de modificación del registro");

            entity.Property(e => e.UsuarioCreacion)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasComment("Usuario de creación del registro");

            entity.Property(e => e.UsuarioModificacion)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasComment("Usuario de modificación del registro");
        });

        modelBuilder.Entity<TUsuario>(entity =>
        {
            entity.ToTable("TUsuario");
            entity.Property(e => e.Id).HasComment("Identificador de registro");
            entity.Property(e => e.IdTipoUsuario).HasComment("Tipo de usuario");
            entity.Property(e => e.Username)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasComment("Nombre de usuario");
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(255)
                .HasComment("Hash de contraseña");
            entity.Property(e => e.PasswordSalt)
                .HasMaxLength(255)
                .HasComment("Salt de contraseña");
            entity.Property(e => e.Estado).HasComment("Estado del registro");
            entity.Property(e => e.FechaCreacion)
                .HasColumnType("datetime")
                .HasComment("Fecha de creación del registro");
            entity.Property(e => e.FechaModificacion)
                .HasColumnType("datetime")
                .HasComment("Fecha de modificación del registro");
            entity.Property(e => e.UsuarioCreacion)
                .HasMaxLength(100)
                .HasComment("Usuario de creación del registro");
            entity.Property(e => e.UsuarioModificacion)
                .HasMaxLength(100)
                .HasComment("Usuario de modificación del registro");
        });

        modelBuilder.Entity<TUsuarioRol>(entity =>
        {
            entity.ToTable("TUsuarioRol");
            entity.Property(e => e.Id).HasComment("Identificador de registro");
            entity.Property(e => e.IdUsuario).HasComment("Id de usuario");
            entity.Property(e => e.IdRol).HasComment("Id de rol");
            entity.Property(e => e.Estado).HasComment("Estado del registro");
            entity.Property(e => e.FechaCreacion)
                .HasColumnType("datetime")
                .HasComment("Fecha de creación del registro");
            entity.Property(e => e.FechaModificacion)
                .HasColumnType("datetime")
                .HasComment("Fecha de modificación del registro");
            entity.Property(e => e.UsuarioCreacion)
                .HasMaxLength(100)
                .HasComment("Usuario de creación del registro");
            entity.Property(e => e.UsuarioModificacion)
                .HasMaxLength(100)
                .HasComment("Usuario de modificación del registro");
        });

        modelBuilder.Entity<TTipoUsuario>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.ToTable("TTipoUsuario");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Estado).HasComment("Estado del registro");
            entity.Property(e => e.FechaCreacion)
                .HasColumnType("datetime")
                .HasComment("Fecha de creación del registro");
            entity.Property(e => e.FechaModificacion)
                .HasColumnType("datetime")
                .HasComment("Fecha de modificación del registro");
            entity.Property(e => e.UsuarioCreacion)
                .HasMaxLength(100)
                .HasComment("Usuario de creación del registro");
            entity.Property(e => e.UsuarioModificacion)
                .HasMaxLength(100)
                .HasComment("Usuario de modificación del registro");
        });

        modelBuilder.Entity<TPersona>(entity =>
        {
            entity.ToTable("TPersona");
            entity.Property(e => e.Id).HasComment("Identificador de registro");
            entity.Property(e => e.IdUsuario).HasComment("Identificador de usuario");
            entity.Property(e => e.TipoDocumento)
                .HasMaxLength(50);
            entity.Property(e => e.NumeroDocumento)
                .HasMaxLength(50);
            entity.Property(e => e.RazonSocial)
                .HasMaxLength(255);
            entity.Property(e => e.ApellidoPaterno)
                .HasMaxLength(255);
            entity.Property(e => e.Estado).HasComment("Estado del registro");
            entity.Property(e => e.FechaCreacion)
                .HasColumnType("datetime")
                .HasComment("Fecha de creación del registro");
            entity.Property(e => e.FechaModificacion)
                .HasColumnType("datetime")
                .HasComment("Fecha de modificación del registro");
            entity.Property(e => e.UsuarioCreacion)
                .HasMaxLength(100)
                .HasComment("Usuario de creación del registro");
            entity.Property(e => e.UsuarioModificacion)
                .HasMaxLength(100)
                .HasComment("Usuario de modificación del registro");
        });

        modelBuilder.Entity<TTorta>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.ToTable("TTorta");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Descripcion)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasComment("");
            entity.Property(e => e.PrecioVenta)
                .HasPrecision(18, 2)
                .HasComment("Costo unitario del producto");
            entity.Property(e => e.StockDisponible).HasComment("Stock del producto");
            entity.Property(e => e.Estado).HasComment("Estado del registro");
            entity.Property(e => e.FechaCreacion)
                .HasColumnType("datetime")
                .HasComment("Fecha de creación del registro");
            entity.Property(e => e.FechaModificacion)
                .HasColumnType("datetime")
                .HasComment("Fecha de modificación del registro");
            entity.Property(e => e.UsuarioCreacion)
                .HasMaxLength(100)
                .HasComment("Usuario de creación del registro");
            entity.Property(e => e.UsuarioModificacion)
                .HasMaxLength(100)
                .HasComment("Usuario de modificación del registro");
        });

    }
}
