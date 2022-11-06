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
    public class AuthService
    {
        private readonly DataContext _context;
        private readonly AuthConfig _config;

        public AuthService(DataContext context, IOptions<AuthConfig> config)
        {
            _context = context;
            _config = config.Value;
        }

        private async Task<DAL.Entities.User> GetUserByCredentials(string login, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Email.ToLower() == login.ToLower());

            if (user == null)
            {
                throw new Exception("User not found");
            }

            if (!HashHelper.Verify(password, user.PasswordHash))
            {
                throw new Exception("Password is incorrect");
            }

            return user;
        }

        private TokenModel GenerateTokens(DAL.Entities.UserSession session)
        {
            var dtNow = DateTime.Now;

            if (session.User == null)
            {
                throw new Exception("User is not exist");
            }

            var jwt = new JwtSecurityToken(
                issuer: _config.Issuer,
                audience: _config.Audience,
                notBefore: dtNow,
                claims: new Claim[]
                {
                    new Claim(ClaimsIdentity.DefaultNameClaimType, session.User.Name),
                    new Claim("id", session.User.Id.ToString()),
                    new Claim("sessionId", session.Id.ToString()),
                },
                expires: DateTime.Now.AddMinutes(_config.LifeTime),
                signingCredentials: new SigningCredentials(_config.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256)
                );
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            var refresh = new JwtSecurityToken(
                notBefore: dtNow,
                claims: new Claim[]
                {
                    new Claim("refreshToken", session.RefreshToken.ToString()),
                },
                expires: DateTime.Now.AddHours(_config.LifeTime),
                signingCredentials: new SigningCredentials(_config.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256)
                );
            var encodedRefresh = new JwtSecurityTokenHandler().WriteToken(refresh);

            return new TokenModel(encodedJwt, encodedRefresh);
        }

        public async Task<TokenModel> GetToken(string login, string password)
        {
            var user = await GetUserByCredentials(login, password);
            var session = await _context.UserSessions.AddAsync(new DAL.Entities.UserSession()
            {
                User = user,
                RefreshToken = Guid.NewGuid(),
                Created = DateTime.UtcNow,
                Id = Guid.NewGuid()
            });
            await _context.SaveChangesAsync();
            return GenerateTokens(session.Entity);
        }

        public async Task<UserSession> GetSessionById(Guid id)
        {
            var session = await _context.UserSessions.FirstOrDefaultAsync(userSession => userSession.Id == id);
            if (session == null)
            {
                throw new Exception("Session is not found");
            }

            return session;
        }

        private async Task<UserSession> GetSessionByRefreshToken(Guid id)
        {
            var session = await _context.UserSessions.Include(x => x.User)
                .FirstOrDefaultAsync(userSession => userSession.RefreshToken == id);
            if (session == null)
            {
                throw new Exception("Session is not found");
            }

            return session;
        }

        public async Task<TokenModel> GetTokenByRefreshToken(string refreshToken)
        {
            var validParams = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateIssuerSigningKey = true,
                ValidateLifetime = true,
                IssuerSigningKey = _config.GetSymmetricSecurityKey()
            };

            var principal = new JwtSecurityTokenHandler().ValidateToken(refreshToken, validParams, out var securityToken);

            if (securityToken is not JwtSecurityToken jwtToken
                || !jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid token");
            }

            if (principal.Claims.FirstOrDefault(claim => claim.Type == "refreshToken")?.Value is String refreshIdString
                && Guid.TryParse(refreshIdString, out var refreshId))
            {
                var session = await GetSessionByRefreshToken(refreshId);
                if (!session.IsActive)
                {
                    throw new Exception("Session is not active");
                }

                session.RefreshToken = Guid.NewGuid();
                await _context.SaveChangesAsync();
                return GenerateTokens(session);
            }
            else
            {
                throw new SecurityTokenException("Invalid token");
            }
        }
    }
}
