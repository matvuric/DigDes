using Api.Models.Follow;
using AutoMapper;
using DAL;
using DAL.Entities;

namespace Api.Services
{
    public class FollowService
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        private readonly UserService _userService;

        public FollowService(DataContext context, IMapper mapper, UserService userService)
        {
            _context = context;
            _mapper = mapper;
            _userService = userService;
        }

        public async Task FollowUser(FollowModel model)
        {
            var followingUser = await _userService.GetUserById(model.FollowingId);
            if (followingUser.IsPrivate)
            {
                model.IsConfirmed = false;
            }

            var relation = _mapper.Map<Relation>(model);
            var followerUser = await _userService.GetUserById(model.FollowerId);

            followerUser.Following?.Add(relation);

            await _context.SaveChangesAsync();
        }

        public async Task UnfollowUser(FollowModel model)
        {
            var followerUser = await _userService.GetUserById(model.FollowerId);
            var relation = followerUser.Following?.FirstOrDefault(x =>
                x.FollowerId == model.FollowerId && x.FollowingId == model.FollowingId);

            followerUser.Following?.Remove(relation!);

            await _context.SaveChangesAsync();
        }

        public async Task ConfirmFollow(FollowModel model)
        {
            var followerUser = await _userService.GetUserById(model.FollowerId);
            var relation = followerUser.Following?.FirstOrDefault(x =>
                x.FollowerId == model.FollowerId && x.FollowingId == model.FollowingId);

            if (model.IsConfirmed)
            {
                relation!.IsConfirmed = true;
            }
            else
            {
                followerUser.Following?.Remove(relation!);
            }

            await _context.SaveChangesAsync();
        }
    }
}
