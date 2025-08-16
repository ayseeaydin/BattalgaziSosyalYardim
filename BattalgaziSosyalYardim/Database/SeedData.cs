using System;
using System.Threading.Tasks;
using BattalgaziSosyalYardim.Entities;
using Microsoft.EntityFrameworkCore;

namespace BattalgaziSosyalYardim.Database
{
    public static class SeedData
    {
        public static async Task RunAsync(AppDbContext db)
        {
            // veritabanını gerekirse migrate et
            await db.Database.MigrateAsync();

            // eğer hiç program yoksa örnekleri ekle:
            if(!await db.AidPrograms.AnyAsync())
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
                        Code="kirtasiye-yardimi",
                        Name="Kırtasiye Yardımı",
                        Description="İhtiyaç sahibi öğrencilere kırtasiye desteği",
                        StartDate = DateTime.UtcNow.Date,
                        IsActive = true
                    }
                );

                await db.SaveChangesAsync();
            }
        }
    }
}
