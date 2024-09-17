using System.ComponentModel.DataAnnotations;
using UMA.Models;

namespace UMA.DTO
{
    public class UserDto
    {
        public string Name { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public Role RoleId { get; set; }
    }
}
