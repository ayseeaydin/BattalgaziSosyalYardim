using Microsoft.EntityFrameworkCore;

namespace BattalgaziSosyalYardim.Veritabani
{
    public class TestDbBaglam : DbContext
    {
        public TestDbBaglam(DbContextOptions<TestDbBaglam>options):base(options){}

        // sadece bağlantıyı test etmek için basit bir tablo
        public DbSet<TestTablo> TestTablolar => Set<TestTablo>();
    }

    public class TestTablo
    {
        public int Id { get; set; }
        public string Ad { get; set; } = string.Empty;
    }
}
