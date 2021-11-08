using System.Text;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

using Tamaris.Entities.Admin;
using Tamaris.API.Infrastructure.Attributes;
using Tamaris.Domains.Authorization;
using Tamaris.API.Services.Email.Interfaces;
using Tamaris.API.Services.Email;

namespace Tamaris.API.Controllers
{
    [TamarisController(Endpoint = "")]
    [Produces("application/json")]
    public class AuthorizationController : BaseController
    {
        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _configuration;
        private readonly IConfigurationSection _jwtSettings;
        private readonly IEmailSender _emailSender;

        public AuthorizationController(UserManager<User> userManager, IConfiguration configuration, IEmailSender emailSender)
        {
            _userManager = userManager;
            _configuration = configuration;
            _jwtSettings = _configuration.GetSection("JwtSettings");
            _emailSender = emailSender;
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest requestModel)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(requestModel.UserName);
                if (user == null)
                    return BadRequest("User does not exist");

                if( !await _userManager.CheckPasswordAsync(user, requestModel.Password) )
                    return Unauthorized(new AuthorizationResponse { ErrorMessage = "Invalid Authentication" });

                var signingCredentials = GetSigningCredentials();
                var claims = GetClaims(user);
                var tokenOptions = GenerateTokenOptions(signingCredentials, claims);
                var token = new JwtSecurityTokenHandler().WriteToken(tokenOptions);

                return Ok(new AuthorizationResponse { IsAuthorized = true, Token = token });
            }
            catch (Exception ex)
            {
                return BadRequest($"Error happened during login process. {ex.InnerException?.Message ?? ex.Message}");
            }
        }



        #region Helper methods
        private SigningCredentials GetSigningCredentials()
        {
            var key = Encoding.UTF8.GetBytes(_jwtSettings.GetSection("securityKey").Value);
            var secret = new SymmetricSecurityKey(key);

            return new SigningCredentials(secret, SecurityAlgorithms.HmacSha256);
        }

        private List<Claim> GetClaims(IdentityUser user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Email ?? user.UserName)
            };

            return claims;
        }

        private JwtSecurityToken GenerateTokenOptions(SigningCredentials signingCredentials, List<Claim> claims)
        {
            var tokenOptions = new JwtSecurityToken(
                issuer: _jwtSettings.GetSection("validIssuer").Value,
                audience: _jwtSettings.GetSection("validAudience").Value,
                claims: claims,
                expires: DateTime.Now.AddMinutes(Convert.ToDouble(_jwtSettings.GetSection("expiryInMinutes").Value)),
                signingCredentials: signingCredentials);

            return tokenOptions;
        }
        #endregion Helper methods


        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest requestModel)
        {
            try
            {
                if (requestModel == null)
                    return BadRequest();

                var user = new User
                {
                    UserName = requestModel.UserName,
                    Email = requestModel.Email
                };

                var result = await _userManager.CreateAsync(user, requestModel.Password);
                if (!result.Succeeded)
                {
                    var errors = result.Errors.Select(e => e.Description);

                    return BadRequest(new RegisterResponse { Errors = errors });
                }

                return await Login(new LoginRequest
                {
                    UserName = requestModel.UserName,
                    Password = requestModel.Password
                });
            }
            catch (Exception ex)
            {
                return BadRequest($"Error happened during register process. {ex.InnerException?.Message ?? ex.Message}");
            }
        }


        [HttpPost("ForgotPassword")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest requestModel)
        {
            if (requestModel == null)
                return BadRequest("Invalid request.");

            var user = await _userManager.FindByEmailAsync(requestModel.Email);
            if (user == null)
                return BadRequest("No user with provided email address.");

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            //var callback = Url.Action(nameof(ResetPassword), "Account", new { token, email = user.Email }, Request.Scheme);

            var link = $"{requestModel.Callback}/{requestModel.Email}/{token}";

            var body = $"Please, click <a href=\"{link}\">here</a> to reset your password.";
            var message = new EmailMessage(new string[] { user.Email }, "Tamaris reset password token", body, null);
            await _emailSender.SendEmailAsync(message);

            return Ok();
        }


        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest requestModel)
        {
            if (requestModel == null)
                return BadRequest("Invalid request.");

            var user = await _userManager.FindByEmailAsync(requestModel.Email);
            if (user == null)
                return BadRequest("No user with provided email address.");

            var resetPassResult = await _userManager.ResetPasswordAsync(user, requestModel.Token, requestModel.Password);
            if (!resetPassResult.Succeeded)
            {
                var errors = "";

                foreach (var error in resetPassResult.Errors)
                    errors += error.Description;

                return BadRequest(errors);
            }

            return Ok();
        }
    }
}