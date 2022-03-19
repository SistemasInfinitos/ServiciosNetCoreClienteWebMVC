using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using ServiciosNetCore.ModelsDB;

#nullable disable

namespace ServiciosNetCore.ModelsDB.Contexts
{
    public partial class Context : DbContext
    {
        public Context()
        {
        }

        public Context(DbContextOptions<Context> options)
            : base(options)
        {
        }

        public virtual DbSet<DetallePedido> DetallePedidos { get; set; }
        public virtual DbSet<EncabezadoPedido> EncabezadoPedidos { get; set; }
        public virtual DbSet<Producto> Productos { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server=SISTEMASINFINIT\\SERVERDESARROLLO;Database=DesarrolladorSemiSenior;user=sa;Password=YoSoyJovenDeCristo*;trustServerCertificate=true;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "Modern_Spanish_CI_AS");

            modelBuilder.Entity<DetallePedido>(entity =>
            {
                entity.Property(e => e.cantidad).HasColumnType("decimal(18, 4)");

                entity.Property(e => e.fechaActualizacion).HasColumnType("datetime");

                entity.Property(e => e.fechaCreacion)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.porcentajeIva).HasColumnType("decimal(18, 4)");

                entity.Property(e => e.valorUnitario).HasColumnType("decimal(18, 4)");

                entity.HasOne(d => d.encabezadoPedidos)
                    .WithMany(p => p.DetallePedidoes)
                    .HasForeignKey(d => d.encabezadoPedidosId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("DetallePedidosEncabezadoPedidos");

                entity.HasOne(d => d.producto)
                    .WithMany(p => p.DetallePedidoes)
                    .HasForeignKey(d => d.productoId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("DetallePedidosProductosId");
            });

            modelBuilder.Entity<EncabezadoPedido>(entity =>
            {
                entity.Property(e => e.fechaActualizacion).HasColumnType("datetime");

                entity.Property(e => e.fechaCreacion)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.valorIva).HasColumnType("decimal(18, 4)");

                entity.Property(e => e.valorNeto).HasColumnType("decimal(18, 4)");

                entity.Property(e => e.valorTotal).HasColumnType("decimal(18, 4)");
            });

            modelBuilder.Entity<Producto>(entity =>
            {
                entity.Property(e => e.descripcion)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.fechaActualizacion).HasColumnType("datetime");

                entity.Property(e => e.fechaCreacion)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.valorUnitario).HasColumnType("decimal(18, 4)");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
