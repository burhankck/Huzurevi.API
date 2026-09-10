using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Huzurevi.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class KurumYonetimi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PersonelId",
                table: "Kullanicilar",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TcKimlikNo",
                table: "Kullanicilar",
                type: "character varying(11)",
                maxLength: 11,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "GlobalTanimlar",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Kategori = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    Ad = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    Kod = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true),
                    AktifMi = table.Column<bool>(type: "boolean", nullable: false),
                    OlusturulmaTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    GuncellenmeTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    SilindiMi = table.Column<bool>(type: "boolean", nullable: false),
                    SilinmeTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GlobalTanimlar", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Kuruluslar",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Ad = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    KisaAd = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    PlakaKodu = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: false),
                    Adres = table.Column<string>(type: "character varying(400)", maxLength: 400, nullable: true),
                    Telefon = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Dahili = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    AktifMi = table.Column<bool>(type: "boolean", nullable: false),
                    OlusturulmaTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    GuncellenmeTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    SilindiMi = table.Column<bool>(type: "boolean", nullable: false),
                    SilinmeTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Kuruluslar", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Ulkeler",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Ad = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    Kod = table.Column<string>(type: "character varying(8)", maxLength: 8, nullable: false),
                    OlusturulmaTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    GuncellenmeTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    SilindiMi = table.Column<bool>(type: "boolean", nullable: false),
                    SilinmeTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ulkeler", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "KullaniciKuruluslari",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    KullaniciId = table.Column<int>(type: "integer", nullable: false),
                    KurulusId = table.Column<int>(type: "integer", nullable: false),
                    Rol = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    AktifMi = table.Column<bool>(type: "boolean", nullable: false),
                    OlusturulmaTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    GuncellenmeTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    SilindiMi = table.Column<bool>(type: "boolean", nullable: false),
                    SilinmeTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KullaniciKuruluslari", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KullaniciKuruluslari_Kullanicilar_KullaniciId",
                        column: x => x.KullaniciId,
                        principalTable: "Kullanicilar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_KullaniciKuruluslari_Kuruluslar_KurulusId",
                        column: x => x.KurulusId,
                        principalTable: "Kuruluslar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OrganizasyonBirimleri",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    KurulusId = table.Column<int>(type: "integer", nullable: false),
                    UstBirimId = table.Column<int>(type: "integer", nullable: true),
                    Ad = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    Kod = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    AktifMi = table.Column<bool>(type: "boolean", nullable: false),
                    OlusturulmaTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    GuncellenmeTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    SilindiMi = table.Column<bool>(type: "boolean", nullable: false),
                    SilinmeTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganizasyonBirimleri", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrganizasyonBirimleri_Kuruluslar_KurulusId",
                        column: x => x.KurulusId,
                        principalTable: "Kuruluslar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrganizasyonBirimleri_OrganizasyonBirimleri_UstBirimId",
                        column: x => x.UstBirimId,
                        principalTable: "OrganizasyonBirimleri",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Iller",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UlkeId = table.Column<int>(type: "integer", nullable: false),
                    Ad = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    PlakaKodu = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: false),
                    OlusturulmaTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    GuncellenmeTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    SilindiMi = table.Column<bool>(type: "boolean", nullable: false),
                    SilinmeTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Iller", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Iller_Ulkeler_UlkeId",
                        column: x => x.UlkeId,
                        principalTable: "Ulkeler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Ilceler",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IlId = table.Column<int>(type: "integer", nullable: false),
                    Ad = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    OlusturulmaTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    GuncellenmeTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    SilindiMi = table.Column<bool>(type: "boolean", nullable: false),
                    SilinmeTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ilceler", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Ilceler_Iller_IlId",
                        column: x => x.IlId,
                        principalTable: "Iller",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Mahalleler",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IlceId = table.Column<int>(type: "integer", nullable: false),
                    Ad = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    OlusturulmaTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    GuncellenmeTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    SilindiMi = table.Column<bool>(type: "boolean", nullable: false),
                    SilinmeTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Mahalleler", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Mahalleler_Ilceler_IlceId",
                        column: x => x.IlceId,
                        principalTable: "Ilceler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Personeller",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    KurulusId = table.Column<int>(type: "integer", nullable: false),
                    SicilNo = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    TcKimlikNo = table.Column<string>(type: "character varying(11)", maxLength: 11, nullable: true),
                    Ad = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Soyad = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Unvan = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: true),
                    Meslek = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: true),
                    Gorev = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: true),
                    Durum = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    DogumTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Cinsiyet = table.Column<string>(type: "text", nullable: true),
                    KanGrubu = table.Column<string>(type: "text", nullable: true),
                    Eposta = table.Column<string>(type: "text", nullable: true),
                    Telefon = table.Column<string>(type: "text", nullable: true),
                    Adres = table.Column<string>(type: "text", nullable: true),
                    UlkeId = table.Column<int>(type: "integer", nullable: true),
                    IlId = table.Column<int>(type: "integer", nullable: true),
                    IlceId = table.Column<int>(type: "integer", nullable: true),
                    MahalleId = table.Column<int>(type: "integer", nullable: true),
                    IseBaslamaTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    AyrilisTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Notlar = table.Column<string>(type: "text", nullable: true),
                    FotoYolu = table.Column<string>(type: "text", nullable: true),
                    OlusturulmaTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    GuncellenmeTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    SilindiMi = table.Column<bool>(type: "boolean", nullable: false),
                    SilinmeTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Personeller", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Personeller_Ilceler_IlceId",
                        column: x => x.IlceId,
                        principalTable: "Ilceler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Personeller_Iller_IlId",
                        column: x => x.IlId,
                        principalTable: "Iller",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Personeller_Kuruluslar_KurulusId",
                        column: x => x.KurulusId,
                        principalTable: "Kuruluslar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Personeller_Mahalleler_MahalleId",
                        column: x => x.MahalleId,
                        principalTable: "Mahalleler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Personeller_Ulkeler_UlkeId",
                        column: x => x.UlkeId,
                        principalTable: "Ulkeler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "PersonelAtamalari",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PersonelId = table.Column<int>(type: "integer", nullable: false),
                    BirimId = table.Column<int>(type: "integer", nullable: false),
                    Gorev = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: true),
                    BaslangicTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    BitisTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    AktifMi = table.Column<bool>(type: "boolean", nullable: false),
                    OlusturulmaTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    GuncellenmeTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    SilindiMi = table.Column<bool>(type: "boolean", nullable: false),
                    SilinmeTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonelAtamalari", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PersonelAtamalari_OrganizasyonBirimleri_BirimId",
                        column: x => x.BirimId,
                        principalTable: "OrganizasyonBirimleri",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PersonelAtamalari_Personeller_PersonelId",
                        column: x => x.PersonelId,
                        principalTable: "Personeller",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PersonelBelgeleri",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PersonelId = table.Column<int>(type: "integer", nullable: false),
                    BelgeTuru = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    BelgeTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Aciklama = table.Column<string>(type: "text", nullable: true),
                    OrijinalAd = table.Column<string>(type: "text", nullable: false),
                    SaklamaYolu = table.Column<string>(type: "text", nullable: false),
                    IcerikTipi = table.Column<string>(type: "text", nullable: false),
                    Boyut = table.Column<long>(type: "bigint", nullable: false),
                    OlusturulmaTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    GuncellenmeTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    SilindiMi = table.Column<bool>(type: "boolean", nullable: false),
                    SilinmeTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonelBelgeleri", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PersonelBelgeleri_Personeller_PersonelId",
                        column: x => x.PersonelId,
                        principalTable: "Personeller",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PersonelYakinlari",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PersonelId = table.Column<int>(type: "integer", nullable: false),
                    Ad = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Soyad = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Yakinlik = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    Telefon = table.Column<string>(type: "text", nullable: true),
                    Adres = table.Column<string>(type: "text", nullable: true),
                    OlusturulmaTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    GuncellenmeTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    SilindiMi = table.Column<bool>(type: "boolean", nullable: false),
                    SilinmeTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonelYakinlari", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PersonelYakinlari_Personeller_PersonelId",
                        column: x => x.PersonelId,
                        principalTable: "Personeller",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Kullanicilar_PersonelId",
                table: "Kullanicilar",
                column: "PersonelId");

            migrationBuilder.CreateIndex(
                name: "IX_Ilceler_IlId",
                table: "Ilceler",
                column: "IlId");

            migrationBuilder.CreateIndex(
                name: "IX_Iller_UlkeId",
                table: "Iller",
                column: "UlkeId");

            migrationBuilder.CreateIndex(
                name: "IX_KullaniciKuruluslari_KullaniciId_KurulusId",
                table: "KullaniciKuruluslari",
                columns: new[] { "KullaniciId", "KurulusId" },
                unique: true,
                filter: "\"SilindiMi\" = FALSE");

            migrationBuilder.CreateIndex(
                name: "IX_KullaniciKuruluslari_KurulusId",
                table: "KullaniciKuruluslari",
                column: "KurulusId");

            migrationBuilder.CreateIndex(
                name: "IX_Mahalleler_IlceId",
                table: "Mahalleler",
                column: "IlceId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizasyonBirimleri_KurulusId",
                table: "OrganizasyonBirimleri",
                column: "KurulusId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizasyonBirimleri_UstBirimId",
                table: "OrganizasyonBirimleri",
                column: "UstBirimId");

            migrationBuilder.CreateIndex(
                name: "IX_PersonelAtamalari_BirimId",
                table: "PersonelAtamalari",
                column: "BirimId");

            migrationBuilder.CreateIndex(
                name: "IX_PersonelAtamalari_PersonelId",
                table: "PersonelAtamalari",
                column: "PersonelId");

            migrationBuilder.CreateIndex(
                name: "IX_PersonelBelgeleri_PersonelId",
                table: "PersonelBelgeleri",
                column: "PersonelId");

            migrationBuilder.CreateIndex(
                name: "IX_Personeller_IlceId",
                table: "Personeller",
                column: "IlceId");

            migrationBuilder.CreateIndex(
                name: "IX_Personeller_IlId",
                table: "Personeller",
                column: "IlId");

            migrationBuilder.CreateIndex(
                name: "IX_Personeller_KurulusId_SicilNo",
                table: "Personeller",
                columns: new[] { "KurulusId", "SicilNo" },
                unique: true,
                filter: "\"SilindiMi\" = FALSE");

            migrationBuilder.CreateIndex(
                name: "IX_Personeller_MahalleId",
                table: "Personeller",
                column: "MahalleId");

            migrationBuilder.CreateIndex(
                name: "IX_Personeller_UlkeId",
                table: "Personeller",
                column: "UlkeId");

            migrationBuilder.CreateIndex(
                name: "IX_PersonelYakinlari_PersonelId",
                table: "PersonelYakinlari",
                column: "PersonelId");

            migrationBuilder.AddForeignKey(
                name: "FK_Kullanicilar_Personeller_PersonelId",
                table: "Kullanicilar",
                column: "PersonelId",
                principalTable: "Personeller",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Kullanicilar_Personeller_PersonelId",
                table: "Kullanicilar");

            migrationBuilder.DropTable(
                name: "GlobalTanimlar");

            migrationBuilder.DropTable(
                name: "KullaniciKuruluslari");

            migrationBuilder.DropTable(
                name: "PersonelAtamalari");

            migrationBuilder.DropTable(
                name: "PersonelBelgeleri");

            migrationBuilder.DropTable(
                name: "PersonelYakinlari");

            migrationBuilder.DropTable(
                name: "OrganizasyonBirimleri");

            migrationBuilder.DropTable(
                name: "Personeller");

            migrationBuilder.DropTable(
                name: "Kuruluslar");

            migrationBuilder.DropTable(
                name: "Mahalleler");

            migrationBuilder.DropTable(
                name: "Ilceler");

            migrationBuilder.DropTable(
                name: "Iller");

            migrationBuilder.DropTable(
                name: "Ulkeler");

            migrationBuilder.DropIndex(
                name: "IX_Kullanicilar_PersonelId",
                table: "Kullanicilar");

            migrationBuilder.DropColumn(
                name: "PersonelId",
                table: "Kullanicilar");

            migrationBuilder.DropColumn(
                name: "TcKimlikNo",
                table: "Kullanicilar");
        }
    }
}
