using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace LaVentaMusical.Models
{
    public class ApplicationUser : IdentityUser
    {
        // Campo extra que vamos a usar (opcional)
        public string FullName { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext() : base("DefaultConnection", throwIfV1Schema: false) { }
        public static ApplicationDbContext Create() => new ApplicationDbContext();

        // Solo entidades manuales, no las generadas por EF
        public DbSet<Perfil> Perfiles { get; set; }
        public DbSet<Genero> Generos { get; set; }
        public DbSet<Artista> Artistas { get; set; }
        public DbSet<Album> Albumes { get; set; }
    }
}
