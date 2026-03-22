
using System.ComponentModel.DataAnnotations;
namespace HRsystem.Models
{

    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; } // ⚠️ hash later
        public string Role { get; set; }
    }
}