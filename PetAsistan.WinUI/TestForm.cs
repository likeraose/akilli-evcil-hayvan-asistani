using PetAsistan.Business;
using PetAsistan.Business.Servisler;
using PetAsistan.Entities;

namespace PetAsistan.WinUI
{
    // GEÇİCİ TEST EKRANI: Arka plan katmanlarını DevExpress kurmadan denemek için.
    // Adım 7'de yerini DevExpress ana ekranı alacak.
    public class TestForm : Form
    {
        // ===== SERVİSLER (arayüz SADECE bunlarla konuşur) =====
        private readonly SahipServis _sahipServis = new SahipServis();
        private readonly HayvanServis _hayvanServis = new HayvanServis();
        private readonly VeterinerServis _veterinerServis = new VeterinerServis();
        private readonly ZiyaretServis _ziyaretServis = new ZiyaretServis();
        private readonly AsiServis _asiServis = new AsiServis();
        private readonly IlacServis _ilacServis = new IlacServis();
        private readonly KiloServis _kiloServis = new KiloServis();
        private readonly HatirlaticiServis _hatirlaticiServis = new HatirlaticiServis();
        private readonly HatirlatmaServis _hatirlatmaServis = new HatirlatmaServis();
        private readonly AsistanServis _asistanServis = new AsistanServis();

        // ===== KONTROLLER =====
        private readonly ComboBox cmbHayvan = new ComboBox();
        private readonly TextBox txtSoru = new TextBox();
        private readonly TextBox txtCevap = new TextBox();
        private readonly FlowLayoutPanel pnlUst = new FlowLayoutPanel();
        private readonly FlowLayoutPanel pnlButonlar = new FlowLayoutPanel();

        public TestForm()
        {
            Text = "Akıllı Evcil Hayvan Asistanı - Test Ekranı";
            Size = new Size(950, 700);
            StartPosition = FormStartPosition.CenterScreen;
            Font = new Font("Segoe UI", 10);

            // --- Üst panel: hayvan seçimi ---
            pnlUst.Dock = DockStyle.Top;
            pnlUst.Height = 45;
            pnlUst.Padding = new Padding(8);
            pnlUst.Controls.Add(new Label { Text = "Hayvan:", AutoSize = true, Padding = new Padding(0, 5, 0, 0) });
            cmbHayvan.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbHayvan.Width = 300;
            pnlUst.Controls.Add(cmbHayvan);

            // --- Soru kutusu ---
            txtSoru.Dock = DockStyle.Top;
            txtSoru.Multiline = true;
            txtSoru.Height = 70;
            txtSoru.PlaceholderText = "Sorunuzu veya gözlemlediğiniz belirtileri buraya yazın...";

            // --- Butonlar ---
            pnlButonlar.Dock = DockStyle.Top;
            pnlButonlar.Height = 90;
            pnlButonlar.Padding = new Padding(4);

            ButonEkle("1) Örnek veri ekle", (s, e) => OrnekVeriEkle());
            ButonEkle("2) Kural testi", (s, e) => KuralTesti());
            ButonEkle("3) Hatırlatmalar", (s, e) => HatirlatmalariGoster());
            ButonEkle("Asistana sor", async (s, e) =>
                await YapayZekaCalistir("Asistanın cevabı",
                    () => _asistanServis.SoruSorAsync(SeciliHayvan?.Id, txtSoru.Text)));
            ButonEkle("Semptom değerlendir", async (s, e) =>
                await HayvanGerektirenYapayZeka("Semptom değerlendirmesi",
                    id => _asistanServis.SemptomDegerlendirAsync(id, txtSoru.Text)));
            ButonEkle("Beslenme önerisi", async (s, e) =>
                await HayvanGerektirenYapayZeka("Beslenme önerisi",
                    id => _asistanServis.BeslenmeOnerisiAsync(id)));
            ButonEkle("Sağlık özeti", async (s, e) =>
                await HayvanGerektirenYapayZeka("Sağlık özeti",
                    id => _asistanServis.SaglikOzetiAsync(id)));
            ButonEkle("Ekranı temizle", (s, e) => txtCevap.Clear());

            // --- Cevap alanı ---
            txtCevap.Dock = DockStyle.Fill;
            txtCevap.Multiline = true;
            txtCevap.ReadOnly = true;
            txtCevap.ScrollBars = ScrollBars.Vertical;
            txtCevap.BackColor = Color.White;

            // Dock düzeni nedeniyle kontroller TERS sırayla eklenir (en alttaki önce)
            Controls.Add(txtCevap);
            Controls.Add(pnlButonlar);
            Controls.Add(txtSoru);
            Controls.Add(pnlUst);

            Load += (s, e) => HayvanlariYukle();
        }

        // Seçili hayvan (ComboBox'ta ToString() sayesinde "Pamuk (Kedi)" görünüyor)
        private Hayvan? SeciliHayvan => cmbHayvan.SelectedItem as Hayvan;

