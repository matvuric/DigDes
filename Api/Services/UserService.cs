using Api.Models.Attachment;
using Api.Models.User;
using AutoMapper;
using Common.Exceptions;
using DAL;
using DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace Api.Services
{
    public class UserService
    {
        private readonly IMapper _mapper;
        private readonly DataContext _context;

        public UserService(IMapper mapper, DataContext context)
        {
            _mapper = mapper;
            _context = context;
        }

        public async Task<Guid> CreateUser(CreateUserModel model)
        {
            var dbUser = _mapper.Map<User>(model);

            var userEntity = await _context.Users.AddAsync(dbUser);
            await _context.SaveChangesAsync();

            return userEntity.Entity.Id;
        }

        /*public async Task EditProfile(EditUserProfileModel model, Guid userId)
        {
            var user = await GetUserById(userId);
        }*/

        public async Task<IEnumerable<UserAvatarModel>> GetUsers()
        {
            return await _context.Users.AsNoTracking()
                .Include(user => user.Avatar)
                .Include(user => user.Posts)
                .Include(user => user.Likes)
                .Select(user => _mapper.Map<UserAvatarModel>(user)).ToListAsync();
        }

        public async Task<UserAvatarModel> GetUser(Guid id)
        {
            var user = await GetUserById(id);

            return _mapper.Map<User, UserAvatarModel>(user);
        }

        public async Task<User> GetUserById(Guid? id)
        {
            var user = await _context.Users
                .Include(user => user.Avatar)
                .Include(user => user.Posts)
                .Include(user => user.Likes)
                .Include(user => user.Followers)
                .Include(user => user.Following)
                .FirstOrDefaultAsync(user => user.Id == id);

            if (user == null || user == default)
            {
                throw new UserNotFoundException();
            }

            return user;
        }

        public async Task SetAvatar(Guid userId, MetadataModel meta, string filePath)
        {
            var user = await GetUserById(userId);
            var avatar = new Avatar
            {
                Name = meta.Name,
                MimeType = meta.MimeType,
                FilePath = filePath,
                Size = meta.Size,
                Author = user,
            };
            user.Avatar = avatar;

            await _context.SaveChangesAsync();
        }

        public async Task<AttachmentModel> GetAvatarById(Guid userId)
        {
            var user = await GetUserById(userId);
            return _mapper.Map<AttachmentModel>(user.Avatar);
        }

        public async Task<bool> CheckUserExist(string email)
        {
            return await _context.Users.AnyAsync(user => user.Email.ToLower() == email.ToLower());
        }

        public async Task DeleteUser(Guid userId)
        {
            var dbUser = await GetUserById(userId);
            _context.Users.Remove(dbUser);
            await _context.SaveChangesAsync();
        }
    }
}
