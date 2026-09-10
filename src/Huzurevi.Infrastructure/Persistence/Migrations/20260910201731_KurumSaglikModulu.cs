using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Huzurevi.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class KurumSaglikModulu : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "IlacEmirleri",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SakinId = table.Column<int>(type: "integer", nullable: false),
                    IlacAdi = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Doz = table.Column<string>(type: "text", nullable: true),
                    Birim = table.Column<string>(type: "text", nullable: true),
                    KullanimSikligi = table.Column<string>(type: "text", nullable: true),
                    Zamanlama = table.Column<string>(type: "text", nullable: true),
                    BaslangicTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    BitisTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    AktifMi = table.Column<bool>(type: "boolean", nullable: false),
                    KayitTuru = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    OlusturulmaTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    GuncellenmeTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    SilindiMi = table.Column<bool>(type: "boolean", nullable: false),
                    SilinmeTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IlacEmirleri", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IlacEmirleri_Sakinler_SakinId",
                        column: x => x.SakinId,
                        principalTable: "Sakinler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "KurumSaglikKayitlari",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Tur = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    SakinId = table.Column<int>(type: "integer", nullable: true),
                    Tarih = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Personel = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: true),
                    Notlar = table.Column<string>(type: "text", nullable: true),
                    ImzalandiMi = table.Column<bool>(type: "boolean", nullable: false),
                    Imzalayan = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: true),
                    ImzaTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    KanSekeri = table.Column<decimal>(type: "numeric(6,1)", precision: 6, scale: 1, nullable: true),
                    OlcumZamani = table.Column<string>(type: "text", nullable: true),
                    Sistolik = table.Column<int>(type: "integer", nullable: true),
                    Diastolik = table.Column<int>(type: "integer", nullable: true),
                    Nabiz = table.Column<int>(type: "integer", nullable: true),
                    Tedaviler = table.Column<string>(type: "text", nullable: true),
                    DurumDegerlendirme = table.Column<string>(type: "text", nullable: true),
                    HareketKabiliyeti = table.Column<string>(type: "text", nullable: true),
                    GucDenge = table.Column<string>(type: "text", nullable: true),
                    KayitTuru = table.Column<string>(type: "text", nullable: true),
                    YapilanIslemler = table.Column<string>(type: "text", nullable: true),
                    Malzeme = table.Column<string>(type: "text", nullable: true),
                    Nobetci = table.Column<string>(type: "text", nullable: true),
                    GenelDurum = table.Column<string>(type: "text", nullable: true),
                    OnemliOlaylar = table.Column<string>(type: "text", nullable: true),
                    DevirTeslim = table.Column<string>(type: "text", nullable: true),
                    Doktor = table.Column<string>(type: "text", nullable: true),
                    Bulgular = table.Column<string>(type: "text", nullable: true),
                    FizikMuayene = table.Column<string>(type: "text", nullable: true),
                    LabSonuclari = table.Column<string>(type: "text", nullable: true),
                    Oneriler = table.Column<string>(type: "text", nullable: true),
                    OlusturulmaTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    GuncellenmeTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    SilindiMi = table.Column<bool>(type: "boolean", nullable: false),
                    SilinmeTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KurumSaglikKayitlari", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KurumSaglikKayitlari_Sakinler_SakinId",
                        column: x => x.SakinId,
                        principalTable: "Sakinler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "NarkotikIlaclari",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Ad = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    Stok = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: false),
                    Birim = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    AktifMi = table.Column<bool>(type: "boolean", nullable: false),
                    OlusturulmaTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    GuncellenmeTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    SilindiMi = table.Column<bool>(type: "boolean", nullable: false),
                    SilinmeTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NarkotikIlaclari", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "IlacUygulamalari",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IlacEmriId = table.Column<int>(type: "integer", nullable: false),
                    SakinId = table.Column<int>(type: "integer", nullable: false),
                    Tarih = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Durum = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Personel = table.Column<string>(type: "text", nullable: true),
                    Notlar = table.Column<string>(type: "text", nullable: true),
                    OlusturulmaTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    GuncellenmeTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    SilindiMi = table.Column<bool>(type: "boolean", nullable: false),
                    SilinmeTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IlacUygulamalari", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IlacUygulamalari_IlacEmirleri_IlacEmriId",
                        column: x => x.IlacEmriId,
                        principalTable: "IlacEmirleri",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_IlacUygulamalari_Sakinler_SakinId",
                        column: x => x.SakinId,
                        principalTable: "Sakinler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NarkotikHareketleri",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NarkotikIlacId = table.Column<int>(type: "integer", nullable: false),
                    HareketTuru = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Miktar = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: false),
                    Tarih = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    SakinId = table.Column<int>(type: "integer", nullable: true),
                    Notlar = table.Column<string>(type: "text", nullable: true),
                    OlusturulmaTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    GuncellenmeTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    SilindiMi = table.Column<bool>(type: "boolean", nullable: false),
                    SilinmeTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NarkotikHareketleri", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NarkotikHareketleri_NarkotikIlaclari_NarkotikIlacId",
                        column: x => x.NarkotikIlacId,
                        principalTable: "NarkotikIlaclari",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_NarkotikHareketleri_Sakinler_SakinId",
                        column: x => x.SakinId,
                        principalTable: "Sakinler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_IlacEmirleri_SakinId",
                table: "IlacEmirleri",
                column: "SakinId");

            migrationBuilder.CreateIndex(
                name: "IX_IlacUygulamalari_IlacEmriId",
                table: "IlacUygulamalari",
                column: "IlacEmriId");

            migrationBuilder.CreateIndex(
                name: "IX_IlacUygulamalari_SakinId",
                table: "IlacUygulamalari",
                column: "SakinId");

            migrationBuilder.CreateIndex(
                name: "IX_IlacUygulamalari_Tarih",
                table: "IlacUygulamalari",
                column: "Tarih");

            migrationBuilder.CreateIndex(
                name: "IX_KurumSaglikKayitlari_SakinId",
                table: "KurumSaglikKayitlari",
                column: "SakinId");

            migrationBuilder.CreateIndex(
                name: "IX_KurumSaglikKayitlari_Tur_Tarih",
                table: "KurumSaglikKayitlari",
                columns: new[] { "Tur", "Tarih" });

            migrationBuilder.CreateIndex(
                name: "IX_NarkotikHareketleri_NarkotikIlacId",
                table: "NarkotikHareketleri",
                column: "NarkotikIlacId");

            migrationBuilder.CreateIndex(
                name: "IX_NarkotikHareketleri_SakinId",
                table: "NarkotikHareketleri",
                column: "SakinId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IlacUygulamalari");

            migrationBuilder.DropTable(
                name: "KurumSaglikKayitlari");

            migrationBuilder.DropTable(
                name: "NarkotikHareketleri");

            migrationBuilder.DropTable(
                name: "IlacEmirleri");

            migrationBuilder.DropTable(
                name: "NarkotikIlaclari");
        }
    }
}
