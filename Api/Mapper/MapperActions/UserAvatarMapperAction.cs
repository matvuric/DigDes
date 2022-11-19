using Api.Models.User;
using Api.Services;
using AutoMapper;
using DAL.Entities;

namespace Api.Mapper.MapperActions
{
    public class UserAvatarMapperAction : IMappingAction<User, UserAvatarModel>
    {
        private readonly LinkGeneratorService _linkGeneratorService;

        public UserAvatarMapperAction(LinkGeneratorService linkGeneratorService)
        {
            _linkGeneratorService = linkGeneratorService;
        }

        public void Process(User source, UserAvatarModel destination, ResolutionContext context)
        {
            _linkGeneratorService.FixAvatar(source, destination);
        }
    }
}
