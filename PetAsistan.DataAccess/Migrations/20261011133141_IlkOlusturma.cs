using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace PetAsistan.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class IlkOlusturma : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AsiTanimlari",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Ad = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    HedefTur = table.Column<int>(type: "int", nullable: false),
                    TekrarAraligiGun = table.Column<int>(type: "int", nullable: false),
                    Aciklama = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    OlusturmaTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GuncellemeTarihi = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AsiTanimlari", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Sahipler",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Ad = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Soyad = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Telefon = table.Column<string>(type: "nvarchar(11)", maxLength: 11, nullable: false),
                    Eposta = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Adres = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    OlusturmaTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GuncellemeTarihi = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sahipler", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Veterinerler",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VeterinerAdi = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    KlinikAdi = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Telefon = table.Column<string>(type: "nvarchar(11)", maxLength: 11, nullable: false),
                    Adres = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    OlusturmaTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GuncellemeTarihi = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Veterinerler", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Hayvanlar",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Ad = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DogumTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Kilo = table.Column<decimal>(type: "decimal(7,3)", precision: 7, scale: 3, nullable: false),
                    Cins = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Cinsiyet = table.Column<int>(type: "int", nullable: false),
                    KisirMi = table.Column<bool>(type: "bit", nullable: false),
                    Fotograf = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    Notlar = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    SahipId = table.Column<int>(type: "int", nullable: false),
                    Tur = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: false),
                    EvIciMi = table.Column<bool>(type: "bit", nullable: true),
                    Boyut = table.Column<int>(type: "int", nullable: true),
                    OlusturmaTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GuncellemeTarihi = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Hayvanlar", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Hayvanlar_Sahipler_SahipId",
                        column: x => x.SahipId,
                        principalTable: "Sahipler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AiSohbetKayitlari",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HayvanId = table.Column<int>(type: "int", nullable: true),
                    IslemTuru = table.Column<int>(type: "int", nullable: false),
                    Soru = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    Cevap = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OlusturmaTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GuncellemeTarihi = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AiSohbetKayitlari", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AiSohbetKayitlari_Hayvanlar_HayvanId",
                        column: x => x.HayvanId,
                        principalTable: "Hayvanlar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "AsiKayitlari",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HayvanId = table.Column<int>(type: "int", nullable: false),
                    AsiTanimiId = table.Column<int>(type: "int", nullable: false),
                    VeterinerId = table.Column<int>(type: "int", nullable: true),
                    UygulamaTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SonrakiAsiTarihi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SeriNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Notlar = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    OlusturmaTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GuncellemeTarihi = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AsiKayitlari", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AsiKayitlari_AsiTanimlari_AsiTanimiId",
                        column: x => x.AsiTanimiId,
                        principalTable: "AsiTanimlari",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AsiKayitlari_Hayvanlar_HayvanId",
                        column: x => x.HayvanId,
                        principalTable: "Hayvanlar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AsiKayitlari_Veterinerler_VeterinerId",
                        column: x => x.VeterinerId,
                        principalTable: "Veterinerler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Hatirlaticilar",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HayvanId = table.Column<int>(type: "int", nullable: false),
                    Baslik = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Aciklama = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Tarih = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Tur = table.Column<int>(type: "int", nullable: false),
                    TamamlandiMi = table.Column<bool>(type: "bit", nullable: false),
                    OlusturmaTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GuncellemeTarihi = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Hatirlaticilar", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Hatirlaticilar_Hayvanlar_HayvanId",
                        column: x => x.HayvanId,
                        principalTable: "Hayvanlar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "KiloKayitlari",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HayvanId = table.Column<int>(type: "int", nullable: false),
                    OlcumTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Kilo = table.Column<decimal>(type: "decimal(7,3)", precision: 7, scale: 3, nullable: false),
                    Notlar = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    OlusturmaTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GuncellemeTarihi = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KiloKayitlari", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KiloKayitlari_Hayvanlar_HayvanId",
                        column: x => x.HayvanId,
                        principalTable: "Hayvanlar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VeterinerZiyaretleri",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HayvanId = table.Column<int>(type: "int", nullable: false),
                    VeterinerId = table.Column<int>(type: "int", nullable: false),
                    ZiyaretTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Sikayet = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Teshis = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Tedavi = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Ucret = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    SonrakiKontrolTarihi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    OlusturmaTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GuncellemeTarihi = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VeterinerZiyaretleri", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VeterinerZiyaretleri_Hayvanlar_HayvanId",
                        column: x => x.HayvanId,
                        principalTable: "Hayvanlar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VeterinerZiyaretleri_Veterinerler_VeterinerId",
                        column: x => x.VeterinerId,
                        principalTable: "Veterinerler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "IlacKayitlari",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HayvanId = table.Column<int>(type: "int", nullable: false),
                    VeterinerZiyaretiId = table.Column<int>(type: "int", nullable: true),
                    IlacAdi = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Doz = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    GunlukKullanimSayisi = table.Column<int>(type: "int", nullable: false),
                    BaslangicTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BitisTarihi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Notlar = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    OlusturmaTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GuncellemeTarihi = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IlacKayitlari", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IlacKayitlari_Hayvanlar_HayvanId",
                        column: x => x.HayvanId,
                        principalTable: "Hayvanlar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_IlacKayitlari_VeterinerZiyaretleri_VeterinerZiyaretiId",
                        column: x => x.VeterinerZiyaretiId,
                        principalTable: "VeterinerZiyaretleri",
                        principalColumn: "Id");
                });

            migrationBuilder.InsertData(
                table: "AsiTanimlari",
                columns: new[] { "Id", "Aciklama", "Ad", "GuncellemeTarihi", "HedefTur", "OlusturmaTarihi", "TekrarAraligiGun" },
                values: new object[,]
                {
                    { 1, "Panlökopeni, herpes ve calici virüslerine karşı koruma sağlar.", "Kedi Karma Aşısı", null, 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 365 },
                    { 2, "Kedi lösemi virüsüne karşı koruma sağlar.", "Kedi Lösemi (FeLV) Aşısı", null, 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 365 },
                    { 3, "Kuduz hastalığına karşı yıllık aşı.", "Kedi Kuduz Aşısı", null, 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 365 },
                    { 4, "Gençlik hastalığı (distemper), parvo, hepatit ve parainfluenzaya karşı koruma sağlar.", "Köpek Karma Aşısı", null, 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 365 },
                    { 5, "Kuduz hastalığına karşı yıllık aşı.", "Köpek Kuduz Aşısı", null, 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 365 },
                    { 6, "Bulaşıcı köpek öksürüğüne (kennel cough) karşı koruma sağlar.", "Köpek Bronşin Aşısı", null, 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 365 },
                    { 7, "Leptospiroz bakteriyel enfeksiyonuna karşı koruma sağlar.", "Köpek Leptospiroz Aşısı", null, 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 365 },
                    { 8, "Bağırsak parazitlerine karşı, genellikle 3 ayda bir tekrarlanır.", "İç Parazit Uygulaması", null, 0, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 90 },
                    { 9, "Pire ve keneye karşı, genellikle ayda bir tekrarlanır.", "Dış Parazit Uygulaması", null, 0, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 30 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AiSohbetKayitlari_HayvanId",
                table: "AiSohbetKayitlari",
                column: "HayvanId");

            migrationBuilder.CreateIndex(
                name: "IX_AsiKayitlari_AsiTanimiId",
                table: "AsiKayitlari",
                column: "AsiTanimiId");

            migrationBuilder.CreateIndex(
                name: "IX_AsiKayitlari_HayvanId",
                table: "AsiKayitlari",
                column: "HayvanId");

            migrationBuilder.CreateIndex(
                name: "IX_AsiKayitlari_VeterinerId",
                table: "AsiKayitlari",
                column: "VeterinerId");

            migrationBuilder.CreateIndex(
                name: "IX_AsiTanimlari_Ad",
                table: "AsiTanimlari",
                column: "Ad",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Hatirlaticilar_HayvanId",
                table: "Hatirlaticilar",
                column: "HayvanId");

            migrationBuilder.CreateIndex(
                name: "IX_Hayvanlar_SahipId",
                table: "Hayvanlar",
                column: "SahipId");

            migrationBuilder.CreateIndex(
                name: "IX_IlacKayitlari_HayvanId",
                table: "IlacKayitlari",
                column: "HayvanId");

            migrationBuilder.CreateIndex(
                name: "IX_IlacKayitlari_VeterinerZiyaretiId",
                table: "IlacKayitlari",
                column: "VeterinerZiyaretiId");

            migrationBuilder.CreateIndex(
                name: "IX_KiloKayitlari_HayvanId",
                table: "KiloKayitlari",
                column: "HayvanId");

            migrationBuilder.CreateIndex(
                name: "IX_Sahipler_Telefon",
                table: "Sahipler",
                column: "Telefon",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VeterinerZiyaretleri_HayvanId",
                table: "VeterinerZiyaretleri",
                column: "HayvanId");

            migrationBuilder.CreateIndex(
                name: "IX_VeterinerZiyaretleri_VeterinerId",
                table: "VeterinerZiyaretleri",
                column: "VeterinerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AiSohbetKayitlari");

            migrationBuilder.DropTable(
                name: "AsiKayitlari");

            migrationBuilder.DropTable(
                name: "Hatirlaticilar");

            migrationBuilder.DropTable(
                name: "IlacKayitlari");

            migrationBuilder.DropTable(
                name: "KiloKayitlari");

            migrationBuilder.DropTable(
                name: "AsiTanimlari");

            migrationBuilder.DropTable(
                name: "VeterinerZiyaretleri");

            migrationBuilder.DropTable(
                name: "Hayvanlar");

            migrationBuilder.DropTable(
                name: "Veterinerler");

            migrationBuilder.DropTable(
                name: "Sahipler");
        }
    }
}
