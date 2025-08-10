using System.Data.Entity;

namespace LaVentaMusical.Models
{
    public class PAV_PF_Grupo02Entities : DbContext
    {
        public PAV_PF_Grupo02Entities() 
            : base("PAV_PF_Grupo02Connection")
        {
            // Configurar para que no inicialice automáticamente la base de datos
            Database.SetInitializer<PAV_PF_Grupo02Entities>(null);
        }

        public virtual DbSet<Perfil> Perfiles { get; set; }
        public virtual DbSet<Usuario> Usuarios { get; set; }
        public virtual DbSet<Genero> Generos { get; set; }
        public virtual DbSet<Artista> Artistas { get; set; }
        public virtual DbSet<Album> Albumes { get; set; }
        public virtual DbSet<Cancion> Canciones { get; set; }
        public virtual DbSet<Venta> Ventas { get; set; }
        public virtual DbSet<DetalleVenta> DetalleVenta { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // Configuraciones adicionales

            // Configurar precisión decimal para campos monetarios
            modelBuilder.Entity<Usuario>()
                .Property(u => u.DineroDisponible)
                .HasPrecision(10, 2);

            modelBuilder.Entity<Cancion>()
                .Property(c => c.Precio)
                .HasPrecision(10, 2);

            modelBuilder.Entity<Venta>()
                .Property(v => v.Subtotal)
                .HasPrecision(10, 2);

            modelBuilder.Entity<Venta>()
                .Property(v => v.IVA)
                .HasPrecision(10, 2);

            modelBuilder.Entity<Venta>()
                .Property(v => v.ComisionTarjeta)
                .HasPrecision(10, 2);

            modelBuilder.Entity<Venta>()
                .Property(v => v.Total)
                .HasPrecision(10, 2);

            modelBuilder.Entity<Venta>()
                .Property(v => v.DineroUtilizado)
                .HasPrecision(10, 2);

            modelBuilder.Entity<DetalleVenta>()
                .Property(d => d.PrecioUnitario)
                .HasPrecision(10, 2);

            modelBuilder.Entity<DetalleVenta>()
                .Property(d => d.Subtotal)
                .HasPrecision(10, 2);

            // Configurar relaciones
            modelBuilder.Entity<Usuario>()
                .HasRequired(u => u.Perfil)
                .WithMany()
                .HasForeignKey(u => u.PerfilID);

            modelBuilder.Entity<Album>()
                .HasRequired(a => a.Artista)
                .WithMany()
                .HasForeignKey(a => a.ArtistaID);

            modelBuilder.Entity<Cancion>()
                .HasRequired(c => c.Genero)
                .WithMany()
                .HasForeignKey(c => c.GeneroID);

            modelBuilder.Entity<Cancion>()
                .HasRequired(c => c.Album)
                .WithMany()
                .HasForeignKey(c => c.AlbumID);

            modelBuilder.Entity<Venta>()
                .HasRequired(v => v.Usuario)
                .WithMany()
                .HasForeignKey(v => v.UsuarioID);

            modelBuilder.Entity<DetalleVenta>()
                .HasRequired(d => d.Venta)
                .WithMany(v => v.Detalles)
                .HasForeignKey(d => d.VentaID);

            modelBuilder.Entity<DetalleVenta>()
                .HasRequired(d => d.Cancion)
                .WithMany()
                .HasForeignKey(d => d.CancionID);

            base.OnModelCreating(modelBuilder);
        }
    }
}