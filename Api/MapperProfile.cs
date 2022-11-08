using Api.Models;
using Api.Models.Attach;
using Api.Models.Post;
using Api.Models.User;
using AutoMapper;
using Common;

namespace Api
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<CreateUserModel, DAL.Entities.User>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()))
                .ForMember(dest => dest.PasswordHash, opt => opt.MapFrom(src => HashHelper.GetHash(src.Password)))
                .ForMember(dest => dest.BirthDate, opt => opt.MapFrom(src => src.BirthDate.UtcDateTime));

            CreateMap<DAL.Entities.User, UserModel>();
            CreateMap<DAL.Entities.Avatar, AttachModel>();
            CreateMap<DAL.Entities.Post, PostModel>();
            CreateMap<DAL.Entities.PostAttach, PostAttachModel>()
                .ForMember(dest => dest.Url, opt => opt.MapFrom(src=> "/api/Post/GetPostAttach?id=" + src.Id));
            CreateMap<DAL.Entities.PostComment, CommentModel>();
        }
    }
}
