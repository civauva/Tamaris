using System.ComponentModel.DataAnnotations;

namespace Tamaris.Domains.Authorization
{
    public record LoginRequest
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        public string Password { get; set; }

        public bool RememberMe { get; set; }
    }
}

