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

        // Use the EDMX-generated entities (plural names)
        public DbSet<Perfiles> Perfiles { get; set; }
        public DbSet<Generos> Generos { get; set; }
        public DbSet<Artistas> Artistas { get; set; }
        public DbSet<Albumes> Albumes { get; set; }
    }
}
