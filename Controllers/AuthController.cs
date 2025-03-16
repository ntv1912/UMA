using Azure.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Text;
using UMA.Context;
using UMA.DTO;
using UMA.Exceptions;
using UMA.Models;
using UMA.Services;

namespace UMA.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController:ControllerBase
    {
        private readonly DataContext _dataContext;
        private readonly JwtTokenService _jwtTokenService;
        private readonly PasswordHasher<User>_passwordHasher;
        public AuthController(JwtTokenService jwtTokenService,DataContext context   )
        {
            _jwtTokenService=jwtTokenService;
            _dataContext = context;
            _passwordHasher= new PasswordHasher<User>();
        }
        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login(UserLoginDto login)
        {
            var user = await _dataContext.Users.SingleOrDefaultAsync(t => (t.UserName == login.inp || t.Email == login.inp || t.PhoneNumber == login.inp));
            if (user == null)
            {
                throw new NotFoundException();
            }
            var verPass = _passwordHasher.VerifyHashedPassword(user, user.Password, login.password);
            if (verPass == PasswordVerificationResult.Success)
            {
                var result = _jwtTokenService.GenToken(user);
                return (Ok(new
                {
                    token = result,
                }));
            }
            throw new InvalidLoginException();
        }
        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Resgister(UserDto userDto)
        {
            if (userDto == null)
            {
                throw new InvalidInputException();
            }
            if (userDto.RoleId == Role.Admin)
            {
                throw new UnauthorizedException();
            }
            var isExists = await _dataContext.Users.AnyAsync(t => t.UserName == userDto.UserName || t.Email == userDto.Email);
            if (isExists)
            {
                throw new DuplicateDataException();
            }
            var newUser = new User
            {
                Name = userDto.Name,
                Address = userDto.Address,
                Email = userDto.Email,
                PhoneNumber = userDto.PhoneNumber,
                RoleId = userDto.RoleId,
                UserName = userDto.UserName,
            };
            newUser.Password = _passwordHasher.HashPassword(newUser, userDto.Password);
            await _dataContext.Users.AddAsync(newUser);
            await _dataContext.SaveChangesAsync();
            var result = _jwtTokenService.GenToken(newUser);
            return Ok(new { token = result });
        }
    }
}
