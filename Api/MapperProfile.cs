using Api.Models.Attachment;
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
            CreateMap<DAL.Entities.User, UserAvatarModel>();

            CreateMap<DAL.Entities.Avatar, AttachmentModel>();

            CreateMap<DAL.Entities.PostAttachment, AttachmentModel>();
            CreateMap<DAL.Entities.PostAttachment, AttachmentExternalModel>();

            CreateMap<CreatePostModel, PostModel>();

            CreateMap<MetadataModel, PostAttachmentModel>();

            CreateMap<PostAttachmentModel, DAL.Entities.PostAttachment>();

            CreateMap<PostModel, DAL.Entities.Post>()
                .ForMember(dest => dest.PostAttachments, opt => opt.MapFrom(src => src.PostAttachments))
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => DateTime.UtcNow));
        }
    }
}
