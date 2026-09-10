using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Huzurevi.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Yedekleme : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "YedekSaat",
                table: "UygulamaAyarlari",
                type: "integer",
                nullable: false,
                defaultValue: 2);

            migrationBuilder.AddColumn<int>(
                name: "YedekSaklamaAdet",
                table: "UygulamaAyarlari",
                type: "integer",
                nullable: false,
                defaultValue: 14);

            migrationBuilder.AddColumn<bool>(
                name: "YedeklemeAktifMi",
                table: "UygulamaAyarlari",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "YedekKayitlari",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OlusturulmaTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Ad = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    Tur = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Durum = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Boyut = table.Column<long>(type: "bigint", nullable: false),
                    VeritabaniYolu = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: false),
                    DosyaArsivYolu = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: true),
                    HataMesaji = table.Column<string>(type: "text", nullable: true),
                    Olusturan = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_YedekKayitlari", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "UygulamaAyarlari",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "YedekSaat", "YedekSaklamaAdet", "YedeklemeAktifMi" },
                values: new object[] { 2, 14, false });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "YedekKayitlari");

            migrationBuilder.DropColumn(
                name: "YedekSaat",
                table: "UygulamaAyarlari");

            migrationBuilder.DropColumn(
                name: "YedekSaklamaAdet",
                table: "UygulamaAyarlari");

            migrationBuilder.DropColumn(
                name: "YedeklemeAktifMi",
                table: "UygulamaAyarlari");
        }
    }
}
