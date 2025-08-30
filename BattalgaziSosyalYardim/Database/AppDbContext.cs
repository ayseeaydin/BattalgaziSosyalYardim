using BattalgaziSosyalYardim.Entities;
using Microsoft.EntityFrameworkCore;

namespace BattalgaziSosyalYardim.Database
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<AidProgram> AidPrograms => Set<AidProgram>();
        public DbSet<Application> Applications => Set<Application>();
        public DbSet<AdminUser> AdminUsers => Set<AdminUser>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<AidProgram>()
                .HasIndex(p => p.Code)
                .IsUnique();

            modelBuilder.Entity<Application>()
                .Property(a => a.Status)
                .HasConversion<int>();

            modelBuilder.Entity<Application>()
                .HasIndex(a => new { a.AidProgramId, a.BabyNationalId })
                .IsUnique();

            modelBuilder.Entity<Application>()
                .Property(a => a.MotherNationalId)
                .HasColumnType("char(11)");

            modelBuilder.Entity<Application>()
                .Property(a => a.BabyNationalId)
                .HasColumnType("char(11)");

            modelBuilder.Entity<Application>()
                .Property(a => a.MotherBirthDate)
                .HasColumnType("date");

            modelBuilder.Entity<Application>()
                .Property(a => a.MotherFirstName).HasMaxLength(50);
            modelBuilder.Entity<Application>()
                .Property(a => a.MotherLastName).HasMaxLength(50);

            modelBuilder.Entity<Application>().HasIndex(a => a.Status);
            modelBuilder.Entity<Application>().HasIndex(a => a.CreatedAt);


            modelBuilder.Entity<Application>()
                .ToTable(tb =>
                {
                    tb.HasCheckConstraint("ck_mother_national_id",
                        "\"MotherNationalId\" ~ '^[1-9][0-9]{10}$'");
                    tb.HasCheckConstraint("ck_baby_national_id",
                        "\"BabyNationalId\" ~ '^[1-9][0-9]{10}$'");
                    tb.HasCheckConstraint("ck_phone_number",
                        "\"PhoneNumber\" ~ '^(?:\\+?90)?0?5[0-9]{9}$'");
                });

            modelBuilder.Entity<AdminUser>()
                .HasIndex(u => u.NationalId)
                .IsUnique();

            modelBuilder.Entity<AdminUser>()
                .Property(u => u.NationalId)
                .HasColumnType("varchar(11)");

            modelBuilder.Entity<AdminUser>()
                .Property(u => u.FirstName).HasMaxLength(50);

            modelBuilder.Entity<AdminUser>()
                .Property(u => u.LastName).HasMaxLength(50);
        }
    }
}
