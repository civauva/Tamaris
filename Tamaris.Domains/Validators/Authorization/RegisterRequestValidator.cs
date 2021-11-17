using FluentValidation;
using Tamaris.Domains.Authorization;

namespace Tamaris.Domains.Validators.Authorization
{
	public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
	{
		public RegisterRequestValidator()
		{
			RuleFor(x => x.UserName).NotEmpty().NotNull().MinimumLength(5);
			RuleFor(x => x.Email).NotEmpty().NotNull().MinimumLength(7).EmailAddress();
		}
	}
}
