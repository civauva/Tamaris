using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

using Tamaris.Entities.Admin;
using Tamaris.API.Infrastructure.Attributes;
using Tamaris.Domains.Authorization;

namespace Tamaris.API.Controllers
{
    [TamarisController(Endpoint = "")]
    [Produces("application/json")]
    public class Authorization2Controller : BaseController
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public Authorization2Controller(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(request.UserName);
                if (user == null)
                    return BadRequest("User does not exist");

                var singInResult = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);
                if (!singInResult.Succeeded)
                    return BadRequest("Invalid password");

                await _signInManager.SignInAsync(user, request.RememberMe);

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest($"Error happened during login process. {ex.InnerException?.Message ?? ex.Message}");
            }
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest parameters)
        {
            try
            {
                var user = new User
                {
                    UserName = parameters.UserName
                };
                var result = await _userManager.CreateAsync(user, parameters.Password);
                if (!result.Succeeded) return BadRequest(result.Errors.FirstOrDefault()?.Description);

                return await Login(new LoginRequest
                {
                    UserName = parameters.UserName,
                    Password = parameters.Password
                });
            }
            catch (Exception ex)
            {
                return BadRequest($"Error happened during register process. {ex.InnerException?.Message ?? ex.Message}");
            }
        }

        [Authorize]
        [HttpPost("Logout")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Ok();
        }

        [HttpGet("CurrentUserInfo")]
        public CurrentUser CurrentUserInfo()
        {
            return new CurrentUser
            {
                IsAuthenticated = User.Identity.IsAuthenticated,
                UserName = User.Identity.Name,
                Claims = User.Claims
                .ToDictionary(c => c.Type, c => c.Value)
            };
        }
    }
}