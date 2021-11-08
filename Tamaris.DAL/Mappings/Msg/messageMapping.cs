using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Tamaris.Entities.Msg;



namespace Tamaris.DAL.Mappings.Msg
{
	public class MessageMapping: IEntityTypeConfiguration<Message>
	{
		public void Configure(EntityTypeBuilder<Message> builder)
		{
			builder.ToTable("messages", "msg");
			builder.HasKey(x => x.Id);

			// Fields mapping
			builder.Property(x => x.Id).HasColumnName(@"message_id").HasColumnType("int").IsRequired().ValueGeneratedOnAdd(); // message_id
			builder.Property(x => x.SenderUserId).HasColumnName(@"sender_user_id").HasColumnType("nvarchar").HasMaxLength(450).IsRequired(); // sender_user_id
			builder.Property(x => x.ReceiverUserId).HasColumnName(@"receiver_user_id").HasColumnType("nvarchar").HasMaxLength(450).IsRequired(); // receiver_user_id
			builder.Property(x => x.MessageText).HasColumnName(@"message_text").HasColumnType("nvarchar").HasMaxLength(2000).IsRequired(); // message_text
			builder.Property(x => x.SentOn).HasColumnName(@"sent_on").HasColumnType("datetime").IsRequired(); // sent_on
			builder.Property(x => x.IsRead).HasColumnName(@"is_read").HasColumnType("bit").IsRequired(); // is_read

			// Foreign keys

			// *** One-to-many
			builder.HasOne(d => d.ReceiverUser)
				.WithMany(p => p.ReceivedMessages)
				.HasForeignKey(fk => fk.ReceiverUserId)
				.HasConstraintName("FK__messages__receiver_users")
				.OnDelete(DeleteBehavior.Restrict);
			builder.HasOne(d => d.SenderUser)
				.WithMany(p => p.SentMessages)
				.HasForeignKey(fk => fk.SenderUserId)
				.HasConstraintName("FK__messages__sender_users")
				.OnDelete(DeleteBehavior.Restrict);
		}
	}
}