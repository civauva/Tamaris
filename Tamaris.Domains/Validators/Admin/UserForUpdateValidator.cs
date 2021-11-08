using FluentValidation;
using Tamaris.Domains.Admin;

namespace Tamaris.Domains.Validators.Admin
{
	public class UserForUpdateValidator: AbstractValidator<UserForUpdate>
	{
		public UserForUpdateValidator()
		{

		}
	}
}