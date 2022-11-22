using Api.Models.Like;
using AutoMapper;
using Common.Exceptions;
using DAL;
using DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace Api.Services
{
    public class LikeService
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public LikeService(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task LikePost(PostLikeModel model)
        {
            var lastLike = await GetPostLikeByIds(model.AuthorId, model.PostId);

            if(lastLike == null)
            {
                var dbPostLike = _mapper.Map<PostLike>(model);
                await _context.PostLikes.AddAsync(dbPostLike);
                await _context.SaveChangesAsync();
            }
            else
            {
                if (lastLike.IsPositive == model.IsPositive)
                {
                    await DeletePostLike(lastLike.Id);
                }
                else
                {
                    lastLike.IsPositive = model.IsPositive;
                    await _context.SaveChangesAsync();
                }
            }
        }

        private async Task<PostLike> GetPostLikeByIds(Guid? userId, Guid postId)
        {
            var like = await _context.PostLikes.Where(postLike => postLike.AuthorId == userId)
                .FirstOrDefaultAsync(postLike => postLike.PostId == postId);

            return like!;
        }

        private async Task DeletePostLike(Guid postLikeId)
        {
            var dbPostLike = await GetPostLikeById(postLikeId);
            _context.PostLikes.Remove(dbPostLike);
            await _context.SaveChangesAsync();
        }

        private async Task<PostLike> GetPostLikeById(Guid likeId)
        {
            var like = await _context.PostLikes.FirstOrDefaultAsync(postLike => postLike.Id == likeId);

            if (like == null)
            {
                throw new LikeNotFoundException();
            }

            return like;
        }

        public async Task LikePostComment(PostCommentLikeModel model)
        {
            var lastLike = await GetPostCommentLikeByIds(model.AuthorId, model.PostCommentId);

            if (lastLike == null)
            {
                var dbPostCommentLike = _mapper.Map<PostCommentLike>(model);
                await _context.PostCommentLikes.AddAsync(dbPostCommentLike);
                await _context.SaveChangesAsync();
            }
            else
            {
                if (lastLike.IsPositive == model.IsPositive)
                {
                    await DeletePostCommentLike(lastLike.Id);
                }
                else
                {
                    lastLike.IsPositive = model.IsPositive;
                    await _context.SaveChangesAsync();
                }
            }
        }

        private async Task<PostCommentLike> GetPostCommentLikeByIds(Guid? userId, Guid postCommentId)
        {
            var like = await _context.PostCommentLikes.Where(postLike => postLike.AuthorId == userId)
                .FirstOrDefaultAsync(postLike => postLike.PostCommentId == postCommentId);

            return like!;
        }

        private async Task DeletePostCommentLike(Guid likeId)
        {
            var dbPostCommentLike = await GetPostCommentLikeById(likeId);
            _context.PostCommentLikes.Remove(dbPostCommentLike);
            await _context.SaveChangesAsync();
        }

        private async Task<PostCommentLike> GetPostCommentLikeById(Guid likeId)
        {
            var like = await _context.PostCommentLikes.FirstOrDefaultAsync(postCommentLike => postCommentLike.Id == likeId);

            if (like == null)
            {
                throw new LikeNotFoundException();
            }

            return like;
        }
    }
}