        // =====================================================
        //  1) ÖRNEK VERİ: Tüm servisleri sırayla dener
        // =====================================================
        private void OrnekVeriEkle()
        {
            Yaz("===== ÖRNEK VERİ EKLENİYOR =====");

            Sahip sahip = new Sahip { Ad = "Ayşe", Soyad = "Yılmaz", Telefon = "0532 123 45 67" };
            IslemSonucu sahipSonucu = _sahipServis.Ekle(sahip);
            SonucYaz(sahipSonucu);

            if (!sahipSonucu.BasariliMi)
            {
                Yaz("Örnek veri muhtemelen daha önce eklenmiş. Hayvan listesinden devam edebilirsiniz.");
                return;
            }

            Veteriner veteriner = new Veteriner
            {
                VeterinerAdi = "Dr. Mehmet Kaya",
                KlinikAdi = "Pati Veteriner Kliniği",
                Telefon = "0326 123 45 67"
            };
            SonucYaz(_veterinerServis.Ekle(veteriner));

            // KEDİ ve KÖPEK: Aynı servis, farklı türler (kalıtım + TPH)
            Kedi pamuk = new Kedi
            {
                Ad = "Pamuk",
                Cins = "Tekir",
                Cinsiyet = Cinsiyet.Disi,
                KisirMi = true,
                DogumTarihi = DateTime.Today.AddYears(-3).AddMonths(-2),
                Kilo = 4.8m,
                SahipId = sahip.Id
            };
            SonucYaz(_hayvanServis.Ekle(pamuk));

            Kopek karabas = new Kopek
            {
                Ad = "Karabaş",
                Cins = "Kangal",
                Cinsiyet = Cinsiyet.Erkek,
                Boyut = KopekBoyutu.Buyuk,
                DogumTarihi = DateTime.Today.AddYears(-5),
                Kilo = 45m,
                SahipId = sahip.Id
            };
            SonucYaz(_hayvanServis.Ekle(karabas));

            // KİLO GEÇMİŞİ (Pamuk yavaş yavaş kilo almış)
            SonucYaz(_kiloServis.Ekle(new KiloKaydi
            { HayvanId = pamuk.Id, Kilo = 4.2m, OlcumTarihi = DateTime.Today.AddMonths(-4) }));
            SonucYaz(_kiloServis.Ekle(new KiloKaydi
            { HayvanId = pamuk.Id, Kilo = 4.5m, OlcumTarihi = DateTime.Today.AddMonths(-2) }));

            // AŞILAR: Biri yaklaşıyor, biri gecikmiş (sonraki tarih otomatik hesaplanacak)
            SonucYaz(_asiServis.Ekle(new AsiKaydi
            {
                HayvanId = pamuk.Id,
                AsiTanimiId = 1,   // Kedi Karma Aşısı
                UygulamaTarihi = DateTime.Today.AddYears(-1).AddDays(5),
                VeterinerId = veteriner.Id
            }));
            SonucYaz(_asiServis.Ekle(new AsiKaydi
            {
                HayvanId = pamuk.Id,
                AsiTanimiId = 3,   // Kedi Kuduz Aşısı
                UygulamaTarihi = DateTime.Today.AddDays(-380)
            }));

            // VETERİNER ZİYARETİ
            SonucYaz(_ziyaretServis.Ekle(new VeterinerZiyareti
            {
                HayvanId = pamuk.Id,
                VeterinerId = veteriner.Id,
                ZiyaretTarihi = DateTime.Today.AddDays(-20),
                Sikayet = "Rutin kontrol",
                Teshis = "Hafif kilo artışı",
                Tedavi = "Diyet önerildi",
                Ucret = 750,
                SonrakiKontrolTarihi = DateTime.Today.AddDays(10)
            }));

            // İLAÇ (Karabaş'a eklem desteği)
            SonucYaz(_ilacServis.Ekle(new IlacKaydi
            {
                HayvanId = karabas.Id,
                IlacAdi = "Eklem destek tableti",
                Doz = "1 tablet",
                GunlukKullanimSayisi = 1,
                BaslangicTarihi = DateTime.Today.AddDays(-10),
                BitisTarihi = DateTime.Today.AddDays(20)
            }));

            // KULLANICI HATIRLATICISI
            SonucYaz(_hatirlaticiServis.Ekle(new Hatirlatici
            {
                HayvanId = pamuk.Id,
                Baslik = "Tırnak kesimi",
                Tarih = DateTime.Today.AddDays(3),
                Tur = HatirlaticiTuru.Bakim
            }));

            HayvanlariYukle();
            Yaz("===== BİTTİ =====");
        }

