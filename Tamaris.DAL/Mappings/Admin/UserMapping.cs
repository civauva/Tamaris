using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Tamaris.Entities.Admin;



namespace Tamaris.DAL.Mappings.Admin
{
	public class UserMapping: IEntityTypeConfiguration<User>
	{
		public void Configure(EntityTypeBuilder<User> builder)
		{
			builder.ToTable("users", "admin");
			builder.HasKey(x => x.Id);

			// Fields mapping
			// builder.Property(x => x.Id).HasColumnName(@"user_id").HasColumnType("int").IsRequired().ValueGeneratedOnAdd(); // user_id
			builder.Property(x => x.FirstName).HasColumnName(@"first_name").HasColumnType("nvarchar").HasMaxLength(50).IsRequired(false); // first_name
			builder.Property(x => x.LastName).HasColumnName(@"last_name").HasColumnType("nvarchar").HasMaxLength(50).IsRequired(false); // last_name
			builder.Property(x => x.Company).HasColumnName(@"company").HasColumnType("nvarchar").HasMaxLength(50).IsRequired(false); // company
			builder.Property(x => x.Avatar).HasColumnName(@"avatar").HasColumnType("image").IsRequired(false); // avatar
		}
	}
}