using Tamaris.Domains.Admin;

namespace Tamaris.Web.Models
{
    public record RoleForCheck : RoleForSelect
    {
        public bool IsChecked { get; set; }
    }
}
