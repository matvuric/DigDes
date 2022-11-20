using Api.Mapper.MapperActions;
using Api.Models.Attachment;
using Api.Models.Post;
using Api.Models.PostComment;
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
            CreateMap<MetadataModel, PostCommentAttachmentModel>();

            CreateMap<PostAttachmentModel, PostAttachment>();

            CreateMap<PostModel, Post>()
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => DateTime.UtcNow));

            CreateMap<Post, ReturnPostModel>()
                .ForMember(dest => dest.CommentsCount, opt => opt.MapFrom(src => src.Comments!.Count));
            CreateMap<Post, ReturnPostWithCommentsModel>()
                .ForMember(dest => dest.PostComments, opt => opt.MapFrom(src => src.Comments));

            CreateMap<PostCommentAttachment, AttachmentModel>();
            CreateMap<PostCommentAttachment, AttachmentExternalModel>().AfterMap<PostCommentAttachmentMapperAction>();

            CreateMap<CreatePostCommentModel, PostCommentModel>();

            CreateMap<PostCommentAttachmentModel, PostCommentAttachment>();

            CreateMap<PostCommentModel, PostComment>()
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => DateTime.UtcNow));

            CreateMap<PostComment, ReturnPostCommentModel>();
        }
    }
}
