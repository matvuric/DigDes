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

        private TokenModel GenerateTokens(DAL.Entities.User user)
        {
            var dtNow = DateTime.Now;

            var jwt = new JwtSecurityToken(
                issuer: _config.Issuer,
                audience: _config.Audience,
                notBefore: dtNow,
                claims: new Claim[]
                {
                    new Claim(ClaimsIdentity.DefaultNameClaimType, user.Name),
                    new Claim("id", user.Id.ToString()),
                },
                expires: DateTime.Now.AddMinutes(_config.LifeTime),
                signingCredentials: new SigningCredentials(_config.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256)
                );
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            var refresh = new JwtSecurityToken(
                notBefore: dtNow,
                claims: new Claim[]
                {
                    new Claim("id", user.Id.ToString()),
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

            return GenerateTokens(user);
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

            if (principal.Claims.FirstOrDefault(x => x.Type == "id")?.Value is String userIdString
                && Guid.TryParse(userIdString, out var userId))
            {
                var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);

                if (user == null)
                {
                    throw new Exception("User not found");
                }

                return GenerateTokens(user);
            }
            else
            {
                throw new SecurityTokenException("Invalid token");
            }
        }
    }
}
