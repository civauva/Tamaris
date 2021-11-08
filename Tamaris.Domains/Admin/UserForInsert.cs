namespace Tamaris.Domains.Admin
{
	public class UserForInsert : BaseDomain
	{
		public string Username { get; set; } // username
		public string FirstName { get; set; } // first_name
		public string LastName { get; set; } // last_name
		public string Email { get; set; } // email
		public string Password { get; set; } // password
		public string Company { get; set; } // company
		public byte[] Avatar { get; set; } // avatar
	}
}