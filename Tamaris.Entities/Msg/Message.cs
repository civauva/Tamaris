using Tamaris.Entities.Admin;



namespace Tamaris.Entities.Msg
{
	/// <summary>
	/// msg.messages
	/// </summary>
	public class Message : BaseEntity
	{
		public Message()
		{
		}

		/// <summary>
		/// message_id
		/// </summary>
		public int Id { get; set; }
		/// <summary>
		/// sender_user_id
		/// </summary>
		public string SenderUserId { get; set; }
		/// <summary>
		/// receiver_user_id
		/// </summary>
		public string ReceiverUserId { get; set; }
		/// <summary>
		/// subject
		/// </summary>
		public string Subject { get; set; }
		/// <summary>
		/// message_text
		/// </summary>
		public string MessageText { get; set; }
		/// <summary>
		/// sent_on
		/// </summary>
		public DateTime SentOn { get; set; }
		/// <summary>
		/// is_read
		/// </summary>
		public bool IsRead { get; set; }

		// Foreign keys
		public virtual User ReceiverUser { get; set; } // FK__messages__receiver_users
		public virtual User SenderUser { get; set; } // FK__messages__sender_users
	}
}