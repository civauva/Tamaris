using System;
using System.Collections.Generic;



namespace Tamaris.Domains.Admin
{
	public class RoleForInsert : BaseDomain 
	{
		public RoleForInsert()
		{
		}

		public string? RoleName { get; set; } // role_name
		public bool IsAdmin { get; set; } // is_admin
	}
}