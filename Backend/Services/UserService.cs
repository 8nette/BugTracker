using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Backend;
using Backend.Models;
using Backend.DB;
using Backend.Helpers;

namespace Backend.Services
{
    public interface IUserService
    {
        Developer Authenticate(string username, string password, string secret);
        bool Register(Developer user);
        Developer GetUser(string username);
        Developer GetUserById(int? id);
        bool UpdateUser(Developer user);
    }

    public class UserService : IUserService
    {
        private IDatabase database;

        public UserService(IDatabase _database)
        {
            database = _database;
        }

        public Developer Authenticate(string username, string password, string secret)
        {
            Developer user = database.checkDeveloper(username, password);

            if (user == null)
                return null;

            // authentication successful so generate jwt token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Id.ToString()),
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            user.Token = tokenHandler.WriteToken(token);

            return user.WithoutPassword();
        }

        public bool Register(Developer user)
        {
            return database.createDeveloper(user);
        }

        public Developer GetUser(string username)
        {
            return database.getDeveloper(username);
        }

        public Developer GetUserById(int? id)
        {
            return database.getDeveloperById(id);
        }

        public bool UpdateUser(Developer user)
        {
            return database.updateDeveloper(user);
        }
    }
}
