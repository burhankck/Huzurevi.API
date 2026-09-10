using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Huzurevi.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class SakinDemografikAlanlar : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AnaAdi",
                table: "Sakinler",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "AylikGelir",
                table: "Sakinler",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AyrilisDurumu",
                table: "Sakinler",
                type: "character varying(40)",
                maxLength: 40,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "AyrilisTarihi",
                table: "Sakinler",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BabaAdi",
                table: "Sakinler",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BasvuruDurumu",
                table: "Sakinler",
                type: "character varying(40)",
                maxLength: 40,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EngelDurumu",
                table: "Sakinler",
                type: "character varying(40)",
                maxLength: 40,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "KabulNedeni",
                table: "Sakinler",
                type: "character varying(80)",
                maxLength: 80,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "KabulSekli",
                table: "Sakinler",
                type: "character varying(40)",
                maxLength: 40,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "KayitNo",
                table: "Sakinler",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "KayitTuru",
                table: "Sakinler",
                type: "character varying(40)",
                maxLength: 40,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "KimGetirdi",
                table: "Sakinler",
                type: "character varying(150)",
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Meslek",
                table: "Sakinler",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NeredenGeldigi",
                table: "Sakinler",
                type: "character varying(150)",
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NufusKutukIli",
                table: "Sakinler",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OgrenimDurumu",
                table: "Sakinler",
                type: "character varying(40)",
                maxLength: 40,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OncekiYasamYeri",
                table: "Sakinler",
                type: "character varying(150)",
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SonOturduguAdres",
                table: "Sakinler",
                type: "character varying(300)",
                maxLength: 300,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UcretDurumu",
                table: "Sakinler",
                type: "character varying(40)",
                maxLength: 40,
                nullable: true);

            migrationBuilder.Sql("""
                UPDATE "Sakinler"
                SET "KayitNo" = 'SKN-' || LPAD("Id"::text, 6, '0')
                WHERE "KayitNo" IS NULL OR "KayitNo" = '';
                """);

            migrationBuilder.CreateIndex(
                name: "IX_Sakinler_KayitNo",
                table: "Sakinler",
                column: "KayitNo",
                unique: true,
                filter: "\"SilindiMi\" = FALSE AND \"KayitNo\" IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Sakinler_KayitNo",
                table: "Sakinler");

            migrationBuilder.DropColumn(
                name: "AnaAdi",
                table: "Sakinler");

            migrationBuilder.DropColumn(
                name: "AylikGelir",
                table: "Sakinler");

            migrationBuilder.DropColumn(
                name: "AyrilisDurumu",
                table: "Sakinler");

            migrationBuilder.DropColumn(
                name: "AyrilisTarihi",
                table: "Sakinler");

            migrationBuilder.DropColumn(
                name: "BabaAdi",
                table: "Sakinler");

            migrationBuilder.DropColumn(
                name: "BasvuruDurumu",
                table: "Sakinler");

            migrationBuilder.DropColumn(
                name: "EngelDurumu",
                table: "Sakinler");

            migrationBuilder.DropColumn(
                name: "KabulNedeni",
                table: "Sakinler");

            migrationBuilder.DropColumn(
                name: "KabulSekli",
                table: "Sakinler");

            migrationBuilder.DropColumn(
                name: "KayitNo",
                table: "Sakinler");

            migrationBuilder.DropColumn(
                name: "KayitTuru",
                table: "Sakinler");

            migrationBuilder.DropColumn(
                name: "KimGetirdi",
                table: "Sakinler");

            migrationBuilder.DropColumn(
                name: "Meslek",
                table: "Sakinler");

            migrationBuilder.DropColumn(
                name: "NeredenGeldigi",
                table: "Sakinler");

            migrationBuilder.DropColumn(
                name: "NufusKutukIli",
                table: "Sakinler");

            migrationBuilder.DropColumn(
                name: "OgrenimDurumu",
                table: "Sakinler");

            migrationBuilder.DropColumn(
                name: "OncekiYasamYeri",
                table: "Sakinler");

            migrationBuilder.DropColumn(
                name: "SonOturduguAdres",
                table: "Sakinler");

            migrationBuilder.DropColumn(
                name: "UcretDurumu",
                table: "Sakinler");
        }
    }
}
