namespace Tamaris.Domains.Msg
{
	public record MessageForSelect : BaseDomain 
	{
		public int Id { get; set; } // message_id
		public string SenderUserId { get; set; } // sender_user_id
		public string ReceiverUserId { get; set; } // receiver_user_id
		public string? MessageText { get; set; } // message_text
		public DateTime SentOn { get; set; } // sent_on
		public bool IsRead { get; set; } // is_read
	}
}