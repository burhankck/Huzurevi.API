using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Huzurevi.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class KurumSurecleri : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EsyaTespitleri",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SakinId = table.Column<int>(type: "integer", nullable: false),
                    Kategori = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    Adet = table.Column<int>(type: "integer", nullable: false),
                    Aciklama = table.Column<string>(type: "text", nullable: true),
                    TespitTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    OnayDurumu = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Onaylayan = table.Column<string>(type: "text", nullable: true),
                    OnayTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    OnayNotu = table.Column<string>(type: "text", nullable: true),
                    OlusturulmaTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    GuncellenmeTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    SilindiMi = table.Column<bool>(type: "boolean", nullable: false),
                    SilinmeTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EsyaTespitleri", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EsyaTespitleri_Sakinler_SakinId",
                        column: x => x.SakinId,
                        principalTable: "Sakinler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "IzinSurecleri",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SakinId = table.Column<int>(type: "integer", nullable: false),
                    IzinTuru = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    BaslangicTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    BitisTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    TeslimAlan = table.Column<string>(type: "text", nullable: true),
                    Notlar = table.Column<string>(type: "text", nullable: true),
                    OnayDurumu = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Onaylayan = table.Column<string>(type: "text", nullable: true),
                    OnayTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    OnayNotu = table.Column<string>(type: "text", nullable: true),
                    OlusturulmaTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    GuncellenmeTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    SilindiMi = table.Column<bool>(type: "boolean", nullable: false),
                    SilinmeTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IzinSurecleri", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IzinSurecleri_Sakinler_SakinId",
                        column: x => x.SakinId,
                        principalTable: "Sakinler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MirasciTeslimleri",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SakinId = table.Column<int>(type: "integer", nullable: false),
                    MirasciId = table.Column<int>(type: "integer", nullable: true),
                    TeslimAlan = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    TeslimAlanTelefon = table.Column<string>(type: "text", nullable: true),
                    TeslimTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    EsyaOzeti = table.Column<string>(type: "text", nullable: true),
                    Notlar = table.Column<string>(type: "text", nullable: true),
                    EvrakYolu = table.Column<string>(type: "text", nullable: true),
                    OnayDurumu = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Onaylayan = table.Column<string>(type: "text", nullable: true),
                    OnayTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    OnayNotu = table.Column<string>(type: "text", nullable: true),
                    OlusturulmaTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    GuncellenmeTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    SilindiMi = table.Column<bool>(type: "boolean", nullable: false),
                    SilinmeTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MirasciTeslimleri", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MirasciTeslimleri_Mirascilar_MirasciId",
                        column: x => x.MirasciId,
                        principalTable: "Mirascilar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_MirasciTeslimleri_Sakinler_SakinId",
                        column: x => x.SakinId,
                        principalTable: "Sakinler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OnayYetkileri",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    KullaniciId = table.Column<int>(type: "integer", nullable: false),
                    Alan = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    AktifMi = table.Column<bool>(type: "boolean", nullable: false),
                    OlusturulmaTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    GuncellenmeTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    SilindiMi = table.Column<bool>(type: "boolean", nullable: false),
                    SilinmeTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OnayYetkileri", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OnayYetkileri_Kullanicilar_KullaniciId",
                        column: x => x.KullaniciId,
                        principalTable: "Kullanicilar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PsikolojikDegerlendirmeler",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SakinId = table.Column<int>(type: "integer", nullable: false),
                    Tarih = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Tur = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Puan = table.Column<int>(type: "integer", nullable: true),
                    SosyalDurum = table.Column<string>(type: "text", nullable: true),
                    UzmanGorusu = table.Column<string>(type: "text", nullable: true),
                    Oneriler = table.Column<string>(type: "text", nullable: true),
                    OnayDurumu = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Onaylayan = table.Column<string>(type: "text", nullable: true),
                    OnayTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    OnayNotu = table.Column<string>(type: "text", nullable: true),
                    OlusturulmaTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    GuncellenmeTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    SilindiMi = table.Column<bool>(type: "boolean", nullable: false),
                    SilinmeTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PsikolojikDegerlendirmeler", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PsikolojikDegerlendirmeler_Sakinler_SakinId",
                        column: x => x.SakinId,
                        principalTable: "Sakinler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SosyalIncelemeler",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SakinId = table.Column<int>(type: "integer", nullable: true),
                    Tarih = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Durum = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    BasvuruYapan = table.Column<string>(type: "text", nullable: true),
                    TcKimlikNo = table.Column<string>(type: "text", nullable: true),
                    AdSoyad = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    DogumTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DogumYeri = table.Column<string>(type: "text", nullable: true),
                    Cinsiyet = table.Column<string>(type: "text", nullable: true),
                    MedeniDurum = table.Column<string>(type: "text", nullable: true),
                    EgitimDurumu = table.Column<string>(type: "text", nullable: true),
                    Telefon = table.Column<string>(type: "text", nullable: true),
                    Eposta = table.Column<string>(type: "text", nullable: true),
                    Adres = table.Column<string>(type: "text", nullable: true),
                    GelirKaynagi = table.Column<string>(type: "text", nullable: true),
                    AylikGelir = table.Column<string>(type: "text", nullable: true),
                    SosyalGuvence = table.Column<string>(type: "text", nullable: true),
                    Mulk = table.Column<string>(type: "text", nullable: true),
                    AileUyeleri = table.Column<string>(type: "text", nullable: true),
                    YakinlikDereceleri = table.Column<string>(type: "text", nullable: true),
                    Iletisim = table.Column<string>(type: "text", nullable: true),
                    KronikHastaliklar = table.Column<string>(type: "text", nullable: true),
                    KullanilanIlaclar = table.Column<string>(type: "text", nullable: true),
                    EngelDurumu = table.Column<string>(type: "text", nullable: true),
                    BakimIhtiyaci = table.Column<string>(type: "text", nullable: true),
                    UzmanGorusu = table.Column<string>(type: "text", nullable: true),
                    Oneri = table.Column<string>(type: "text", nullable: true),
                    Sonuc = table.Column<string>(type: "text", nullable: true),
                    OnayDurumu = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Onaylayan = table.Column<string>(type: "text", nullable: true),
                    OnayTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    OnayNotu = table.Column<string>(type: "text", nullable: true),
                    OlusturulmaTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    GuncellenmeTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    SilindiMi = table.Column<bool>(type: "boolean", nullable: false),
                    SilinmeTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SosyalIncelemeler", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SosyalIncelemeler_Sakinler_SakinId",
                        column: x => x.SakinId,
                        principalTable: "Sakinler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EsyaTespitleri_SakinId",
                table: "EsyaTespitleri",
                column: "SakinId");

            migrationBuilder.CreateIndex(
                name: "IX_IzinSurecleri_SakinId",
                table: "IzinSurecleri",
                column: "SakinId");

            migrationBuilder.CreateIndex(
                name: "IX_MirasciTeslimleri_MirasciId",
                table: "MirasciTeslimleri",
                column: "MirasciId");

            migrationBuilder.CreateIndex(
                name: "IX_MirasciTeslimleri_SakinId",
                table: "MirasciTeslimleri",
                column: "SakinId");

            migrationBuilder.CreateIndex(
                name: "IX_OnayYetkileri_KullaniciId_Alan",
                table: "OnayYetkileri",
                columns: new[] { "KullaniciId", "Alan" });

            migrationBuilder.CreateIndex(
                name: "IX_PsikolojikDegerlendirmeler_SakinId",
                table: "PsikolojikDegerlendirmeler",
                column: "SakinId");

            migrationBuilder.CreateIndex(
                name: "IX_SosyalIncelemeler_SakinId",
                table: "SosyalIncelemeler",
                column: "SakinId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EsyaTespitleri");

            migrationBuilder.DropTable(
                name: "IzinSurecleri");

            migrationBuilder.DropTable(
                name: "MirasciTeslimleri");

            migrationBuilder.DropTable(
                name: "OnayYetkileri");

            migrationBuilder.DropTable(
                name: "PsikolojikDegerlendirmeler");

            migrationBuilder.DropTable(
                name: "SosyalIncelemeler");
        }
    }
}
