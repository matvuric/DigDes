using Api.Models.User;
using Api.Services;
using AutoMapper;
using DAL.Entities;

namespace Api.Mapper.MapperActions
{
    public class PostUserAvatarMapperAction : IMappingAction<User, PostUserModel>
    {
        private readonly LinkGeneratorService _linkGeneratorService;

        public PostUserAvatarMapperAction(LinkGeneratorService linkGeneratorService)
        {
            _linkGeneratorService = linkGeneratorService;
        }

        public void Process(User source, PostUserModel destination, ResolutionContext context)
        {
            _linkGeneratorService.FixPostUserAvatar(source, destination);
        }
    }
}