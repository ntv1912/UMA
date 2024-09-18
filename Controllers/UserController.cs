using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UMA.Context;
using UMA.DTO;
using UMA.Models;

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
        public IActionResult GetAllUsers()
        {
            try
            {
                var users = _context.Users.ToList();
                return Ok(users);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("GetUserById/id={id}")]
        [Authorize(Policy = "AdminPolicy,UserPolicy")]
        public IActionResult GetUserById(Guid id)
        {
            try
            {
                var user = _context.Users.SingleOrDefault(u => u.Id == id);
                if (user == null)
                {
                    return NotFound($"User with ID {id} not found.");
                }
                return Ok(user);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("CreateUser")]
        [AllowAnonymous]
        public IActionResult CreateUser(UserDto userDto)
        {
            if (userDto == null)
            {
                return BadRequest("User data is null.");
            }
            if(userDto .RoleId == Role.Admin )
            {
                return Unauthorized();
            }
            try
            {
                var isExists = _context.Users.Any(t =>  t.UserName==userDto.UserName|| t.Email==userDto.Email) ;
                if(isExists)
                {
                    return Conflict("Data already exists");
                }
                var newUser = new User
                {
                    Name = userDto.Name,
                    Address = userDto.Address,
                    Email = userDto.Email,
                    PhoneNumber = userDto.PhoneNumber,
                    RoleId = userDto.RoleId,
                    UserName = userDto.UserName,
                    Password = _passwordHasher.HashPassword(new User(), userDto.Password)
                };

                _context.Users.Add(newUser);
                _context.SaveChanges();
                return CreatedAtAction(nameof(GetUserById), new { id = newUser.Id }, newUser);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut]
        [Route("UpdateUser")]
        [Authorize(Policy ="UserPolicy")]
        public IActionResult UpdateUser(Guid id, UserDto userDto)
        {
            if (userDto == null)
            {
                return BadRequest("User data is null.");
            }

            try
            {

                var user = _context.Users.Find(id);
                if (user == null)
                {
                    return NotFound($"User with ID {id} not found.");
                }
                var lsUsers = _context.Users.Where(t  =>t.Id != id).ToList();
                var isExists = lsUsers.Any(t => t.UserName == userDto.UserName || t.Email == userDto.Email);
                if (isExists)
                {
                    return Conflict("Data already exists");
                }

                user.Name = userDto.Name;
                user.Address = userDto.Address;
                user.Email = userDto.Email;
                user.PhoneNumber = userDto.PhoneNumber;
                user.RoleId = userDto.RoleId;
                user.UserName = userDto.UserName;

                _context.Users.Update(user);
                _context.SaveChanges();
                return Ok(user);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
