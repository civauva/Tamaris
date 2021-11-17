using FluentValidation;
using Tamaris.Domains.Admin;

namespace Tamaris.Domains.Validators.Admin
{
	public class UserForInsertValidator: AbstractValidator<UserForInsert>
	{
		public UserForInsertValidator()
		{
			RuleFor(x => x.Username).MinimumLength(5);
			RuleFor(x => x.Email).MinimumLength(7).EmailAddress();
			RuleFor(x => x.Company).NotEmpty();
		}
	}
}