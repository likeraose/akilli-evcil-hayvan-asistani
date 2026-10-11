using System.Text;
using PetAsistan.Entities;

namespace PetAsistan.Business
{
    // Bir hayvanın bilgilerini yapay zekaya gönderilecek düzenli bir metne çevirir.
    // GİZLİLİK: Sahibin kişisel bilgileri (ad, telefon, adres) bilerek GÖNDERİLMEZ.
    public static class HayvanBaglami
    {
        public static string Olustur(Hayvan hayvan)
        {
            StringBuilder metin = new StringBuilder();

            // ===== TEMEL BİLGİLER =====
            metin.AppendLine("HAYVAN BİLGİLERİ");
            metin.AppendLine($"- Adı: {hayvan.Ad}");
            metin.AppendLine($"- Tür: {hayvan.TurAdi}" +
                             (string.IsNullOrWhiteSpace(hayvan.Cins) ? "" : $", Cins: {hayvan.Cins}"));
            metin.AppendLine($"- Cinsiyet: {(hayvan.Cinsiyet == Cinsiyet.Disi ? "Dişi" : "Erkek")}, " +
                             $"Kısırlaştırılmış: {(hayvan.KisirMi ? "Evet" : "Hayır")}");

            // ÇOK BİÇİMLİLİK: Bu metotlar kedi, köpek ve kuş için FARKLI hesaplıyor
            metin.AppendLine($"- Yaş: {hayvan.Yas} yıl (insan yaşıyla yaklaşık {hayvan.InsanYasinaCevir()})");
            metin.AppendLine($"- Güncel kilo: {hayvan.Kilo} kg, " +
                             $"tahmini günlük enerji ihtiyacı: {hayvan.GunlukKaloriIhtiyaci()} kcal");

            // Türe özgü bilgiler (nesnenin gerçek türüne bakarak)
            if (hayvan is Kopek kopek)
                metin.AppendLine($"- Köpek boyutu: {BoyutAdi(kopek.Boyut)}");

            if (hayvan is Kedi kedi)
                metin.AppendLine($"- Yaşam alanı: {(kedi.EvIciMi ? "Sadece ev içi" : "Dışarı da çıkıyor")}");

            // ===== KİLO GEÇMİŞİ (son 5 ölçüm) =====
            List<KiloKaydi> sonKilolar = hayvan.KiloKayitlari
                .OrderByDescending(k => k.OlcumTarihi)
                .Take(5)
                .OrderBy(k => k.OlcumTarihi)
                .ToList();

            if (sonKilolar.Count > 1)
            {
                metin.AppendLine("- Kilo geçmişi: " +
                    string.Join(", ", sonKilolar.Select(k => $"{k.OlcumTarihi:dd.MM.yyyy}: {k.Kilo} kg")));
            }

            // ===== AŞILAR (her türün en son kaydı) =====
            metin.AppendLine();
            metin.AppendLine("AŞILAR");

            List<AsiKaydi> guncelAsilar = hayvan.AsiKayitlari
                .GroupBy(a => a.AsiTanimiId)
                .Select(grup => grup.OrderByDescending(a => a.UygulamaTarihi).First())
                .ToList();

            if (guncelAsilar.Count == 0)
                metin.AppendLine("- Kayıtlı aşı yok");

            foreach (AsiKaydi asi in guncelAsilar)
            {
                string sonraki = asi.SonrakiAsiTarihi.HasValue
                    ? $", sonraki: {asi.SonrakiAsiTarihi:dd.MM.yyyy}"
                    : "";

                metin.AppendLine($"- {asi.AsiTanimi?.Ad}: son uygulama {asi.UygulamaTarihi:dd.MM.yyyy}{sonraki}");
            }

            // ===== DEVAM EDEN İLAÇLAR =====
            metin.AppendLine();
            metin.AppendLine("DEVAM EDEN İLAÇLAR / TEDAVİLER");

            List<IlacKaydi> aktifIlaclar = hayvan.IlacKayitlari.Where(i => i.DevamEdiyorMu).ToList();

            if (aktifIlaclar.Count == 0)
                metin.AppendLine("- Yok");

            foreach (IlacKaydi ilac in aktifIlaclar)
            {
                string bitis = ilac.BitisTarihi.HasValue ? $", bitiş: {ilac.BitisTarihi:dd.MM.yyyy}" : "";
                metin.AppendLine($"- {ilac.IlacAdi} ({ilac.Doz}, günde {ilac.GunlukKullanimSayisi} kez){bitis}");
            }

            // ===== SON VETERİNER ZİYARETLERİ (son 3) =====
            metin.AppendLine();
            metin.AppendLine("SON VETERİNER ZİYARETLERİ");

            List<VeterinerZiyareti> sonZiyaretler = hayvan.Ziyaretler
                .OrderByDescending(z => z.ZiyaretTarihi)
                .Take(3)
                .ToList();

            if (sonZiyaretler.Count == 0)
                metin.AppendLine("- Kayıtlı ziyaret yok");

            foreach (VeterinerZiyareti ziyaret in sonZiyaretler)
            {
                string teshis = string.IsNullOrWhiteSpace(ziyaret.Teshis) ? "" : $", teşhis: {ziyaret.Teshis}";
                metin.AppendLine($"- {ziyaret.ZiyaretTarihi:dd.MM.yyyy}: şikayet: {ziyaret.Sikayet}{teshis}");
            }

            // ===== NOTLAR =====
            if (!string.IsNullOrWhiteSpace(hayvan.Notlar))
            {
                metin.AppendLine();
                metin.AppendLine($"SAHİBİNİN NOTLARI: {hayvan.Notlar}");
            }

            return metin.ToString();
        }

        private static string BoyutAdi(KopekBoyutu boyut)
        {
            return boyut switch
            {
                KopekBoyutu.Kucuk => "Küçük ırk",
                KopekBoyutu.Orta => "Orta ırk",
                KopekBoyutu.Buyuk => "Büyük ırk",
                _ => "Bilinmiyor"
            };
        }
    }
}