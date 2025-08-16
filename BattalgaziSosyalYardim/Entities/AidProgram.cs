using System;

namespace BattalgaziSosyalYardim.Entities
{
    public class AidProgram
    {
        public int Id { get; set; }
        public string Code { get; set; } = default!;  // örn:bez-desteği , kırtasiye desteği
        public string Name { get; set; } = default!;
        public string? Description { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsActive { get; set; } = true; // Program aktif mi?
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Oluşturulma tarihi

    }
}
