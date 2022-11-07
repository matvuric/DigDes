using Api.Configs;
using Api.Models;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Common;
using DAL;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using DAL.Entities;

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
            var dbUser = _mapper.Map<DAL.Entities.User>(model);
            var t = await _context.Users.AddAsync(dbUser);
            await _context.SaveChangesAsync();
            return t.Entity.Id;
        }

        public async Task<List<UserModel>> GetUsers()
        {
            return await _context.Users.AsNoTracking().ProjectTo<UserModel>(_mapper.ConfigurationProvider).ToListAsync();
        }

        public async Task<User> GetUserById(Guid id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == id);

            if (user == null)
            {
                throw new Exception("User not found");
            }

            return user;
        }

        public async Task<UserModel> GetUser(Guid id)
        {
            var user = await GetUserById(id);

            return _mapper.Map<UserModel>(user);
        }

        private async Task<User> GetUserByIdWithAvatar(Guid id)
        {
            var user = await _context.Users.Include(user => user.Avatar).FirstOrDefaultAsync(x => x.Id == id);

            if (user == null)
            {
                throw new Exception("User not found");
            }

            return user;
        }

        public async Task SetAvatar(Guid userId, MetaDataModel meta, string filePath)
        {
            var user = await GetUserByIdWithAvatar(userId);
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

        public async Task<AttachModel> GetAvatarById(Guid userId)
        {
            var user = await GetUserByIdWithAvatar(userId);
            return _mapper.Map<AttachModel>(user.Avatar);
        }
    }
}
