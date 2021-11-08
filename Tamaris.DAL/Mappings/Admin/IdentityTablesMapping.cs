using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Tamaris.Entities.Admin;

namespace Tamaris.DAL.Mappings.Admin
{
    internal class IdentityTablesMapping
    {
        /// <summary>
        /// This method maps standard Identity EF tables to our own custom names
        /// </summary>
        /// <param name="builder"></param>
        internal static void MapTables(ModelBuilder builder)
        {
            builder.Entity<IdentityUserRole<string>>().ToTable("user_roles", "admin");   // AspNetUserRole
            builder.Entity<IdentityUserClaim<string>>().ToTable("user_claims", "admin"); // AspNetUserClaim
            builder.Entity<IdentityUserLogin<string>>().ToTable("user_logins", "admin"); // AspNetUserLogin
            builder.Entity<IdentityRoleClaim<string>>().ToTable("role_claims", "admin"); // AspNetRoleClaim
            builder.Entity<IdentityUserToken<string>>().ToTable("user_tokens", "admin"); // AspNetUserToken
        }

        /// <summary>
        /// This method is responsible for creating default roles: administrators and standard users
        /// </summary>
        /// <param name="modelBuilder"></param>
        internal static void SeedData(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Role>()
                .HasData
                (
                    new Role { Id = Guid.NewGuid().ToString(), Name = "Administrators", IsAdmin = true },
                    new Role { Id = Guid.NewGuid().ToString(), Name = "Standard users", IsAdmin = false }
                );
        }
    }
}