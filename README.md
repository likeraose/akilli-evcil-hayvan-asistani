\# 🐾 Akıllı Evcil Hayvan Asistanı



Evcil hayvanların sağlık, aşı, ilaç, kilo ve veteriner takibini yapan, \*\*yapay zeka destekli\*\* bir masaüstü uygulaması.



Kullanıcı; hayvanlarını, veteriner ziyaretlerini, aşılarını ve ilaçlarını kaydeder. Uygulama yaklaşan aşı ve kontrol tarihlerini otomatik hatırlatır. Yapay zeka asistanı da hayvanın tüm sağlık geçmişini bilerek soruları cevaplar, belirtileri ön değerlendirmeden geçirir ve beslenme önerisi sunar.



> \*\*Ders:\*\* Nesne Tabanlı Programlama (2. Sınıf) — Dönem Projesi

> \*\*Geliştirici:\*\* Etem İbiş — 225541041



\---



\## ✨ Özellikler



\### Kayıt ve takip

\- \*\*Sahip ve hayvan yönetimi:\*\* Kedi, köpek ve kuş türleri; cins, cinsiyet, kısırlaştırma durumu, fotoğraf

\- \*\*Veteriner ziyaretleri:\*\* Şikayet, teşhis, tedavi, ücret ve sonraki kontrol tarihi

\- \*\*Aşı takibi:\*\* Türe uygun aşı kontrolü, sonraki aşı tarihinin otomatik hesaplanması

\- \*\*İlaç ve tedavi takibi:\*\* Devam eden tedaviler, bitiş tarihi hatırlatması

\- \*\*Kilo takibi:\*\* Kilo geçmişi; hayvanın güncel kilosu en son ölçüme göre otomatik güncellenir

\- \*\*Hatırlatıcılar:\*\* Aşı, kontrol, ilaç ve kullanıcı hatırlatıcıları tek listede (gecikmiş / bugün / bu hafta)



\### Türe özel hesaplamalar

\- Günlük kalori ihtiyacı (veteriner RER formülü, türe ve duruma göre)

\- İnsan yaşına çevirme (kedi, köpek ve kuş için farklı formüller; köpekte boyuta göre)

\- Türe özel kilo sınırları ve bakım tavsiyeleri



\### 🤖 Yapay zeka asistanı (Google Gemini)

| Özellik | Açıklama |

|---|---|

| \*\*Sohbet\*\* | Seçili hayvanın tüm bilgilerini bilerek soruları cevaplar |

| \*\*Semptom ön değerlendirmesi\*\* | Belirtilere göre aciliyet seviyesi, olası nedenler ve uyarı işaretleri |

| \*\*Beslenme önerisi\*\* | Kalori ihtiyacı ve kilo geçmişine göre kişisel öneri |

| \*\*Sağlık özeti\*\* | Veteriner randevusuna götürülebilecek kısa özet |



\*\*Sorumlu kullanım:\*\* Asistan kesin teşhis koymaz ve ilaç dozu önermez. Acil belirtilerde veterinere yönlendirir. Sağlıkla ilgili cevapların sonuna "veteriner muayenesinin yerini tutmaz" uyarısı uygulama tarafından otomatik eklenir. \*\*Gizlilik\*\* için sahiplerin kişisel bilgileri (ad, telefon, adres) yapay zekaya gönderilmez.



\---



\## 🛠 Kullanılan Teknolojiler



| Alan | Teknoloji |

|---|---|

| Dil / Platform | C#, .NET 10, Windows Forms |

| Geliştirme ortamı | Visual Studio 2026 |

| Arayüz | DevExpress WinForms bileşenleri \*(geliştirme aşamasında)\* |

| Veritabanı | SQL Server LocalDB |

| Veri erişimi | Entity Framework Core 10 (Code First + Migration) |

| Yapay zeka | Google Gemini API (`HttpClient` ile REST) |

| Sürüm kontrolü | Git \& GitHub |



\---



\## 🏗 Mimari



Proje \*\*katmanlı mimari\*\* ile geliştirilmiştir. Her katmanın tek bir sorumluluğu vardır:



