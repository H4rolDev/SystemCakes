using System;
using H.DataAccess.Entidades;
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
    public virtual DbSet<TRol> TRol { get; set; }
    public virtual DbSet<TPersona> TPersona { get; set; }
    public virtual DbSet<TUsuario> TUsuario { get; set; }
    public virtual DbSet<TUsuarioRol> TUsuarioRol { get; set; }
    //public virtual DbSet<TCargo> TCargo { get; set; }
    //public virtual DbSet<TPersonal> TPersonal { get; set; }
    public virtual DbSet<TCategoriaTorta> TCategoriaTorta { get; set; }
    public virtual DbSet<TTorta> TTorta { get; set; }
    public virtual DbSet<TTortaLote> TTortaLote { get; set; }
    //public virtual DbSet<TUnidadMedida> TUnidadMedida { get; set; }
    public virtual DbSet<TInsumo> TInsumo { get; set; }
    //public virtual DbSet<TInsumoLote> TInsumoLote { get; set; }
    //public virtual DbSet<TTipoMovimiento> TTipoMovimiento { get; set; }
    //public virtual DbSet<TMovimientoInsumo> TMovimientoInsumo { get; set; }
    //public virtual DbSet<TMovimientoTorta> TMovimientoTorta { get; set; }
    public virtual DbSet<TProduccion> TProduccion { get; set; }
    //public virtual DbSet<TProduccionDetalleInsumo> TProduccionDetalleInsumo { get; set; }
    //public virtual DbSet<TEstadoVenta> TEstadoVenta { get; set; }
    //public virtual DbSet<TTipoEntrega> TTipoEntrega { get; set; }
    //public virtual DbSet<TVenta> TVenta { get; set; }
    //public virtual DbSet<TVentaDetalle> TVentaDetalle { get; set; }
    //public virtual DbSet<TMetodoPago> TMetodoPago { get; set; }
    //public virtual DbSet<TPagoVenta> TPagoVenta { get; set; }
    //public virtual DbSet<TEstadoEntrega> TEstadoEntrega { get; set; }
    //public virtual DbSet<TEntregaDelivery> TEntregaDelivery { get; set; }
    //public virtual DbSet<TTipoComprobante> TTipoComprobante { get; set; }
    //public virtual DbSet<TComprobanteVenta> TComprobanteVenta { get; set; }
    //public virtual DbSet<TCancelacionVenta> TCancelacionVenta { get; set; }
    //public virtual DbSet<TPromocion> TPromocion { get; set; }
    //public virtual DbSet<TPromocionTorta> TPromocionTorta { get; set; }
    public virtual DbSet<TRecetaTorta> TRecetaTorta { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // =========================================================
        // CONFIGURACIÓN GLOBAL DE AUDITORÍA
        // =========================================================

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
            {
                modelBuilder.Entity(entityType.ClrType)
                    .Property("FechaCreacion")
                    .HasColumnType("datetime")
                    .IsRequired();

                modelBuilder.Entity(entityType.ClrType)
                    .Property("UsuarioCreacion")
                    .IsRequired();

                modelBuilder.Entity(entityType.ClrType)
                    .Property("FechaModificacion")
                    .HasColumnType("datetime");

                modelBuilder.Entity(entityType.ClrType)
                    .Property("UsuarioModificacion");

                modelBuilder.Entity(entityType.ClrType)
                    .Property("Activo")
                    .IsRequired();
            }
        }

        // =========================================================
        // SEGURIDAD
        // =========================================================

        modelBuilder.Entity<TRol>(entity =>
        {
            entity.ToTable("TRol");
            entity.Property(e => e.Nombre).HasMaxLength(100).IsRequired();
        });

        modelBuilder.Entity<TPersona>(entity =>
        {
            entity.ToTable("TPersona");

            entity.Property(e => e.Nombres)
                .HasMaxLength(150);

            entity.Property(e => e.ApellidoMaterno)
                .HasMaxLength(150);

            entity.Property(e => e.ApellidoPaterno)
                .HasMaxLength(150);

            entity.Property(e => e.NumeroDocumento)
                .HasMaxLength(20)
                .IsRequired();

            entity.Property(e => e.RazonSocial)
                .HasMaxLength(250)
                .IsRequired(false);

            entity.Property(e => e.Telefono)
                .HasMaxLength(20);

            entity.Property(e => e.Direccion)
                .HasMaxLength(300);


            entity.HasOne<TTipoDocumento>()
                .WithMany()
                .HasForeignKey(e => e.IdTipoDocumento)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<TUsuario>(entity =>
        {
            entity.ToTable("TUsuario");
            entity.Property(e => e.Username).HasMaxLength(100).IsRequired();
            entity.Property(e => e.PasswordHash).IsRequired();
            entity.Property(e => e.PasswordSalt).IsRequired();

            entity.HasOne<TPersona>()
                .WithMany()
                .HasForeignKey(e => e.IdPersona)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<TUsuarioRol>(entity =>
        {
            entity.ToTable("TUsuarioRol");

            entity.HasOne<TUsuario>()
                .WithMany()
                .HasForeignKey(e => e.IdUsuario)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne<TRol>()
                .WithMany()
                .HasForeignKey(e => e.IdRol)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // =========================================================
        // PERSONAL
        // =========================================================

        /*modelBuilder.Entity<TCargo>(entity =>
        {
            entity.ToTable("TCargo");
            entity.Property(e => e.Nombre).HasMaxLength(100).IsRequired();
        });

        modelBuilder.Entity<TPersonal>(entity =>
        {
            entity.ToTable("TPersonal");

            entity.HasOne<TPersona>()
                .WithMany()
                .HasForeignKey(e => e.IdPersona)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne<TCargo>()
                .WithMany()
                .HasForeignKey(e => e.IdCargo)
                .OnDelete(DeleteBehavior.Restrict);
        });*/

        // =========================================================
        // TORTAS
        // =========================================================

        modelBuilder.Entity<TCategoriaTorta>(entity =>
        {
            entity.ToTable("TCategoriaTorta");
            entity.Property(e => e.Nombre).HasMaxLength(100).IsRequired();
        });

        modelBuilder.Entity<TTorta>(entity =>
        {
            entity.ToTable("TTorta");

            entity.Property(e => e.Nombre).HasMaxLength(150).IsRequired();
            entity.Property(e => e.Descripcion).HasMaxLength(500);
            entity.Property(e => e.Cantidades).HasMaxLength(150);
            entity.Property(e => e.PrecioVenta).HasPrecision(10, 2).IsRequired();
            entity.Property(e => e.EsPersonalizable).HasPrecision(10, 2);
            entity.Property(e => e.ImagenUrl);
            entity.Property(e => e.ImagenPublicId);

            entity.HasOne<TCategoriaTorta>()
                .WithMany()
                .HasForeignKey(e => e.IdCategoriaTorta)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<TTortaLote>(entity =>
        {
            entity.ToTable("TTortaLote");

            entity.Property(e => e.NumeroLote).HasPrecision(10, 2).IsRequired();
            entity.Property(e => e.CantidadInicial).HasPrecision(10, 2).IsRequired();
            entity.Property(e => e.CantidaddDisponible).HasPrecision(10, 2).IsRequired();
            entity.Property(e => e.FechaProduccion).HasColumnType("datetime").IsRequired();
            entity.Property(e => e.FechaVencimiento).HasColumnType("datetime").IsRequired();

            entity.HasOne<TTorta>()
                .WithMany()
                .HasForeignKey(e => e.IdTorta)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // =========================================================
        // INSUMOS
        // =========================================================

        modelBuilder.Entity<TUnidadMedida>(entity =>
        {
            entity.ToTable("TUnidadMedida");
            entity.Property(e => e.Nombre).HasMaxLength(50).IsRequired();
            entity.Property(e => e.Abreviatura).HasMaxLength(10).IsRequired();
        });

        modelBuilder.Entity<TInsumo>(entity =>
        {
            entity.ToTable("TInsumo");

            entity.Property(e => e.Nombre).HasMaxLength(150).IsRequired();
            entity.Property(e => e.StockActual).HasPrecision(18, 2).IsRequired();
            entity.Property(e => e.StockMinimo).HasPrecision(18, 2).IsRequired();
            entity.Property(e => e.CostoUnitario).HasPrecision(18, 2).IsRequired();

            entity.HasOne<TUnidadMedida>()
                .WithMany()
                .HasForeignKey(e => e.IdUnidadMedida)
                .OnDelete(DeleteBehavior.Restrict);
        });

        /*modelBuilder.Entity<TInsumoLote>(entity =>
        {
            entity.ToTable("TInsumoLote");

            entity.Property(e => e.CantidadDisponible).HasPrecision(10, 2).IsRequired();
            entity.Property(e => e.CostoUnitario).HasPrecision(10, 2).IsRequired();
            entity.Property(e => e.FechaIngreso).HasColumnType("datetime").IsRequired();
            entity.Property(e => e.FechaVencimiento).HasColumnType("datetime");

            entity.HasOne<TInsumo>()
                .WithMany()
                .HasForeignKey(e => e.IdInsumo)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<TTipoMovimiento>(entity =>
        {
            entity.ToTable("TTipoMovimiento");
            entity.Property(e => e.Nombre).HasMaxLength(100).IsRequired();
        });

        modelBuilder.Entity<TMovimientoInsumo>(entity =>
        {
            entity.ToTable("TMovimientoInsumo");

            entity.Property(e => e.Cantidad).HasPrecision(10, 2).IsRequired();
            entity.Property(e => e.FechaMovimiento).HasColumnType("datetime").IsRequired();

            entity.HasOne<TInsumoLote>()
                .WithMany()
                .HasForeignKey(e => e.IdInsumoLote)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne<TTipoMovimiento>()
                .WithMany()
                .HasForeignKey(e => e.IdTipoMovimiento)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<TMovimientoTorta>(entity =>
        {
            entity.ToTable("TMovimientoTorta");

            entity.Property(e => e.Cantidad).HasPrecision(10, 2).IsRequired();
            entity.Property(e => e.FechaMovimiento).HasColumnType("datetime").IsRequired();

            entity.HasOne<TTortaLote>()
                .WithMany()
                .HasForeignKey(e => e.IdTortaLote)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne<TTipoMovimiento>()
                .WithMany()
                .HasForeignKey(e => e.IdTipoMovimiento)
                .OnDelete(DeleteBehavior.Restrict);
        });*/

        // =========================================================
        // PRODUCCIÓN
        // =========================================================

        modelBuilder.Entity<TProduccion>(entity =>
        {
            entity.ToTable("TProduccion");

            entity.Property(e => e.CantidadProducida).HasPrecision(10, 2).IsRequired();
            entity.Property(e => e.Observacion).HasMaxLength(150);
            entity.Property(e => e.FechaProduccion).HasColumnType("datetime").IsRequired();

            entity.HasOne<TTorta>()
                .WithMany()
                .HasForeignKey(e => e.IdTorta)
                .OnDelete(DeleteBehavior.Restrict);
        });

        /*modelBuilder.Entity<TProduccionDetalleInsumo>(entity =>
        {
            entity.ToTable("TProduccionDetalleInsumo");

            entity.Property(e => e.CantidadUsada).HasPrecision(10, 2).IsRequired();

            entity.HasOne<TProduccion>()
                .WithMany()
                .HasForeignKey(e => e.IdProduccion)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne<TInsumoLote>()
                .WithMany()
                .HasForeignKey(e => e.IdInsumoLote)
                .OnDelete(DeleteBehavior.Restrict);
        });*/

        modelBuilder.Entity<TRecetaTorta>(entity =>
        {
            entity.ToTable("TRecetaTorta");

            entity.Property(e => e.CantidadRequerida).HasPrecision(10, 2).IsRequired();

            entity.HasIndex(e => new { e.IdTorta, e.IdInsumo }).IsUnique();

            entity.HasOne<TTorta>()
                .WithMany()
                .HasForeignKey(e => e.IdTorta)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne<TInsumo>()
                .WithMany()
                .HasForeignKey(e => e.IdInsumo)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // =========================================================
        // VENTAS
        // =========================================================

        //modelBuilder.Entity<TEstadoVenta>(entity =>
        //{
        //    entity.ToTable("TEstadoVenta");
        //    entity.Property(e => e.Nombre).HasMaxLength(100).IsRequired();
        //});

        //modelBuilder.Entity<TTipoEntrega>(entity =>
        //{
        //    entity.ToTable("TTipoEntrega");
        //    entity.Property(e => e.Nombre).HasMaxLength(100).IsRequired();
        //});

        //modelBuilder.Entity<TVenta>(entity =>
        //{
        //    entity.ToTable("TVenta");

        //    entity.Property(e => e.Total).HasPrecision(10, 2).IsRequired();
        //    entity.Property(e => e.FechaVenta).HasColumnType("datetime").IsRequired();

        //    entity.HasOne<TEstadoVenta>()
        //        .WithMany()
        //        .HasForeignKey(e => e.IdEstadoVenta)
        //        .OnDelete(DeleteBehavior.Restrict);

        //    entity.HasOne<TTipoEntrega>()
        //        .WithMany()
        //        .HasForeignKey(e => e.IdTipoEntrega)
        //        .OnDelete(DeleteBehavior.Restrict);
        //});

        //modelBuilder.Entity<TVentaDetalle>(entity =>
        //{
        //    entity.ToTable("TVentaDetalle");

        //    entity.Property(e => e.Cantidad).HasPrecision(10, 2).IsRequired();
        //    entity.Property(e => e.PrecioBase).HasPrecision(10, 2).IsRequired();
        //    entity.Property(e => e.PrecioPersonalizacion).HasPrecision(10, 2);
        //    entity.Property(e => e.PrecioFinal).HasPrecision(10, 2).IsRequired();
        //    entity.Property(e => e.SubTotal).HasPrecision(10, 2).IsRequired();

        //    entity.HasOne<TVenta>()
        //        .WithMany()
        //        .HasForeignKey(e => e.IdVenta)
        //        .OnDelete(DeleteBehavior.Cascade);

        //    entity.HasOne<TTorta>()
        //        .WithMany()
        //        .HasForeignKey(e => e.IdTorta)
        //        .OnDelete(DeleteBehavior.Restrict);

        //    entity.HasOne<TTortaLote>()
        //        .WithMany()
        //        .HasForeignKey(e => e.IdTortaLote)
        //        .OnDelete(DeleteBehavior.Restrict);
        //});

        //modelBuilder.Entity<TMetodoPago>(entity =>
        //{
        //    entity.ToTable("TMetodoPago");
        //    entity.Property(e => e.Nombre).HasMaxLength(100).IsRequired();
        //});

        //modelBuilder.Entity<TPagoVenta>(entity =>
        //{
        //    entity.ToTable("TPagoVenta");

        //    entity.Property(e => e.Monto).HasPrecision(10, 2).IsRequired();
        //    entity.Property(e => e.FechaPago).HasColumnType("datetime").IsRequired();

        //    entity.HasOne<TVenta>()
        //        .WithMany()
        //        .HasForeignKey(e => e.IdVenta)
        //        .OnDelete(DeleteBehavior.Cascade);

        //    entity.HasOne<TMetodoPago>()
        //        .WithMany()
        //        .HasForeignKey(e => e.IdMetodoPago)
        //        .OnDelete(DeleteBehavior.Restrict);
        //});

        //modelBuilder.Entity<TEstadoEntrega>(entity =>
        //{
        //    entity.ToTable("TEstadoEntrega");
        //    entity.Property(e => e.Nombre).HasMaxLength(100).IsRequired();
        //});

        //modelBuilder.Entity<TEntregaDelivery>(entity =>
        //{
        //    entity.ToTable("TEntregaDelivery");

        //    entity.Property(e => e.DireccionEntrega).HasMaxLength(300).IsRequired();
        //    entity.Property(e => e.FechaEntregaProgramada).HasColumnType("datetime");

        //    entity.HasOne<TVenta>()
        //        .WithMany()
        //        .HasForeignKey(e => e.IdVenta)
        //        .OnDelete(DeleteBehavior.Cascade);

        //    entity.HasOne<TEstadoEntrega>()
        //        .WithMany()
        //        .HasForeignKey(e => e.IdEstadoEntrega)
        //        .OnDelete(DeleteBehavior.Restrict);
        //});

        //modelBuilder.Entity<TTipoComprobante>(entity =>
        //{
        //    entity.ToTable("TTipoComprobante");
        //    entity.Property(e => e.Nombre).HasMaxLength(100).IsRequired();
        //});

        //modelBuilder.Entity<TComprobanteVenta>(entity =>
        //{
        //    entity.ToTable("TComprobanteVenta");

        //    entity.Property(e => e.Serie).HasMaxLength(10).IsRequired();
        //    entity.Property(e => e.Numero).HasMaxLength(20).IsRequired();
        //    entity.Property(e => e.FechaEmision).HasColumnType("datetime").IsRequired();

        //    entity.HasOne<TVenta>()
        //        .WithMany()
        //        .HasForeignKey(e => e.IdVenta)
        //        .OnDelete(DeleteBehavior.Cascade);

        //    entity.HasOne<TTipoComprobante>()
        //        .WithMany()
        //        .HasForeignKey(e => e.IdTipoComprobante)
        //        .OnDelete(DeleteBehavior.Restrict);
        //});

        //modelBuilder.Entity<TCancelacionVenta>(entity =>
        //{
        //    entity.ToTable("TCancelacionVenta");

        //    entity.Property(e => e.Motivo).HasMaxLength(500).IsRequired();
        //    entity.Property(e => e.FechaCancelacion).HasColumnType("datetime").IsRequired();

        //    entity.HasOne<TVenta>()
        //        .WithMany()
        //        .HasForeignKey(e => e.IdVenta)
        //        .OnDelete(DeleteBehavior.Cascade);
        //});

        // =========================================================
        // PROMOCIONES
        // =========================================================

        //modelBuilder.Entity<TPromocion>(entity =>
        //{
        //    entity.ToTable("TPromocion");

        //    entity.Property(e => e.Nombre).HasMaxLength(150).IsRequired();
        //    entity.Property(e => e.DescuentoPorcentaje).HasPrecision(5, 2).IsRequired();
        //    entity.Property(e => e.FechaInicio).HasColumnType("datetime").IsRequired();
        //    entity.Property(e => e.FechaFin).HasColumnType("datetime");
        //});

        //modelBuilder.Entity<TPromocionTorta>(entity =>
        //{
        //    entity.ToTable("TPromocionTorta");

        //    entity.HasOne<TPromocion>()
        //        .WithMany()
        //        .HasForeignKey(e => e.IdPromocion)
        //        .OnDelete(DeleteBehavior.Cascade);

        //    entity.HasOne<TTorta>()
        //        .WithMany()
        //        .HasForeignKey(e => e.IdTorta)
        //        .OnDelete(DeleteBehavior.Restrict);
        //});

        modelBuilder.Entity<TTipoDocumento>(entity =>
        {
            entity.ToTable("TTipoDocumento");

            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .IsRequired();

        });
    }
}