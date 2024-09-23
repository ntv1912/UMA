using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using UMA.Context;
using UMA.DTO;
using UMA.Exceptions;
using UMA.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace UMA.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IPasswordHasher<User> _passwordHasher;

        public UsersController(DataContext context)
        {
            _context = context;
            _passwordHasher = new PasswordHasher<User>();
        }

        [HttpGet("GetAllUsers")]
        [Authorize(Policy= "AdminPolicy")]
        public async Task<Response> GetAllUsers()
        {
            var users = await _context.Users.ToListAsync();
            if(users==null|| users.Count==0) throw new NotFoundException();
            Response res = new Response
            {
                IsError = true,
                Message = "Success",
                Data = users,
                StatusCode= 200
            };
            return res;
        }

        [HttpGet("GetUserById/id={id}")]
        [Authorize(Policy = "AdminPolicy,UserPolicy")]
        public async Task<Response> GetUserById(string id)
        {
            Guid Id = new Guid();
            if (!Guid.TryParse(id, out Id) )
            {
                throw new InvalidInputException();
            }
            var user =await _context.Users.SingleOrDefaultAsync(u => u.Id == Id);
            if (user == null) throw new NotFoundException();
            Response res = new Response
            {
                IsError = true,
                Message = "Success",
                Data = user,
                StatusCode = 200
            };
            return res;
        }

        [HttpPost("CreateUser")]
        [Authorize(Policy ="AdminPolicy")]
        public async Task<Response> CreateUser(UserDto userDto)
        {
            if (userDto == null)
            {
                throw new InvalidInputException();
            }
            if(userDto .RoleId == Role.Admin )
            {
              throw new UnauthorizedException();
            }

            var isExists =await _context.Users.AnyAsync(t =>  t.UserName==userDto.UserName|| t.Email==userDto.Email) ;
            if(isExists)
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
            await _context.Users.AddAsync(newUser);
            await  _context.SaveChangesAsync();
            Response res = new Response
            {
                IsError = true,
                Message = "Success",
                Data = newUser,
                StatusCode = 200
            };
            return res;
        }
        [HttpPut]
        [Route("UpdateUser")]
        [Authorize(Policy ="UserPolicy")]
        public async Task<Response> UpdateUser(string id, UserDto userDto)
        {
            Guid Id= new Guid();
            if(!Guid.TryParse(id, out Id)|| userDto == null)
            {
                throw new InvalidInputException();
            }

            var user =await _context.Users.FindAsync(id);
            if (user == null)
            {
                throw new NotFoundException();
            }
            var lsUsers =await _context.Users.Where(t  =>t.Id != Id).ToListAsync();
            var isExists = lsUsers.Any(t => t.UserName == userDto.UserName || t.Email == userDto.Email);
            if (isExists)
            {
                throw new DuplicateDataException();
            }

            user.Name = userDto.Name;
            user.Address = userDto.Address;
            user.Email = userDto.Email;
            user.PhoneNumber = userDto.PhoneNumber;
            user.RoleId = userDto.RoleId;
            user.UserName = userDto.UserName;

            _context.Users.Update(user);
            _context.SaveChanges();
            Response res = new Response
            {
                IsError = true,
                Message = "Success",
                Data = user,
                StatusCode = 200
            };
            return res;
        }
    }
}
