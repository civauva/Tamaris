namespace Tamaris.Domains.Msg
{
	public class MessageForInsert : BaseDomain 
	{
		public MessageForInsert()
		{
			Subject = "";
		}

		public string SenderUsername { get; set; } // sender_username
		public string ReceiverUsername { get; set; } // receiver_username
		public string Subject { get; set; } // subject
		public string? MessageText { get; set; } // message_text
	}
}