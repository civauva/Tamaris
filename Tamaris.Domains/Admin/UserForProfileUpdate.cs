namespace Tamaris.Domains.Admin
{
	public record UserForProfileUpdate : BaseDomain
	{
		public string Id { get; set; } // user_id
		public string FirstName { get; set; } // first name
		public string LastName { get; set; } // last name
		public string Email { get; set; } // email
		public string Company { get; set; } // company
		public string CurrentPassword { get; set; } // current password
		public string NewPassword { get; set; } // new password
		public byte[] Avatar { get; set; } // avatar
    }
}