namespace Tamaris.Domains.Admin
{
	public record RoleForUpdate : BaseDomain 
	{
		public string Id { get; set; } // role_id
		public string? RoleName { get; set; } // role_name
		public bool IsAdmin { get; set; } // is_admin
	}
}