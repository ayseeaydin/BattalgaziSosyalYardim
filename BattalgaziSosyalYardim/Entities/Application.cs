using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BattalgaziSosyalYardim.Entities
{
    public class Application
    {
        public long Id { get; set; }
        
        public int AidProgramId { get; set; }
        public AidProgram AidProgram { get; set; }= default!;

        [Required, StringLength(11, MinimumLength =11)]
        [RegularExpression(@"^[1-9][0-9]{10}$", ErrorMessage ="Anne TC kimlik numarası 11 haneli olmalıdır.")]
        public string MotherNationalId { get; set; } = default!;

        [Required, StringLength(50)]
        public string MotherFirstName { get; set; } = default!;

        [Required, StringLength(50)]
        public string MotherLastName { get; set; } = default!;

        [Required]
        [Column(TypeName ="date")]
        public DateTime MotherBirthDate { get; set; }

        [Required]
        [RegularExpression(@"^(?:\+?90)?0?5\d{9}$", ErrorMessage = "Geçerli bir Türk GSM numarası giriniz.")]
        public string PhoneNumber { get; set; } = default!;

        [Required, StringLength(11, MinimumLength = 11)]
        [RegularExpression(@"^[1-9][0-9]{10}$", ErrorMessage = "Bebek TC Kimlik numarası 11 haneli olmalıdır.")]
        public string BabyNationalId { get; set; } = default!;

        public ApplicationStatus Status { get; set; } = ApplicationStatus.Pending;

        public string? RejectionReason { get; set; } 
        public string? Notes { get; set; } 

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; 
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public string? CreatedByIp { get; set; }
        public string? DecisionUserId { get; set; }
        public DateTime? DecisionDate { get; set; }

        public string MotherFullName => $"{MotherFirstName} {MotherLastName}".Trim();
    }
}
