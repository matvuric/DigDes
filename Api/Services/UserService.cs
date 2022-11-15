using Api.Models.Attachment;
using Api.Models.User;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using DAL;
using DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace Api.Services
{
    public class UserService
    {
        private readonly IMapper _mapper;
        private readonly DataContext _context;
        private Func<UserModel, string?>? _linkGenerator;

        public UserService(IMapper mapper, DataContext context)
        {
            _mapper = mapper;
            _context = context;
        }

        public void SetLinkGenerator(Func<UserModel, string?> linkGenerator)
        {
            _linkGenerator = linkGenerator;
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

        public async Task<Guid> CreateUser(CreateUserModel model)
        {
            var dbUser = _mapper.Map<User>(model);

            var userEntity = await _context.Users.AddAsync(dbUser);
            await _context.SaveChangesAsync();

            return userEntity.Entity.Id;
        }

        public async Task<IEnumerable<UserAvatarModel>> GetUsers()
        {
            var users = await _context.Users.AsNoTracking().ProjectTo<UserModel>(_mapper.ConfigurationProvider).ToListAsync();
            return users.Select(user => new UserAvatarModel(user, _linkGenerator));
        }

        public async Task<UserAvatarModel> GetUser(Guid id)
        {
            var user = await GetUserById(id);

            return new UserAvatarModel(_mapper.Map<UserModel>(user), user.Avatar == null? null: _linkGenerator);
        }

        public async Task<User> GetUserById(Guid id)
        {
            var user = await _context.Users.Include(user => user.Avatar).FirstOrDefaultAsync(user => user.Id == id);

            if (user == null || user == default)
            {
                throw new Exception("User not found");
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
    }
}
