using System.ComponentModel.DataAnnotations;

namespace BattalgaziSosyalYardim.Models
{
    public class LoginViewModel
    {
        [Display(Name = "T.C. Kimlik No")]
        [Required(ErrorMessage = "{0} zorunludur.")]
        [StringLength(11, MinimumLength = 11, ErrorMessage = "{0} 11 haneli olmalıdır.")]
        [RegularExpression(@"^[1-9][0-9]{10}$", ErrorMessage = "{0} 11 haneli olmalıdır.")]
        public string NationalId { get; set; } = default!;

        [Display(Name = "Şifre")]
        [Required(ErrorMessage = "{0} zorunludur.")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = default!;
    }
}