        // =====================================================
        //  2) KURAL TESTİ: Hatalı veriler YAKALANIYOR mu?
        // =====================================================
        private void KuralTesti()
        {
            Yaz("===== KURAL TESTİ (hepsinin ✖ vermesi bekleniyor) =====");

            Hayvan? kedi = (cmbHayvan.DataSource as List<Hayvan>)?.FirstOrDefault(h => h is Kedi);

            if (kedi == null)
            {
                Yaz("Önce örnek veri ekleyin.");
                return;
            }

            // Test 1: Kediye köpek aşısı (Business katmanı yakalamalı)
            Yaz("Test 1 - Kediye köpek kuduz aşısı:");
            SonucYaz(_asiServis.Ekle(new AsiKaydi { HayvanId = kedi.Id, AsiTanimiId = 5 }));

            // Test 2: Aynı telefonla ikinci sahip (Business katmanı yakalamalı)
            Yaz("Test 2 - Aynı telefonla ikinci sahip:");
            SonucYaz(_sahipServis.Ekle(new Sahip { Ad = "Ali", Soyad = "Demir", Telefon = "05321234567" }));

            // Test 3: Hayvanları olan sahibi silmek (Business katmanı yakalamalı)
            Yaz("Test 3 - Hayvanları olan sahibi silmek:");
            SonucYaz(_sahipServis.Sil(kedi.SahipId));

            // Test 4: 60 kiloluk kedi (Entities katmanı, setter içinde yakalamalı)
            Yaz("Test 4 - 60 kg kedi:");
            try
            {
                Kedi devKedi = new Kedi { Kilo = 60 };
                Yaz("✔ (Bu satır görünmemeli!)");
            }
            catch (DogrulamaException ex)
            {
                Yaz("✖ " + ex.Message);
            }

            Yaz("===== BİTTİ =====");
        }

        // =====================================================
        //  3) HATIRLATMALAR: IHatirlatilabilir'in gücü
        // =====================================================
        private void HatirlatmalariGoster()
        {
            Yaz("===== ÖNÜMÜZDEKİ 30 GÜN (ve gecikmiş) HATIRLATMALAR =====");

            IslemSonucu<List<HatirlatmaOgesi>> sonuc = _hatirlatmaServis.YaklasanlariGetir(30);

            if (!sonuc.BasariliMi)
            {
                Yaz("✖ " + sonuc.Mesaj);
                return;
            }

            if (sonuc.Veri!.Count == 0)
                Yaz("Yaklaşan hatırlatma yok.");

            foreach (HatirlatmaOgesi oge in sonuc.Veri)
                Yaz($"[{oge.Durum}] {oge.Tarih:dd.MM.yyyy} ({oge.KalanGun} gün) - {oge.Tur}: {oge.Mesaj}");
        }

        // =====================================================
        //  YAPAY ZEKA
        // =====================================================
        private async Task HayvanGerektirenYapayZeka(string baslik, Func<int, Task<IslemSonucu<string>>> islem)
        {
            if (SeciliHayvan == null)
            {
                Yaz("✖ Lütfen önce bir hayvan seçin.");
                return;
            }

            int hayvanId = SeciliHayvan.Id;
            await YapayZekaCalistir(baslik, () => islem(hayvanId));
        }

        private async Task YapayZekaCalistir(string baslik, Func<Task<IslemSonucu<string>>> islem)
        {
            ButonlariKilitle(true);
            Yaz($"⏳ {baslik} hazırlanıyor, lütfen bekleyin...");

            // await: Gemini düşünürken pencere DONMAZ (taşımayı deneyin!)
            IslemSonucu<string> sonuc = await islem();

            if (sonuc.BasariliMi)
                Yaz($"----- {baslik} -----{Environment.NewLine}{sonuc.Veri}{Environment.NewLine}");
            else
                Yaz("✖ " + sonuc.Mesaj);

            ButonlariKilitle(false);
        }

        // =====================================================
        //  YARDIMCI METOTLAR
        // =====================================================
        private void HayvanlariYukle()
        {
            IslemSonucu<List<Hayvan>> sonuc = _hayvanServis.TumunuGetir();

            if (!sonuc.BasariliMi)
            {
                Yaz("✖ " + sonuc.Mesaj);
                return;
            }

            cmbHayvan.DataSource = sonuc.Veri;
            Yaz($"Veritabanında {sonuc.Veri!.Count} hayvan var.");
        }

        private void ButonEkle(string metin, EventHandler tiklama)
        {
            Button buton = new Button { Text = metin, AutoSize = true, Margin = new Padding(4) };
            buton.Click += tiklama;
            pnlButonlar.Controls.Add(buton);
        }

        private void ButonlariKilitle(bool kilitli)
        {
            foreach (Control kontrol in pnlButonlar.Controls)
                kontrol.Enabled = !kilitli;
        }

        private void SonucYaz(IslemSonucu sonuc)
        {
            Yaz((sonuc.BasariliMi ? "✔ " : "✖ ") + sonuc.Mesaj);
        }

        private void Yaz(string metin)
        {
            // Gemini satır sonlarını "\n" ile gönderiyor, TextBox "\r\n" istiyor
            string duzgun = metin.Replace("\r\n", "\n").Replace("\n", Environment.NewLine);
            txtCevap.AppendText(duzgun + Environment.NewLine);
        }
    }
}