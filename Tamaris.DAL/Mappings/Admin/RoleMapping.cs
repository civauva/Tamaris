using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Tamaris.Entities.Admin;



namespace Tamaris.DAL.Mappings.Admin
{
	public class RoleMapping: IEntityTypeConfiguration<Role>
	{
		public void Configure(EntityTypeBuilder<Role> builder)
		{
			builder.ToTable("roles", "admin");
			builder.HasKey(x => x.Id);

			// Fields mapping
			builder.Property(x => x.IsAdmin).HasColumnName(@"is_admin").HasColumnType("bit").IsRequired(); // is_admin
		}
	}
}