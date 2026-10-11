using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using PetAsistan.Entities;

namespace PetAsistan.Business.Servisler
{
    // Tüm servislerin temeli. abstract: Tek başına kullanılmaz, sadece miras verilir.
    // Görevi: try/catch'i TEK YERDE yazmak ve teknik hataları anlaşılır mesajlara çevirmek.
    public abstract class BaseServis
    {
        // Ekle / Güncelle / Sil gibi VERİ DÖNDÜRMEYEN işlemleri güvenle çalıştırır.
        // Action: "Parametre almayan, değer döndürmeyen bir iş paketi"
        protected IslemSonucu Calistir(Action islem, string basariMesaji)
        {
            try
            {
                islem();    // Gönderilen iş paketini çalıştır
                return IslemSonucu.Basarili(basariMesaji);
            }
            catch (Exception ex)
            {
                return IslemSonucu.Hatali(HataMesajiUret(ex));
            }
        }

        // Listele / Getir gibi VERİ DÖNDÜREN işlemleri güvenle çalıştırır.
        // Func<T>: "Parametre almayan, T türünde değer döndüren bir iş paketi"
        protected IslemSonucu<T> Getir<T>(Func<T> islem)
        {
            try
            {
                T veri = islem();
                return IslemSonucu<T>.Basarili(veri);
            }
            catch (Exception ex)
            {
                return IslemSonucu<T>.Hatali(HataMesajiUret(ex));
            }
        }

        // Teknik hatayı kullanıcının anlayacağı Türkçe bir mesaja çevirir.
        // virtual: Gerekirse bir servis kendi özel mesajlarını üretebilir (override).
        protected virtual string HataMesajiUret(Exception ex)
        {
            // 1) Kullanıcının yanlış veri girmesi → bizim yazdığımız mesaj zaten anlaşılır
            if (ex is DogrulamaException)
                return ex.Message;

            // 2) Veritabanı kayıt hataları
            if (ex is DbUpdateException && ex.InnerException is SqlException sqlHatasi)
            {
                // 547: Yabancı anahtar ihlali (Restrict kuralı: bağlı kayıt var)
                if (sqlHatasi.Number == 547)
                    return "Bu kayıt başka kayıtlarda kullanıldığı için silinemez veya değiştirilemez.";

                // 2601 / 2627: Benzersizlik ihlali (aynı telefon, aynı aşı adı vb.)
                if (sqlHatasi.Number == 2601 || sqlHatasi.Number == 2627)
                    return "Bu bilgilerle kayıtlı başka bir kayıt zaten var.";

                return "Kayıt sırasında bir veritabanı hatası oluştu.";
            }

            // 3) Veritabanına hiç bağlanılamadı (LocalDB kapalı vb.)
            if (ex is SqlException)
                return "Veritabanına bağlanılamadı. Lütfen veritabanı sunucusunun çalıştığından emin olun.";

            // 4) Bizim fırlattığımız "işlem yapılamaz" hataları (örn: "Kayıt bulunamadı")
            if (ex is InvalidOperationException)
                return ex.Message;

            // 5) Hiçbirine uymadıysa
            return "Beklenmeyen bir hata oluştu: " + ex.Message;
        }
    }
}