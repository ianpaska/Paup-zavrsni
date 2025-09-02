using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;

namespace GameShop3.Models
{
    public class ApplicationUser : IdentityUser
    {
        // Ovdje ostaje kako je bilo, možeš kasnije dodati custom polja
    }

    // KONTEKST BAZE
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public DbSet<Game> Games { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Comment> Comments { get; set; }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }

    // SEED ADMIN
    public static class IdentitySeed
    {
        public static void SeedAdmin(ApplicationDbContext context)
        {
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));

            // Kreiraj Admin rolu ako ne postoji
            if (!roleManager.RoleExists("Admin"))
            {
                roleManager.Create(new IdentityRole("Admin"));
            }

            string adminEmail = "admin@gmail.com";
            string adminPassword = "123123";

            var adminUser = userManager.FindByEmail(adminEmail);

            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true
                };

                var result = userManager.Create(adminUser, adminPassword);
                if (result.Succeeded)
                {
                    userManager.AddToRole(adminUser.Id, "Admin");
                    context.SaveChanges();
                }
                else
                {
                    // Debug – da vidiš zašto nije uspjelo
                    foreach (var err in result.Errors)
                    {
                        System.Diagnostics.Debug.WriteLine("Seed error: " + err);
                    }
                }
            }
        }
    }
}
