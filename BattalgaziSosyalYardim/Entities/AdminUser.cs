using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BattalgaziSosyalYardim.Entities
{
    public class AdminUser
    {
        public int Id { get; set; }
        [Required, StringLength(11, MinimumLength = 11)]
        [RegularExpression(@"^[1-9][0-9]{10}$", ErrorMessage = "T.C. Kimlik No 11 haneli olmalıdır.")]
        [Column(TypeName = "varchar(11)")]
        public string NationalId { get; set; } = default!; 

        [Required, StringLength(50)]
        public string FirstName { get; set; } = default!;

        [Required, StringLength(50)]
        public string LastName { get; set; } = default!;

        [Required]
        public string PasswordHash { get; set; } = default!; // PBKDF2 hash

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? LastLoginAt { get; set; }

        [NotMapped]
        public string FullName => $"{FirstName} {LastName}".Trim();
    }
}
