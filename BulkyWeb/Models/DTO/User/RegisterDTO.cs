using System.ComponentModel.DataAnnotations;

namespace BulkyWeb.Models.DTO.User
{
    public class RegisterDTO
    {


        public string Email { get; set; }

        public string Password { get; set; }

        [Required]
        [Compare("Password")]
        public string ConfirmPassword { get; set; }
    }


}
