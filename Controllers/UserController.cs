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

        [HttpGet("GetUserById{id}")]
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
        public IActionResult CreateUser(UserDto userDto)
        {
            if (userDto == null)
            {
                return BadRequest("User data is null.");
            }

            try
            {
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

        [HttpPut("UpdateUser{id}")]

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
