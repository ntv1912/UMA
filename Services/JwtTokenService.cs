using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using UMA.Models;

namespace UMA.Services
{
    public class JwtTokenService
    {
        private readonly IConfiguration _configuration;
        public JwtTokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public string GenToken(User user)
        {
            var jwtSetting = _configuration.GetSection("Jwt");
            var key= new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSetting["Key"]));
            var creds= new SigningCredentials(key,SecurityAlgorithms.HmacSha256);
            var claims = new[]
            {
              /*  new Claim(ClaimTypes.Name,user.Name),
                new Claim(ClaimTypes.MobilePhone,user.PhoneNumber),
                new Claim(ClaimTypes.Email,user.Email),*/
                new Claim(ClaimTypes.Role,((int)user.RoleId).ToString()),
            };
            var token = new JwtSecurityToken(
                issuer: jwtSetting["Issuer"],
                audience: jwtSetting["Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(int.Parse(jwtSetting["ExpiryMinutes"])),
                signingCredentials:creds
                );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
