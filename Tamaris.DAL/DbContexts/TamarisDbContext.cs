using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

using Tamaris.Entities.Admin;
using Tamaris.Entities.Msg;
using Tamaris.DAL.Mappings.Admin;
using Tamaris.DAL.Mappings.Msg;

namespace Tamaris.DAL.DbContexts
{
	public class TamarisDbContext: IdentityDbContext<User, Role, string>
	{
		public TamarisDbContext(DbContextOptions<TamarisDbContext> options) : base(options) { }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			try
			{
				// Customizations must go after base.OnModelCreating(builder)
				#region Admin
				modelBuilder.ApplyConfiguration(new UserMapping());
				modelBuilder.ApplyConfiguration(new RoleMapping());

				IdentityTablesMapping.MapTables(modelBuilder);
				IdentityTablesMapping.SeedData(modelBuilder);

				#endregion Admin

				#region Msg
				modelBuilder.ApplyConfiguration( new MessageMapping() );
				#endregion Msg


				// Imagine a ton more customizations
			}
			catch (Exception ex)
			{
				throw new Exception("Error while setting up Database Context.", ex);
			}
		}

		#region Admin
		public DbSet<Role> Roles { get; set; }
		public DbSet<User> Users { get; set; }
		#endregion Admin

		#region Msg
		public DbSet<Message> Messages { get; set; }
		#endregion Msg


	}
}