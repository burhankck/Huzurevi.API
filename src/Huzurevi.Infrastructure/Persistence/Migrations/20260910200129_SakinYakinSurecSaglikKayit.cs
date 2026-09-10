using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Huzurevi.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class SakinYakinSurecSaglikKayit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Aciklama",
                table: "Yakinlar",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Meslek",
                table: "Yakinlar",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Oncelik",
                table: "Yakinlar",
                type: "integer",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<string>(
                name: "Grup",
                table: "SakinBelgeleri",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "Genel");

            migrationBuilder.CreateTable(
                name: "SakinSaglikKayitlari",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SakinId = table.Column<int>(type: "integer", nullable: false),
                    Tur = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Tarih = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Ad = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    DurumTipi = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true),
                    BaslangicTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    BitisTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    AmeliyatTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Hastane = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    Komplikasyon = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    MarkaModel = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    SeriNo = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: true),
                    TeminTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    KontrolTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Taraf = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Derece = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true),
                    Bolge = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: true),
                    Teshis = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    Doz = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: true),
                    KullanimSikligi = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: true),
                    UygulamaYolu = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: true),
                    ReceteliMi = table.Column<bool>(type: "boolean", nullable: false),
                    ReceteNo = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: true),
                    ZamanlamaTipi = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true),
                    ZamanDilimleri = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    ReaksiyonTipi = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: true),
                    Aciklama = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    AktifMi = table.Column<bool>(type: "boolean", nullable: false),
                    OlusturulmaTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    GuncellenmeTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    SilindiMi = table.Column<bool>(type: "boolean", nullable: false),
                    SilinmeTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SakinSaglikKayitlari", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SakinSaglikKayitlari_Sakinler_SakinId",
                        column: x => x.SakinId,
                        principalTable: "Sakinler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SakinSaglikKayitlari_SakinId_Tur",
                table: "SakinSaglikKayitlari",
                columns: new[] { "SakinId", "Tur" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SakinSaglikKayitlari");

            migrationBuilder.DropColumn(
                name: "Aciklama",
                table: "Yakinlar");

            migrationBuilder.DropColumn(
                name: "Meslek",
                table: "Yakinlar");

            migrationBuilder.DropColumn(
                name: "Oncelik",
                table: "Yakinlar");

            migrationBuilder.DropColumn(
                name: "Grup",
                table: "SakinBelgeleri");
        }
    }
}
