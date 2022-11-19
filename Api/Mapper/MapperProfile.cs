using Api.Mapper.MapperActions;
using Api.Models.Attachment;
using Api.Models.Post;
using Api.Models.User;
using AutoMapper;
using Common;
using DAL.Entities;

namespace Api.Mapper
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<CreateUserModel, User>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()))
                .ForMember(dest => dest.PasswordHash, opt => opt.MapFrom(src => HashHelper.GetHash(src.Password)))
                .ForMember(dest => dest.BirthDate, opt => opt.MapFrom(src => src.BirthDate.UtcDateTime));

            CreateMap<User, UserModel>();
            CreateMap<User, UserAvatarModel>()
                .ForMember(dest => dest.PostsCount, opt => opt.MapFrom(src => src.Posts!.Count))
                .AfterMap<UserAvatarMapperAction>();

            CreateMap<Avatar, AttachmentModel>();

            CreateMap<PostAttachment, AttachmentModel>();
            CreateMap<PostAttachment, AttachmentExternalModel>().AfterMap<PostAttachmentMapperAction>();

            CreateMap<CreatePostModel, PostModel>();

            CreateMap<MetadataModel, PostAttachmentModel>();

            CreateMap<PostAttachmentModel, PostAttachment>();

            CreateMap<PostModel, Post>()
                .ForMember(dest => dest.PostAttachments, opt => opt.MapFrom(src => src.PostAttachments))
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => DateTime.UtcNow));

            CreateMap<Post, ReturnPostModel>();
        }
    }
}
