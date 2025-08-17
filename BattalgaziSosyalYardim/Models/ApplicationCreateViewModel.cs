using System;
using System.ComponentModel.DataAnnotations;

namespace BattalgaziSosyalYardim.Models
{
    public class ApplicationCreateViewModel
    {
        [Display(Name = "Anne T.C. Kimlik No")]
        [Required, StringLength(11, MinimumLength = 11)]
        [RegularExpression(@"^[1-9][0-9]{10}$", ErrorMessage = "T.C. Kimlik No 11 haneli olmalıdır.")]
        public string MotherNationalId { get; set; } = default!;

        [Display(Name = "Anne Doğum Tarihi")]
        [Required, DataType(DataType.Date)]
        public DateTime MotherBirthDate { get; set; }

        [Display(Name = "Anne Ad Soyad")]
        [Required, StringLength(100)]
        public string MotherFullName { get; set; } = default!;

        [Display(Name = "Telefon")]
        [Required]
        [RegularExpression(@"^(?:\+?90)?0?5\d{9}$", ErrorMessage = "Geçerli bir GSM numarası giriniz.")]
        public string PhoneNumber { get; set; } = default!;

        [Display(Name = "Bebek T.C. Kimlik No")]
        [Required, StringLength(11, MinimumLength = 11)]
        [RegularExpression(@"^[1-9][0-9]{10}$", ErrorMessage = "T.C. Kimlik No 11 haneli olmalıdır.")]
        public string BabyNationalId { get; set; } = default!;

        // UI amaçlı
        public string ProgramCode { get; set; } = "bez-destegi";
        public string ProgramTitle { get; set; } = "0-2 YAŞ BEBEK BEZİ DESTEĞİ BAŞVURU FORMU";

    }
}
