namespace Tamaris.Domains.Authorization
{
    public record AuthorizationResponse
    {
        public bool IsAuthorized { get; set; }
        public string ErrorMessage { get; set; }
        public string Token { get; set; }
    }
}