namespace Tamaris.Domains.Admin
{
	public record RoleForInsert : BaseDomain 
	{
		public string? RoleName { get; set; } // role_name
		public bool IsAdmin { get; set; } // is_admin
	}
}