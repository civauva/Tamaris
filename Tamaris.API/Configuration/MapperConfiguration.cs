using AutoMapper;

using Tamaris.Entities.Admin;
using Tamaris.Entities.Msg;

using Tamaris.Domains.Admin;
using Tamaris.Domains.Msg;


namespace Tamaris.API.Configuration
{
    public class MapperConfiguration: Profile
    {
        public MapperConfiguration()
        {
			// Admin
			CreateMap<RoleForInsert, Role>(AutoMapper.MemberList.Source);
			CreateMap<RoleForUpdate, Role>(AutoMapper.MemberList.Source);

			CreateMap<UserForInsert, User>(AutoMapper.MemberList.Source);
			CreateMap<UserForUpdate, User>(AutoMapper.MemberList.Source);


			// Msg
			CreateMap<MessageForInsert, Message>(AutoMapper.MemberList.Source);
			CreateMap<MessageForUpdate, Message>(AutoMapper.MemberList.Source);
        }
    }
}