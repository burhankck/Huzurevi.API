using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Huzurevi.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class SakinFotoVasiBelge : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FotoYolu",
                table: "Sakinler",
                type: "character varying(260)",
                maxLength: 260,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "SakinBelgeleri",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SakinId = table.Column<int>(type: "integer", nullable: false),
                    BelgeTuru = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    BelgeTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    GecerlilikTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Aciklama = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    AktifMi = table.Column<bool>(type: "boolean", nullable: false),
                    OrijinalAd = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    SaklamaYolu = table.Column<string>(type: "character varying(260)", maxLength: 260, nullable: false),
                    IcerikTipi = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Boyut = table.Column<long>(type: "bigint", nullable: false),
                    OlusturulmaTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    GuncellenmeTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    SilindiMi = table.Column<bool>(type: "boolean", nullable: false),
                    SilinmeTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SakinBelgeleri", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SakinBelgeleri_Sakinler_SakinId",
                        column: x => x.SakinId,
                        principalTable: "Sakinler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Vasiler",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SakinId = table.Column<int>(type: "integer", nullable: false),
                    Ad = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Soyad = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    TcKimlikNo = table.Column<string>(type: "character varying(11)", maxLength: 11, nullable: true),
                    DogumTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Telefon = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Eposta = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    Adres = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    Yakinlik = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    MahkemeAdi = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    KararNo = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    KararTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    BaslangicTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    BitisTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Kapsam = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    VasiTuru = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Sebep = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Durum = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Aciklama = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    OlusturulmaTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    GuncellenmeTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    SilindiMi = table.Column<bool>(type: "boolean", nullable: false),
                    SilinmeTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vasiler", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Vasiler_Sakinler_SakinId",
                        column: x => x.SakinId,
                        principalTable: "Sakinler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SakinBelgeleri_SakinId",
                table: "SakinBelgeleri",
                column: "SakinId");

            migrationBuilder.CreateIndex(
                name: "IX_Vasiler_SakinId",
                table: "Vasiler",
                column: "SakinId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SakinBelgeleri");

            migrationBuilder.DropTable(
                name: "Vasiler");

            migrationBuilder.DropColumn(
                name: "FotoYolu",
                table: "Sakinler");
        }
    }
}
