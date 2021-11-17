using FluentValidation;
using Tamaris.Domains.Admin;

namespace Tamaris.Domains.Validators.Admin
{
	public class UserForUpdateValidator: AbstractValidator<UserForUpdate>
	{
		public UserForUpdateValidator()
		{
			RuleFor(x => x.Username).MinimumLength(5);
			RuleFor(x => x.Email).MinimumLength(7).EmailAddress();
		}
	}
}