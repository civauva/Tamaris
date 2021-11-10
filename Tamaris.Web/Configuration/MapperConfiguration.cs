using AutoMapper;
using Tamaris.Domains.Admin;
using Tamaris.Web.Models;

namespace Tamaris.Web.Configuration
{
	public class MapperConfiguration : Profile
	{
		public MapperConfiguration()
		{
			// Admin
			CreateMap<RoleForInsert, RoleForSelect>(AutoMapper.MemberList.Source);
			CreateMap<RoleForUpdate, RoleForSelect>(AutoMapper.MemberList.Source);
			CreateMap<RoleForSelect, RoleForCheck>(AutoMapper.MemberList.Source);

			CreateMap<UserForSelect, UserForInsert>(AutoMapper.MemberList.Source);
			CreateMap<UserForSelect, UserForUpdate>(AutoMapper.MemberList.Source);
		}
	}
}