```

AkilliEvcilHayvanAsistani

├── PetAsistan.Entities     → Varlık sınıfları, doğrulama kuralları, ortak interface'ler

├── PetAsistan.DataAccess   → EF Core DbContext, migration'lar, Repository'ler

├── PetAsistan.Business     → İş kuralları, servisler, merkezi hata yönetimi

├── PetAsistan.AI           → Yapay zeka servis sözleşmesi ve Gemini uygulaması

└── PetAsistan.WinUI        → Kullanıcı arayüzü

```



```

\[WinUI] ──► \[Business] ──► \[DataAccess] ──► SQL Server LocalDB

&#x20;                │

&#x20;                └──► \[AI] ──► Google Gemini API

```



Arayüz yalnızca \*\*servislerle\*\* konuşur. Veritabanını veya yapay zeka servisini doğrudan bilmez. Her işlem, başarı durumunu ve kullanıcıya gösterilecek mesajı taşıyan bir `IslemSonucu` nesnesi döndürür.



\---



\## 🧩 Kullanılan OOP Kavramları



| Kavram | Projedeki karşılığı |

|---|---|

| \*\*Kapsülleme\*\* | `private` alanlar + doğrulama yapan property'ler (örn. telefon formatı, negatif kilo engeli) |

| \*\*Kalıtım\*\* | `BaseEntity` → tüm varlıklar; `Hayvan` → `Kedi`, `Kopek`, `Kus`; `BaseServis` → `HayvanaBagliServis` → `IlacServis`... |

| \*\*Çok biçimlilik\*\* | `GunlukKaloriIhtiyaci()`, `InsanYasinaCevir()`, `BakimTavsiyesi()`, `MaksimumKilo` her türde farklı (`override`) |

| \*\*Soyut sınıflar\*\* | `BaseEntity`, `Hayvan`, `BaseServis`, `HayvanaBagliServis` |

| \*\*Interface'ler\*\* | `IHatirlatilabilir` (4 sınıf uygular), `IRepository<T>`, `IYapayZekaServisi` |

| \*\*Generic yapılar\*\* | `GenericRepository<T>`, `IslemSonucu<T>`, `List<T>`, generic metotlar (`Getir<T>`) |

| \*\*Koleksiyonlar + LINQ\*\* | Bire-çok ilişki listeleri, gruplama, filtreleme, sıralama |

| \*\*İstisna yönetimi\*\* | Özel `DogrulamaException` ve `YapayZekaException`; teknik hatalar merkezi olarak Türkçe mesajlara çevrilir |

| \*\*Bağımlılıkların tersine çevrilmesi\*\* | Servisler repository'leri ve yapay zeka servisini \*\*interface\*\* olarak constructor üzerinden alır |



\---



\## 🗄 Veritabanı Tasarımı



```mermaid

erDiagram

&#x20;   SAHIPLER ||--o{ HAYVANLAR : sahiptir

&#x20;   HAYVANLAR ||--o{ VETERINER\_ZIYARETLERI : yapar

&#x20;   VETERINERLER ||--o{ VETERINER\_ZIYARETLERI : kabul\_eder

&#x20;   HAYVANLAR ||--o{ ASI\_KAYITLARI : olur

&#x20;   ASI\_TANIMLARI ||--o{ ASI\_KAYITLARI : tanimlar

&#x20;   VETERINERLER |o--o{ ASI\_KAYITLARI : uygular

&#x20;   HAYVANLAR ||--o{ ILAC\_KAYITLARI : kullanir

&#x20;   VETERINER\_ZIYARETLERI |o--o{ ILAC\_KAYITLARI : yazar

&#x20;   HAYVANLAR ||--o{ KILO\_KAYITLARI : olculur

&#x20;   HAYVANLAR ||--o{ HATIRLATICILAR : sahiptir

&#x20;   HAYVANLAR |o--o{ AI\_SOHBET\_KAYITLARI : ilgilidir

```



\- \*\*Kalıtımın veritabanına yansıması (TPH):\*\* Kedi, köpek ve kuş tek bir `Hayvanlar` tablosunda saklanır; `Tur` sütunu nesnenin türünü belirler.

