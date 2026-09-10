using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Huzurevi.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class SakinSaglikOlcumDegerlendirme : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SakinOlcumleri",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SakinId = table.Column<int>(type: "integer", nullable: false),
                    Tarih = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    BoyCm = table.Column<decimal>(type: "numeric(6,1)", precision: 6, scale: 1, nullable: true),
                    KiloKg = table.Column<decimal>(type: "numeric(6,1)", precision: 6, scale: 1, nullable: true),
                    KanGrubu = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Aciklama = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    AktifMi = table.Column<bool>(type: "boolean", nullable: false),
                    OlusturulmaTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    GuncellenmeTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    SilindiMi = table.Column<bool>(type: "boolean", nullable: false),
                    SilinmeTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SakinOlcumleri", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SakinOlcumleri_Sakinler_SakinId",
                        column: x => x.SakinId,
                        principalTable: "Sakinler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SakinSaglikDegerlendirmeleri",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SakinId = table.Column<int>(type: "integer", nullable: false),
                    Tur = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Tarih = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Durum = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    Seviye = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Taraf = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    YardimciAracMi = table.Column<bool>(type: "boolean", nullable: false),
                    Aciklama = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    AktifMi = table.Column<bool>(type: "boolean", nullable: false),
                    OlusturulmaTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    GuncellenmeTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    SilindiMi = table.Column<bool>(type: "boolean", nullable: false),
                    SilinmeTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SakinSaglikDegerlendirmeleri", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SakinSaglikDegerlendirmeleri_Sakinler_SakinId",
                        column: x => x.SakinId,
                        principalTable: "Sakinler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SakinOlcumleri_SakinId",
                table: "SakinOlcumleri",
                column: "SakinId");

            migrationBuilder.CreateIndex(
                name: "IX_SakinSaglikDegerlendirmeleri_SakinId_Tur",
                table: "SakinSaglikDegerlendirmeleri",
                columns: new[] { "SakinId", "Tur" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SakinOlcumleri");

            migrationBuilder.DropTable(
                name: "SakinSaglikDegerlendirmeleri");
        }
    }
}
