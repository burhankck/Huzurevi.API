using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Huzurevi.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class DenetimKayitlari : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LogSaklamaGun",
                table: "UygulamaAyarlari",
                type: "integer",
                nullable: false,
                defaultValue: 365);

            migrationBuilder.CreateTable(
                name: "DenetimKayitlari",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OlusturulmaTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Tur = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Islem = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    Aciklama = table.Column<string>(type: "character varying(400)", maxLength: 400, nullable: false),
                    KullaniciAdi = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: true),
                    KullaniciId = table.Column<int>(type: "integer", nullable: true),
                    IpAdresi = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    DurumKodu = table.Column<int>(type: "integer", nullable: true),
                    Yol = table.Column<string>(type: "character varying(240)", maxLength: 240, nullable: true),
                    HataTipi = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: true),
                    TeknikDetay = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DenetimKayitlari", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "UygulamaAyarlari",
                keyColumn: "Id",
                keyValue: 1,
                column: "LogSaklamaGun",
                value: 365);

            migrationBuilder.CreateIndex(
                name: "IX_DenetimKayitlari_OlusturulmaTarihi",
                table: "DenetimKayitlari",
                column: "OlusturulmaTarihi");

            migrationBuilder.CreateIndex(
                name: "IX_DenetimKayitlari_Tur",
                table: "DenetimKayitlari",
                column: "Tur");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DenetimKayitlari");

            migrationBuilder.DropColumn(
                name: "LogSaklamaGun",
                table: "UygulamaAyarlari");
        }
    }
}
