using System;
using System.ComponentModel.DataAnnotations;

namespace BattalgaziSosyalYardim.Models
{
    public class ApplicationCreateViewModel
    {
        [Display(Name = "Anne T.C. Kimlik No")]
        [Required(ErrorMessage = "{0} zorunludur.")]
        [StringLength(11, MinimumLength = 11, ErrorMessage = "{0} 11 haneli olmalıdır.")]
        [RegularExpression(@"^[1-9][0-9]{10}$", ErrorMessage = "{0} 11 haneli olmalıdır.")]
        public string MotherNationalId { get; set; } = default!;

        [Display(Name = "Anne Doğum Tarihi")]
        [Required(ErrorMessage = "{0} zorunludur.")]
        [DataType(DataType.Date)]
        public DateTime? MotherBirthDate { get; set; }

        [Display(Name = "Anne Ad Soyad")]
        [Required(ErrorMessage = "{0} zorunludur.")]
        [StringLength(100, ErrorMessage = "{0} en fazla {1} karakter olabilir.")]
        public string MotherFullName { get; set; } = default!;

        [Display(Name = "Telefon")]
        [Required(ErrorMessage = "{0} zorunludur.")]
        [RegularExpression(@"^(?:\+?90)?0?5\d{9}$", ErrorMessage = "{0} 05XXXXXXXXX formatında olmalıdır.")]
        public string PhoneNumber { get; set; } = default!;

        [Display(Name = "Bebek T.C. Kimlik No")]
        [Required(ErrorMessage = "{0} zorunludur.")]
        [StringLength(11, MinimumLength = 11, ErrorMessage = "{0} 11 haneli olmalıdır.")]
        [RegularExpression(@"^[1-9][0-9]{10}$", ErrorMessage = "{0} 11 haneli olmalıdır.")]
        public string BabyNationalId { get; set; } = default!;

        // UI için
        public string ProgramCode { get; set; } = "bez-destegi";
        public string ProgramTitle { get; set; } = "0-2 YAŞ BEBEK BEZİ DESTEĞİ BAŞVURU FORMU";
    }
}
