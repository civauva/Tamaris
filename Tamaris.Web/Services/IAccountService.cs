using System;
using Tamaris.Domains.Authorization;

namespace Tamaris.Web.Services
{
    public interface IAccountService
    {
        Task<AuthorizationResponse> Login(LoginRequest loginRequest);
        Task<RegisterResponse> Register(RegisterRequest registerRequest);
        Task<string> ForgotPassword(ForgotPasswordRequest forgotPasswordRequest);
        Task<string> ResetPassword(ResetPasswordRequest resetPasswordRequest);
        Task Logout();
    }
}