\- \*\*Normalizasyon:\*\* Aşı türleri (`AsiTanimlari`) ile yapılan aşılar (`AsiKayitlari`) ayrı tablolardadır. Yaygın aşı türleri veritabanı oluşturulurken otomatik eklenir.



\*\*Silme kuralları:\*\*



| Kural | Örnek |

|---|---|

| \*\*Engelle (Restrict)\*\* | Hayvanları olan sahip, ziyaret kaydı olan veteriner, kullanılmış aşı türü silinemez |

| \*\*Zincirleme sil (Cascade)\*\* | Hayvan silinince aşı, ilaç, kilo, ziyaret ve hatırlatıcı kayıtları da silinir |

| \*\*Boşalt (Set Null)\*\* | Veteriner silinirse aşı kaydı kalır, sadece veteriner bağlantısı boşalır |



\---



\## 🚀 Kurulum ve Çalıştırma



\### Gereksinimler

\- \*\*Visual Studio 2026\*\*, ".NET masaüstü geliştirme" iş yüküyle

\- \*\*SQL Server Express LocalDB\*\* (Visual Studio kurulumunda seçilebilir)

\- \*\*DevExpress WinForms\*\* (30 günlük ücretsiz deneme: \[devexpress.com/products/try](https://www.devexpress.com/products/try/))

\- \*\*Google Gemini API anahtarı\*\* (ücretsiz: \[aistudio.google.com/apikey](https://aistudio.google.com/apikey))



\### Adımlar



\*\*1. Repoyu klonlayın\*\*

```bash

git clone https://github.com/likeraose/akilli-evcil-hayvan-asistani.git

```



\*\*2. Ayar dosyasını oluşturun\*\*



`PetAsistan.WinUI` klasöründeki `appsettings.example.json` dosyasını kopyalayın ve kopyanın adını `appsettings.json` yapın. Ardından `ApiKey` alanına kendi Gemini API anahtarınızı yazın:

```json

"Gemini": {

&#x20; "ApiKey": "KENDI\_ANAHTARINIZ",

&#x20; "Model": "gemini-3.5-flash-lite"

}

```

> `appsettings.json` güvenlik nedeniyle `.gitignore` ile repoya dahil edilmez.



\*\*3. Veritabanını oluşturun\*\*



Visual Studio'da çözümü açın, ardından:

1\. \*\*Araçlar → NuGet Paket Yöneticisi → Paket Yöneticisi Konsolu\*\*'nu açın.

2\. \*\*Varsayılan proje\*\* olarak `PetAsistan.DataAccess` seçin. (Başlangıç projesi `PetAsistan.WinUI` olmalıdır.)

3\. Şu komutu çalıştırın:

```powershell

Update-Database

```



\*\*4. Uygulamayı çalıştırın\*\*



\*\*F5\*\* ile uygulamayı başlatın.



\---



\## 🔒 Güvenlik Notları

\- API anahtarı ve bağlantı dizesi `appsettings.json` dosyasında tutulur ve repoya \*\*yüklenmez\*\*.

\- API anahtarı isteklerde URL'de değil, HTTP başlığında (`x-goog-api-key`) gönderilir.

\- Yapay zekaya yalnızca hayvanla ilgili bilgiler gönderilir.



\---



\## 📋 Proje Durumu

\- \[x] Katmanlı mimari ve varlık sınıfları

\- \[x] Veritabanı tasarımı ve migration

\- \[x] Repository katmanı

\- \[x] İş kuralları ve merkezi hata yönetimi

\- \[x] Yapay zeka entegrasyonu (Gemini)

\- \[ ] DevExpress arayüzü

\- \[ ] Raporlama / dışa aktarma



\---



\## 🤝 Geliştirme Sürecinde Yapay Zeka Kullanımı

Bu projenin geliştirilmesi sırasında, kavramların öğrenilmesi ve kod yazımı aşamalarında yapay zeka araçlarından destek alınmıştır. Ayrıntılar proje raporunda belirtilmiştir.

