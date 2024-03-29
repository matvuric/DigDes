﻿using Api.Models.Attachment;
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
        private readonly AttachmentService _attachmentService;

        public UserService(IMapper mapper, DataContext context, AttachmentService attachmentService)
        {
            _mapper = mapper;
            _context = context;
            _attachmentService = attachmentService;
        }

        public async Task CreateUser(CreateUserModel model)
        {
            var dbUser = _mapper.Map<User>(model);
            var t = await _context.Users.AddAsync(dbUser);
            await _context.SaveChangesAsync();

            if (model.Image != null)
            {
                await SetAvatar(t.Entity.Id, model.Image);
            }
        }

        public async Task EditUserProfile(Guid userId, EditUserProfileModel model)
        {
            var user = await GetUserById(userId);
            user.Username = model.Username;
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.Bio = model.Bio;
            user.Email = model.Email;
            user.Phone = model.Phone;
            user.BirthDate = model.BirthDate;
            user.IsPrivate = model.IsPrivate;
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<UserAvatarModel>> GetUsers()
        {
            return await _context.Users.AsNoTracking()
                .Include(user => user.Avatar)
                .Include(user => user.Posts)
                .Include(user => user.Likes)
                .Include(user => user.Followers)
                .Include(user => user.Following)
                .Select(user => _mapper.Map<UserAvatarModel>(user)).ToListAsync();
        }

        public async Task<UserProfileModel> GetCurrentUser(Guid id)
        {
            var user = await GetUserById(id);

            return _mapper.Map<User, UserProfileModel>(user);
        }

        public async Task<UserAvatarModel> GetUser(Guid id)
        {
            var user = await GetUserById(id);

            return _mapper.Map<User, UserAvatarModel>(user);
        }

        public async Task<User> GetUserById(Guid id)
        {
            var user = await _context.Users
                .Include(user => user.Avatar)
                .Include(user => user.Posts)
                .Include(user => user.Likes)
                .Include(user => user.Followers)
                .Include(user => user.Following)
                .FirstOrDefaultAsync(user => user.Id == id);

            if (user == null)
            {
                throw new UserNotFoundException();
            }

            return user;
        }

        public async Task SetAvatar(Guid userId, MetadataModel meta)
        {
            var model = _mapper.Map<MetadataLinkModel>(meta);

            model.AuthorId = userId;
            model.FilePath = Path.Combine(Directory.GetCurrentDirectory(),
                "Attachments", model.TempId.ToString());
            _attachmentService.MoveFile(model);

            var user = await GetUserById(userId);
            var avatar = _mapper.Map<Avatar>(model);
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
            return await _context.Users
                .AnyAsync(user => user.Email.ToLower() == email.ToLower());
        }

        public async Task DeleteUser(Guid userId)
        {
            var dbUser = await GetUserById(userId);
            _context.Users.Remove(dbUser);
            await _context.SaveChangesAsync();
        }

        public async Task SetPushToken(Guid userId, string? token = null)
        {
            var user = await GetUserById(userId);
            user.PushToken = token;
            await _context.SaveChangesAsync();
        }

        public async Task<string?> GetPushToken(Guid userId)
        {
            var user = await GetUserById(userId);
            return user.PushToken;
        }
    }
}
