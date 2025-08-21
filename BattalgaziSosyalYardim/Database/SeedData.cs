using System;
using System.Threading.Tasks;
using BattalgaziSosyalYardim.Entities;
using Microsoft.EntityFrameworkCore;
using BattalgaziSosyalYardim.Security;

namespace BattalgaziSosyalYardim.Database
{
    public static class SeedData
    {
        public static async Task RunAsync(AppDbContext db)
        {
            Console.WriteLine(">>> SeedData başladı");

            // Gerekirse migrate et
            await db.Database.MigrateAsync();

            // Yardım programları
            if (!await db.AidPrograms.AnyAsync())
            {
                db.AidPrograms.AddRange(
                    new AidProgram
                    {
                        Code = "bez-destegi",
                        Name = "Bez Desteği",
                        Description = "0-24 ay bebekler için bebek bezi desteği",
                        StartDate = DateTime.UtcNow.Date,
                        IsActive = true
                    },
                    new AidProgram
                    {
                        Code = "kirtasiye-yardimi",
                        Name = "Kırtasiye Yardımı",
                        Description = "İhtiyaç sahibi öğrencilere kırtasiye desteği",
                        StartDate = DateTime.UtcNow.Date,
                        IsActive = true
                    }
                );

                await db.SaveChangesAsync();
                Console.WriteLine(">>> AidPrograms eklendi.");
            }

            // Admin kullanıcı (geliştirme için varsayılan)
            if (!await db.AdminUsers.AnyAsync())
            {
                var admin = new AdminUser
                {
                    NationalId = "11111111111",             // DEV: ilk girişte değiştirin
                    FirstName = "Sistem",
                    LastName = "Yöneticisi",
                    PasswordHash = PasswordHasher.Hash("Admin!123"), // DEV: ilk girişte değiştirin
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };

                db.AdminUsers.Add(admin);
                await db.SaveChangesAsync();

                Console.WriteLine(">>> Default AdminUser eklendi. TCKN=11111111111 Şifre=Admin!123");
            }

            Console.WriteLine(">>> SeedData bitti");
        }
    }
}
