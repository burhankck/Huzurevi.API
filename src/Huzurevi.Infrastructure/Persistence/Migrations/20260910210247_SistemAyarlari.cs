using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Huzurevi.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class SistemAyarlari : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "SifreDegistirilmeTarihi",
                table: "Kullanicilar",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "SifreSifirlamaBitis",
                table: "Kullanicilar",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SifreSifirlamaTokenHash",
                table: "Kullanicilar",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "UygulamaAyarlari",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    BakimModu = table.Column<bool>(type: "boolean", nullable: false),
                    SifreMinUzunluk = table.Column<int>(type: "integer", nullable: false),
                    MaksBasarisizGiris = table.Column<int>(type: "integer", nullable: false),
                    OturumDakika = table.Column<int>(type: "integer", nullable: false),
                    SifreGecerlilikGun = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UygulamaAyarlari", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "UygulamaAyarlari",
                columns: new[] { "Id", "BakimModu", "MaksBasarisizGiris", "OturumDakika", "SifreGecerlilikGun", "SifreMinUzunluk" },
                values: new object[] { 1, false, 5, 480, 0, 6 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UygulamaAyarlari");

            migrationBuilder.DropColumn(
                name: "SifreDegistirilmeTarihi",
                table: "Kullanicilar");

            migrationBuilder.DropColumn(
                name: "SifreSifirlamaBitis",
                table: "Kullanicilar");

            migrationBuilder.DropColumn(
                name: "SifreSifirlamaTokenHash",
                table: "Kullanicilar");
        }
    }
}
