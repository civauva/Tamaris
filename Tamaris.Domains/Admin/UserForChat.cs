namespace Tamaris.Domains.Admin
{
	public class UserForChat : BaseDomain
	{
        public UserForChat()
        {
            // Temporarily we are setting the online properties
            // Just some random functions :-)
            // TODO: This should happen somewhere else, of course :-)
            IsOnline = (new Random()).Next() > (Int32.MaxValue / 2);
            OnlineStatus = IsOnline ? "online" : "left 10 hours ago";
        }

		public string Username { get; set; } // username
		public string FirstName { get; set; } // first name
		public string LastName { get; set; } // last name
        public string FullName => $"{LastName} {FirstName}";
        public string Email { get; set; } // email
		public byte[] Avatar { get; set; } // avatar
        public int UnreadCount { get; set; }

        /// <summary>
        /// Read-only property that can be used as a source
        /// for the <img> elements.
        /// </summary>
        public string AvatarSource
        {
            get
            {
                if (Avatar == null || Avatar.Length == 0)
                    return "";
                else
                {
                    var convertedArray = Convert.ToBase64String(Avatar);
                    var thumbnail = $"data:image/jpg;base64,{convertedArray}";
                    return thumbnail;
                }
            }
        }

        public bool IsOnline { get; set; } // is_online
		public DateTime LastSeenOn { get; set; }
        public string OnlineStatus { get; set; }
    }
}