namespace Tamaris.Domains.Admin
{
	public class UserForUpdate : BaseDomain
	{
		public string Id { get; set; } // user_id
		public string Username { get; set; } // username
		public string FirstName { get; set; } // first name
		public string LastName { get; set; } // last name
		public string Email { get; set; } // email
		public string Company { get; set; } // company
		public byte[] Avatar { get; set; } // avatar
	}
}