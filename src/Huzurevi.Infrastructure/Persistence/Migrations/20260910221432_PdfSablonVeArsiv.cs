using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Huzurevi.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class PdfSablonVeArsiv : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PdfSablonlari",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Kod = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    Ad = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    Baslik = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: false),
                    Icerik = table.Column<string>(type: "text", nullable: false),
                    AktifMi = table.Column<bool>(type: "boolean", nullable: false),
                    OlusturulmaTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    GuncellenmeTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    SilindiMi = table.Column<bool>(type: "boolean", nullable: false),
                    SilinmeTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PdfSablonlari", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PdfArsivleri",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SablonId = table.Column<int>(type: "integer", nullable: false),
                    SakinId = table.Column<int>(type: "integer", nullable: true),
                    DosyaYolu = table.Column<string>(type: "text", nullable: false),
                    DosyaAdi = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: false),
                    Olusturan = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    OlusturulmaTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    GuncellenmeTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    SilindiMi = table.Column<bool>(type: "boolean", nullable: false),
                    SilinmeTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PdfArsivleri", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PdfArsivleri_PdfSablonlari_SablonId",
                        column: x => x.SablonId,
                        principalTable: "PdfSablonlari",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PdfArsivleri_Sakinler_SakinId",
                        column: x => x.SakinId,
                        principalTable: "Sakinler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PdfArsivleri_SablonId",
                table: "PdfArsivleri",
                column: "SablonId");

            migrationBuilder.CreateIndex(
                name: "IX_PdfArsivleri_SakinId",
                table: "PdfArsivleri",
                column: "SakinId");

            migrationBuilder.CreateIndex(
                name: "IX_PdfSablonlari_Kod",
                table: "PdfSablonlari",
                column: "Kod",
                unique: true,
                filter: "\"SilindiMi\" = FALSE");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PdfArsivleri");

            migrationBuilder.DropTable(
                name: "PdfSablonlari");
        }
    }
}
