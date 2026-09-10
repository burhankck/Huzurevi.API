using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Huzurevi.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class SakinGunlukIzinEmanet : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SakinEmanetleri",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SakinId = table.Column<int>(type: "integer", nullable: false),
                    IslemTuru = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    EmanetTuru = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    Durum = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Adet = table.Column<int>(type: "integer", nullable: false),
                    Deger = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
                    Tarih = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Saat = table.Column<string>(type: "character varying(5)", maxLength: 5, nullable: true),
                    TeslimEden = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    TeslimAlan = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Aciklama = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    OlusturulmaTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    GuncellenmeTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    SilindiMi = table.Column<bool>(type: "boolean", nullable: false),
                    SilinmeTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SakinEmanetleri", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SakinEmanetleri_Sakinler_SakinId",
                        column: x => x.SakinId,
                        principalTable: "Sakinler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SakinGunlukIzinleri",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SakinId = table.Column<int>(type: "integer", nullable: false),
                    Tarih = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Yer = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    CikisSaati = table.Column<string>(type: "character varying(5)", maxLength: 5, nullable: true),
                    DonusSaati = table.Column<string>(type: "character varying(5)", maxLength: 5, nullable: true),
                    AktifMi = table.Column<bool>(type: "boolean", nullable: false),
                    OlusturulmaTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    GuncellenmeTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    SilindiMi = table.Column<bool>(type: "boolean", nullable: false),
                    SilinmeTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SakinGunlukIzinleri", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SakinGunlukIzinleri_Sakinler_SakinId",
                        column: x => x.SakinId,
                        principalTable: "Sakinler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SakinEmanetleri_SakinId",
                table: "SakinEmanetleri",
                column: "SakinId");

            migrationBuilder.CreateIndex(
                name: "IX_SakinGunlukIzinleri_SakinId",
                table: "SakinGunlukIzinleri",
                column: "SakinId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SakinEmanetleri");

            migrationBuilder.DropTable(
                name: "SakinGunlukIzinleri");
        }
    }
}
