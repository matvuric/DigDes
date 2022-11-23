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
            #region User

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

            #endregion

            #region Attachment

            CreateMap<Avatar, AttachmentModel>();

            CreateMap<PostAttachment, AttachmentModel>();
            CreateMap<PostAttachment, AttachmentExternalModel>().AfterMap<PostAttachmentMapperAction>();

            CreateMap<PostCommentAttachment, AttachmentModel>();
            CreateMap<PostCommentAttachment, AttachmentExternalModel>().AfterMap<PostCommentAttachmentMapperAction>();

            CreateMap<MetadataModel, MetadataLinkModel>();

            CreateMap<MetadataLinkModel, Avatar>();
            CreateMap<MetadataLinkModel, PostAttachment>();
            CreateMap<MetadataLinkModel, PostCommentAttachment>();

            #endregion

            #region Post and comment

            CreateMap<CreatePostModel, PostModel>();
            CreateMap<CreatePostCommentModel, PostCommentModel>();

            CreateMap<PostModel, Post>()
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => DateTime.UtcNow));
            CreateMap<PostCommentModel, PostComment>()
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => DateTime.UtcNow));

            CreateMap<Post, ReturnPostModel>()
                .ForMember(dest => dest.CommentsCount, opt => opt.MapFrom(src => src.Comments!.Count))
                .ForMember(dest => dest.LikesCount, opt => opt.MapFrom(src => src.Likes!.Count(like => like.IsPositive)))
                .ForMember(dest => dest.DislikesCount, opt => opt.MapFrom(src => src.Likes!.Count(like => !like.IsPositive)));
            CreateMap<PostComment, ReturnPostCommentModel>()
                .ForMember(dest => dest.LikesCount, opt => opt.MapFrom(src => src.Likes!.Count(like => like.IsPositive)))
                .ForMember(dest => dest.DislikesCount, opt => opt.MapFrom(src => src.Likes!.Count(like => !like.IsPositive)));

            CreateMap<Post, ReturnPostWithCommentsModel>()
                .ForMember(dest => dest.PostComments, opt => opt.MapFrom(src => src.Comments))
                .ForMember(dest => dest.LikesCount, opt => opt.MapFrom(src => src.Likes!.Count(like => like.IsPositive)))
                .ForMember(dest => dest.DislikesCount, opt => opt.MapFrom(src => src.Likes!.Count(like => !like.IsPositive)));

            #endregion

            #region Like

            CreateMap<PostLikeModel, PostLike>();
            CreateMap<PostCommentLikeModel, PostCommentLike>();

            #endregion

            #region Follow

            CreateMap<FollowModel, Relation>()
                .ForMember(dest => dest.FollowDate, opt => opt.MapFrom(src => DateTime.UtcNow));

            #endregion
        }
    }
}
