using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using POS.Models;

namespace POS.Data;

public partial class ApplicationDbContext : DbContext
{
    public ApplicationDbContext()
    {
    }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Categoria> Categoria { get; set; }

    public virtual DbSet<ControlPrecio> ControlPrecios { get; set; }

    public virtual DbSet<DetalleVenta> DetalleVenta { get; set; }

    public virtual DbSet<Producto> Productos { get; set; }

    public virtual DbSet<Stock> Stocks { get; set; }

    public virtual DbSet<Venta> Venta { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Categoria>(entity =>
        {
            entity.HasKey(e => e.CategoriaId).HasName("PK__Categori__F353C1E5D495B281");

            entity.Property(e => e.Descripcion)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<ControlPrecio>(entity =>
        {
            entity.HasKey(e => e.ControlPrecioId).HasName("PK__ControlP__F1C9AF190797897F");

            entity.ToTable("ControlPrecio");

            entity.HasIndex(e => e.ProductoId, "UQ__ControlP__A430AEA2752DC4D0").IsUnique();

            entity.Property(e => e.CodigoBarra)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Costo).HasColumnType("decimal(20, 2)");
            entity.Property(e => e.PorcentajeGanancia).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.PrecioVenta).HasColumnType("decimal(20, 2)");

            entity.HasOne(d => d.Producto).WithOne(p => p.ControlPrecio)
                .HasForeignKey<ControlPrecio>(d => d.ProductoId)
                .HasConstraintName("FK_Producto_ControlPrecio");
        });

        modelBuilder.Entity<DetalleVenta>(entity =>
        {
            entity.HasKey(e => e.DetalleVentaId).HasName("PK__DetalleV__340EEDA48D3471F9");

            entity.Property(e => e.CodigoBarra)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.PrecioUnitario).HasColumnType("decimal(20, 2)");
            entity.Property(e => e.Subtotal)
                .HasComputedColumnSql("(round([Cantidad]*[PrecioUnitario],(2)))", false)
                .HasColumnType("decimal(31, 2)");

            entity.HasOne(d => d.Producto).WithMany(p => p.DetalleVenta)
                .HasForeignKey(d => d.ProductoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DetalleVenta_Producto");

            entity.HasOne(d => d.Venta).WithMany(p => p.DetalleVenta)
                .HasForeignKey(d => d.VentaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__DetalleVe__Venta__5165187F");
        });

        modelBuilder.Entity<Producto>(entity =>
        {
            entity.HasKey(e => e.ProductoId).HasName("PK__Producto__A430AEA3454048B3");

            entity.ToTable("Producto");

            entity.Property(e => e.CodigoBarra)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.Categoria).WithMany(p => p.Productos)
                .HasForeignKey(d => d.CategoriaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Categoria_Producto");
        });

        modelBuilder.Entity<Stock>(entity =>
        {
            entity.HasKey(e => e.StockId).HasName("PK__Stock__2C83A9C2E9880D22");

            entity.ToTable("Stock");

            entity.HasIndex(e => e.ProductoId, "UQ__Stock__A430AEA202B8D8CB").IsUnique();

            entity.Property(e => e.CodigoBarra)
                .HasMaxLength(20)
                .IsUnicode(false);

            entity.HasOne(d => d.Producto).WithOne(p => p.Stock)
                .HasForeignKey<Stock>(d => d.ProductoId)
                .HasConstraintName("FK_Producto_Stock");
        });

        modelBuilder.Entity<Venta>(entity =>
        {
            entity.HasKey(e => e.VentaId).HasName("PK__Venta__5B4150ACBC445034");

            entity.Property(e => e.FechaVenta)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Total).HasColumnType("decimal(20, 2)");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
