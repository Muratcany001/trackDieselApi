using System.Collections.Generic;
using System.IO;
using System;
using System.Text.Json;
using BarMenu.Entities;
using BarMenu.Entities.AppEntities;


namespace BarMenu
{
    public static class DataSeeder
    {
        public static void SeedErrors(Context context, string jsonPath)
        {
            try
            {
                Console.WriteLine("JSON yolu: " + jsonPath);

                if (!File.Exists(jsonPath))
                {
                    Console.WriteLine("Dosya bulunamadı!");
                    return;
                }

                var json = File.ReadAllText(jsonPath);
                var errorsRaw = JsonSerializer.Deserialize<List<Error>>(json);

                if (errorsRaw == null || !errorsRaw.Any())
                {
                    Console.WriteLine("JSON verisi boş ya da deserialize edilemedi.");
                    return;
                }

                // 🔁 Aynı Code'a sahip kayıtları teke düşür
                var errors = errorsRaw
                    .GroupBy(e => e.Code)
                    .Select(g => g.First())
                    .ToList();

                var existingCodes = context.Errors.Select(e => e.Code).ToHashSet();
                var newErrors = errors.Where(e => !existingCodes.Contains(e.Code)).ToList();

                Console.WriteLine($"Toplam JSON'dan gelen kayıt: {errors.Count}");
                Console.WriteLine($"Yeni eklenecek kayıt sayısı: {newErrors.Count}");

                context.Errors.AddRange(newErrors);
                context.SaveChanges();

                Console.WriteLine("Veritabanına veri eklendi.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Seed işleminde hata oluştu:");
                Console.WriteLine(ex.ToString());
            }
        }


    }
}
