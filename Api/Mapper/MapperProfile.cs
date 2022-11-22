using Api.Mapper.MapperActions;
using Api.Models.Attachment;
using Api.Models.Follow;
using Api.Models.Like;
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
                .ForMember(dest => dest.FollowersCount, opt => opt.MapFrom(src => src.Followers!.Count))
                .ForMember(dest => dest.FollowingsCount, opt => opt.MapFrom(src => src.Following!.Count))
                .ForMember(dest => dest.LikesCount, opt => opt.MapFrom(src => src.Likes!.Count(like => like.IsPositive)))
                .ForMember(dest => dest.DislikesCount, opt => opt.MapFrom(src => src.Likes!.Count(like => !like.IsPositive)))
                .AfterMap<UserAvatarMapperAction>();
            CreateMap<User, PostUserModel>()
                .ForMember(dest => dest.FollowersCount, opt => opt.MapFrom(src => src.Followers!.Count))
                .ForMember(dest => dest.FollowingsCount, opt => opt.MapFrom(src => src.Following!.Count))
                .AfterMap<PostUserAvatarMapperAction>();

            CreateMap<Avatar, AttachmentModel>();

            CreateMap<PostAttachment, AttachmentModel>();
            CreateMap<PostAttachment, AttachmentExternalModel>().AfterMap<PostAttachmentMapperAction>();

            CreateMap<CreatePostModel, PostModel>();


            CreateMap<MetadataModel, MetadataLinkModel>();

            CreateMap<MetadataLinkModel, Avatar>();
            CreateMap<MetadataLinkModel, PostAttachment>();
            CreateMap<MetadataLinkModel, PostCommentAttachment>();

            CreateMap<PostModel, Post>()
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => DateTime.UtcNow));

            CreateMap<Post, ReturnPostModel>()
                .ForMember(dest => dest.CommentsCount, opt => opt.MapFrom(src => src.Comments!.Count))
                .ForMember(dest => dest.LikesCount, opt => opt.MapFrom(src => src.Likes!.Count(like => like.IsPositive)))
                .ForMember(dest => dest.DislikesCount, opt => opt.MapFrom(src => src.Likes!.Count(like => !like.IsPositive)));

            CreateMap<Post, ReturnPostWithCommentsModel>()
                .ForMember(dest => dest.PostComments, opt => opt.MapFrom(src => src.Comments))
                .ForMember(dest => dest.LikesCount, opt => opt.MapFrom(src => src.Likes!.Count(like => like.IsPositive)))
                .ForMember(dest => dest.DislikesCount, opt => opt.MapFrom(src => src.Likes!.Count(like => !like.IsPositive)));

            CreateMap<PostCommentAttachment, AttachmentModel>();
            CreateMap<PostCommentAttachment, AttachmentExternalModel>().AfterMap<PostCommentAttachmentMapperAction>();

            CreateMap<CreatePostCommentModel, PostCommentModel>();

            

            CreateMap<PostCommentModel, PostComment>()
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => DateTime.UtcNow));

            CreateMap<PostComment, ReturnPostCommentModel>()
                .ForMember(dest => dest.LikesCount, opt => opt.MapFrom(src => src.Likes!.Count(like => like.IsPositive)))
                .ForMember(dest => dest.DislikesCount, opt => opt.MapFrom(src => src.Likes!.Count(like => !like.IsPositive)));

            CreateMap<PostLikeModel, PostLike>();
            CreateMap<PostCommentLikeModel, PostCommentLike>();

            CreateMap<FollowModel, Relation>()
                .ForMember(dest => dest.FollowDate, opt => opt.MapFrom(src => DateTime.UtcNow));
        }
    }
}
