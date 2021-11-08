namespace Tamaris.Domains.Authorization
{
    public class RegisterResponse
    {
        public bool IsRegistrationSuccessful { get; set; }
        public IEnumerable<string> Errors { get; set; }
    }
}
