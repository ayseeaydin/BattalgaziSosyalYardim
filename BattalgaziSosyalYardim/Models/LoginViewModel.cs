using System.ComponentModel.DataAnnotations;

namespace BattalgaziSosyalYardim.Models
{
    public class LoginViewModel
    {
        [Required, Display(Name ="TC Kimlik No")]
        public string Username { get; set; } = default!;
        [Required, DataType(DataType.Password), Display(Name = "Şifre")]
        public string Password { get; set; } = default!;
    }
}
