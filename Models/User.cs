using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace UMA.Models
{
    public class User
    {
        [Required]
        [Key]
        [JsonIgnore]
        public Guid Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string UserName {  get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        [StringLength(10)]
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        [Required]
        public Role RoleId { get; set; }

    }
    public enum Role
    {
        Admin =1,
        User= 0
    }
}
