namespace Tamaris.Domains.Authorization
{
    public record RegisterResponse
    {
        public bool IsRegistrationSuccessful { get; set; }
        public IEnumerable<string> Errors { get; set; }
    }
}
