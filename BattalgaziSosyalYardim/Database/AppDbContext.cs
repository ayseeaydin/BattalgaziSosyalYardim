using BattalgaziSosyalYardim.Entities;
using Microsoft.EntityFrameworkCore;

namespace BattalgaziSosyalYardim.Database
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<AidProgram> AidPrograms => Set<AidProgram>();
        public DbSet<Application> Applications => Set<Application>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // AidProgram
            modelBuilder.Entity<AidProgram>()
                .HasIndex(p => p.Code)
                .IsUnique();

            // Application: enum int olarak kalsın
            modelBuilder.Entity<Application>()
                .Property(a => a.Status)
                .HasConversion<int>();

            // Unique: aynı program + aynı bebek tekil olsun
            modelBuilder.Entity<Application>()
                .HasIndex(a => new { a.AidProgramId, a.BabyNationalId })
                .IsUnique();

            // Kolon tipleri
            modelBuilder.Entity<Application>()
                .Property(a => a.MotherNationalId)
                .HasColumnType("char(11)");

            modelBuilder.Entity<Application>()
                .Property(a => a.BabyNationalId)
                .HasColumnType("char(11)");

            // Application: string uzunlukları (DB tarafında garanti altına alalım)
            modelBuilder.Entity<Application>()
                .Property(a => a.MotherFirstName).HasMaxLength(50);
            modelBuilder.Entity<Application>()
                .Property(a => a.MotherLastName).HasMaxLength(50);

            // Application: sık kullanılan alanlara indeks
            modelBuilder.Entity<Application>().HasIndex(a => a.Status);
            modelBuilder.Entity<Application>().HasIndex(a => a.CreatedAt);


            // check constraints
            modelBuilder.Entity<Application>()
                .ToTable(tb =>
                {
                    tb.HasCheckConstraint("ck_mother_national_id",
                        "MotherNationalId ~ '^[1-9][0-9]{10}$'");
                    tb.HasCheckConstraint("ck_baby_national_id",
                        "BabyNationalId ~ '^[1-9][0-9]{10}$'");
                    tb.HasCheckConstraint("ck_phone_number",
                        "PhoneNumber ~ '^(?:\\+?90)?0?5[0-9]{9}$'");
                });
        }
    }
}
