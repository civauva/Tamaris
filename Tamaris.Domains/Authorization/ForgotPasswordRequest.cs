using System.ComponentModel.DataAnnotations;

namespace Tamaris.Domains.Authorization
{
    public class ForgotPasswordRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Callback { get; set; }
    }
}
