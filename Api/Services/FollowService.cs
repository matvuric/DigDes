using Api.Models.Follow;
using AutoMapper;
using Common.Exceptions;
using DAL;
using DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace Api.Services
{
    public class FollowService
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public FollowService(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task FollowUser(FollowModel model)
        {
            var followingUser = await _context.Users
                .FirstOrDefaultAsync(user => user.Id == model.FollowingId);

            if (followingUser == null)
            {
                throw new UserNotFoundException();
            }

            if (followingUser.IsPrivate)
            {
                model.IsConfirmed = false;
            }

            await _context.Relations.AddAsync(_mapper.Map<Relation>(model));
            await _context.SaveChangesAsync();
        }

        public async Task UnfollowUser(FollowModel model)
        {
            var relation = await GetRelation(model);
            _context.Relations.Remove(relation);
            await _context.SaveChangesAsync();
        }

        public async Task ConfirmFollow(FollowModel model)
        {
            var relation = await GetRelation(model);

            if (model.IsConfirmed)
            {
                relation.IsConfirmed = true;
            }
            else
            {
                _context.Relations.Remove(relation);
            }

            await _context.SaveChangesAsync();
        }

        private async Task<Relation> GetRelation(FollowModel model)
        {
            var relation = await _context.Relations
                .FirstOrDefaultAsync(rel => rel.FollowerId == model.FollowerId && rel.FollowingId == model.FollowingId);

            if (relation == null)
            {
                throw new RelationNotFoundException();
            }

            return relation;
        }
    }
}
