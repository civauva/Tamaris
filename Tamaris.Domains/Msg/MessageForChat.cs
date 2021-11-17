namespace Tamaris.Domains.Msg
{
	public record MessageForChat : BaseDomain 
	{
		public int Id { get; set; } // message_id
        public string SenderUsername { get; set; }
        public string SenderFirstName { get; set; }
        public string SenderLastName { get; set; }
        public string SenderFullName => $"{SenderLastName} {SenderFirstName}";
        public byte[] SenderAvatar { get; set; }

        /// <summary>
        /// Read-only property that can be used as a source
        /// for the <img> elements.
        /// </summary>
        public string SenderAvatarSource => GetAvatarSource(SenderAvatar);

        public string ReceiverUsername { get; set; }
        public string ReceiverFirstName { get; set; }
        public string ReceiverLastName { get; set; }
        public string ReceiverFullName => $"{ReceiverLastName} {ReceiverFirstName}";
        public byte[] ReceiverAvatar { get; set; }
        /// <summary>
        /// Read-only property that can be used as a source
        /// for the <img> elements.
        /// </summary>
        public string ReceiverAvatarSource => GetAvatarSource(ReceiverAvatar);
        
        public string? MessageText { get; set; } // message_text
		public DateTime SentOn { get; set; } // sent_on
		public bool IsRead { get; set; } // is_read

        private static string GetAvatarSource(byte[] avatar)
        {
            if (avatar == null || avatar.Length == 0)
                return "";
            else
            {
                var convertedArray = Convert.ToBase64String(avatar);
                var thumbnail = $"data:image/jpg;base64,{convertedArray}";
                return thumbnail;
            }
        }

    }
}