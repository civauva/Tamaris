using System;
using Tamaris.Domains.Authorization;

namespace Tamaris.Web.Services.DataService
{
    public interface IAccountDataService
    {
        Task<AuthorizationResponse> Login(LoginRequest loginRequest);
        Task<RegisterResponse> Register(RegisterRequest registerRequest);
        Task<string> ForgotPassword(ForgotPasswordRequest forgotPasswordRequest);
        Task<string> ResetPassword(ResetPasswordRequest resetPasswordRequest);
        Task<string> GetUsernameAsync();
        Task Logout();
    }
}