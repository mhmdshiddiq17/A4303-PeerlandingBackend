using DAL.DTO.Req;
using DAL.DTO.Res;
using DAL.Models;
using DAL.Repositories.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories.Services
{   
    public class UserServices : IUserServices
    {
        private readonly PostgresContext _context;
        private readonly IConfiguration _configuration;

        public UserServices(PostgresContext context, IConfiguration configuration) 
        {
            _context = context;
            _configuration = configuration;
        }


        public async Task<string> Register(ReqRegisterUserDto register)
        {
            var isAnyEmail = await _context.Users.SingleOrDefaultAsync(e => e.Email == register.Email);
            if(isAnyEmail != null)
            {
                throw new Exception("email already used");
            }

            var newUser = new User
            {
                Name = register.Name,
                Email = register.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(register.Password),
                Role = register.Role,
                Balance = register.Balance,
            };

            await _context.Users.AddAsync(newUser);
            await _context.SaveChangesAsync();
            

            return newUser.Name;
        }


        public Task<List<ResUserDto>> GetAllUsers()
        {
            return _context.Users
                .Where(user => user.Role != "admin")
                .Select(user => new ResUserDto
                {
                    Id = user.Id,
                    Name = user.Name,
                    Email = user.Email,
                    Role = user.Role,
                    Balance = user.Balance
                }).ToListAsync();
        }


        public async Task<ResLoginDto> Login(ReqLoginDto reqLogin)
        {
            var user = await _context.Users.SingleOrDefaultAsync(e => e.Email == reqLogin.Email);
            if (user == null)
            {
                throw new Exception("Invalid email or password");
            }
            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(reqLogin.Password, user.Password);
            if(!isPasswordValid)
            {
                throw new Exception("Invalid email or password");
            }
            var token = GenerateJwtToken(user);
            var loginResponse = new ResLoginDto
            {
                Token = token,
            };
            return loginResponse;
        }
        private string GenerateJwtToken(User user)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings["SecretKey"];

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };
            var token = new JwtSecurityToken(
                issuer: jwtSettings["ValidIssuer"],
                audience: jwtSettings["ValidAudience"],
                claims: claims,
                expires: DateTime.Now.AddHours(2),
                signingCredentials: credentials

            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        
        async Task<string> IUserServices.Update(string userId, ReqUpdateUserDto reqUpdate)
        {
            var existingUser = await _context.Users.SingleOrDefaultAsync(user => user.Id == userId);
            if(existingUser == null)
            {
                throw new Exception("User Not Found");
            }

            existingUser.Name = reqUpdate.Name ?? existingUser.Name;
            existingUser.Name = reqUpdate.Role ?? existingUser.Role;
            existingUser.Balance = reqUpdate.Balance ?? existingUser.Balance;
            _context.Users.Update(existingUser);
            await _context.SaveChangesAsync();

            return reqUpdate.Name;
        }

        public async Task<List<ResUserDto>> GetAllUser()
        {
            return await _context.Users
                .Where(user => user.Role != "admin")
                .Select(user => new ResUserDto
                {
                    Id = user.Id,
                    Name = user.Name,
                    Email = user.Email,
                    Role = user.Role,
                    Balance = user.Balance
                }).ToListAsync();
        }
        public async Task<string> Delete(string userId)
        {
            var user = await _context.Users.SingleOrDefaultAsync(user => user.Id == userId);


            if (user == null)
            {
                throw new Exception("User not found");
            }

            var userName = user.Name;

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return userName;
        }


    }
}
