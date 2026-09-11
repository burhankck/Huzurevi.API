using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Huzurevi.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class OdaYatakBakimVeIndeksler : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Yataklar_OdaId",
                table: "Yataklar");

            migrationBuilder.AlterColumn<string>(
                name: "YatakNumarasi",
                table: "Yataklar",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<string>(
                name: "Durum",
                table: "Yataklar",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "Aktif");

            migrationBuilder.AddColumn<string>(
                name: "Ozellikler",
                table: "Yataklar",
                type: "character varying(300)",
                maxLength: 300,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "YatakTipi",
                table: "Yataklar",
                type: "character varying(40)",
                maxLength: 40,
                nullable: false,
                defaultValue: "Standart");

            migrationBuilder.AddColumn<string>(
                name: "Notlar",
                table: "Odalar",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Ozellikler",
                table: "Odalar",
                type: "character varying(300)",
                maxLength: 300,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "OdaBakimlari",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OdaId = table.Column<int>(type: "integer", nullable: false),
                    BakimTuru = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    BakimTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    PersonelId = table.Column<int>(type: "integer", nullable: true),
                    SureDakika = table.Column<int>(type: "integer", nullable: true),
                    Maliyet = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: true),
                    Not = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    OlusturulmaTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    GuncellenmeTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    SilindiMi = table.Column<bool>(type: "boolean", nullable: false),
                    SilinmeTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OdaBakimlari", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OdaBakimlari_Odalar_OdaId",
                        column: x => x.OdaId,
                        principalTable: "Odalar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OdaBakimlari_Personeller_PersonelId",
                        column: x => x.PersonelId,
                        principalTable: "Personeller",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Yataklar_OdaId_DoluMu",
                table: "Yataklar",
                columns: new[] { "OdaId", "DoluMu" });

            migrationBuilder.CreateIndex(
                name: "IX_Sakinler_Ad_Soyad",
                table: "Sakinler",
                columns: new[] { "Ad", "Soyad" });

            migrationBuilder.CreateIndex(
                name: "IX_Sakinler_Durum",
                table: "Sakinler",
                column: "Durum");

            migrationBuilder.CreateIndex(
                name: "IX_Odalar_Blok_Kat",
                table: "Odalar",
                columns: new[] { "Blok", "Kat" });

            migrationBuilder.CreateIndex(
                name: "IX_Odalar_Durum",
                table: "Odalar",
                column: "Durum");

            migrationBuilder.CreateIndex(
                name: "IX_OdaBakimlari_OdaId_BakimTarihi",
                table: "OdaBakimlari",
                columns: new[] { "OdaId", "BakimTarihi" });

            migrationBuilder.CreateIndex(
                name: "IX_OdaBakimlari_PersonelId",
                table: "OdaBakimlari",
                column: "PersonelId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OdaBakimlari");

            migrationBuilder.DropIndex(
                name: "IX_Yataklar_OdaId_DoluMu",
                table: "Yataklar");

            migrationBuilder.DropIndex(
                name: "IX_Sakinler_Ad_Soyad",
                table: "Sakinler");

            migrationBuilder.DropIndex(
                name: "IX_Sakinler_Durum",
                table: "Sakinler");

            migrationBuilder.DropIndex(
                name: "IX_Odalar_Blok_Kat",
                table: "Odalar");

            migrationBuilder.DropIndex(
                name: "IX_Odalar_Durum",
                table: "Odalar");

            migrationBuilder.DropColumn(
                name: "Durum",
                table: "Yataklar");

            migrationBuilder.DropColumn(
                name: "Ozellikler",
                table: "Yataklar");

            migrationBuilder.DropColumn(
                name: "YatakTipi",
                table: "Yataklar");

            migrationBuilder.DropColumn(
                name: "Notlar",
                table: "Odalar");

            migrationBuilder.DropColumn(
                name: "Ozellikler",
                table: "Odalar");

            migrationBuilder.AlterColumn<string>(
                name: "YatakNumarasi",
                table: "Yataklar",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.CreateIndex(
                name: "IX_Yataklar_OdaId",
                table: "Yataklar",
                column: "OdaId");
        }
    }
}
