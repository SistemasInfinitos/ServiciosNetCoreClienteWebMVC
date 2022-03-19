using Microsoft.EntityFrameworkCore;

#nullable disable

namespace ServicioPersonas.ModelsDB.Contexts
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

        public virtual DbSet<Persona> Personas { get; set; }
        public virtual DbSet<Usuario> Usuarios { get; set; }

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

            modelBuilder.Entity<Persona>(entity =>
            {
                entity.Property(e => e.apellidos)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.fechaActualizacion).HasColumnType("datetime");

                entity.Property(e => e.fechaCreacion)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.nombres)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.HasIndex(e => e.nombreUsuario, "UQ_Usuarios")
                    .IsUnique();

                entity.Property(e => e.fechaActualizacion).HasColumnType("datetime");

                entity.Property(e => e.fechaCreacion)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.nombreUsuario)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.passwordHash).IsRequired();
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
