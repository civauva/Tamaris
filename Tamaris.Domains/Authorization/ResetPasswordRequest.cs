using System.ComponentModel.DataAnnotations;

namespace Tamaris.Domains.Authorization
{
    public class ResetPasswordRequest
    {
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string PasswordConfirm { get; set; }

        public string Email { get; set; }
        public string Token { get; set; }
    }
}
