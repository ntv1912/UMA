using Azure.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Text;
using UMA.Context;
using UMA.DTO;
using UMA.Models;
using UMA.Services;

namespace UMA.Controllers
{
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
        public async Task<IActionResult> Login(string inp,string password)
        {
            var user= await _dataContext.Users.SingleOrDefaultAsync(t => (t.UserName == inp|| t.Email==inp||t.PhoneNumber==inp));
            if (user==null)
            {
                return NotFound("nn");
            }
            var verPass= _passwordHasher.VerifyHashedPassword(user,user.Password,password);
            if(verPass==PasswordVerificationResult.Success)
            {
                var result= _jwtTokenService.GenToken(user);
                return( Ok(new
                {
                    token= result,
                }));
            }
            return Conflict("zz");
        }
    }
}
