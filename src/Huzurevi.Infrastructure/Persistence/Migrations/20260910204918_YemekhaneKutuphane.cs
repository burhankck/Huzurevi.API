using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Huzurevi.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class YemekhaneKutuphane : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BeslenmeProfilleri",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SakinId = table.Column<int>(type: "integer", nullable: false),
                    TeksturTercihi = table.Column<string>(type: "text", nullable: true),
                    HedefKalori = table.Column<decimal>(type: "numeric", nullable: true),
                    HedefProtein = table.Column<decimal>(type: "numeric", nullable: true),
                    HedefSiviMl = table.Column<decimal>(type: "numeric", nullable: true),
                    Notlar = table.Column<string>(type: "text", nullable: true),
                    OlusturulmaTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    GuncellenmeTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    SilindiMi = table.Column<bool>(type: "boolean", nullable: false),
                    SilinmeTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BeslenmeProfilleri", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BeslenmeProfilleri_Sakinler_SakinId",
                        column: x => x.SakinId,
                        principalTable: "Sakinler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Kitaplar",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Isbn = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Ad = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Yazar = table.Column<string>(type: "text", nullable: true),
                    Yayinevi = table.Column<string>(type: "text", nullable: true),
                    Aciklama = table.Column<string>(type: "text", nullable: true),
                    KapakYolu = table.Column<string>(type: "text", nullable: true),
                    OlusturulmaTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    GuncellenmeTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    SilindiMi = table.Column<bool>(type: "boolean", nullable: false),
                    SilinmeTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Kitaplar", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "KutuphaneDolaplari",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Ad = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    Konum = table.Column<string>(type: "text", nullable: true),
                    AktifMi = table.Column<bool>(type: "boolean", nullable: false),
                    OlusturulmaTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    GuncellenmeTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    SilindiMi = table.Column<bool>(type: "boolean", nullable: false),
                    SilinmeTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KutuphaneDolaplari", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SiviAlimlari",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SakinId = table.Column<int>(type: "integer", nullable: false),
                    Tarih = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    SiviTuru = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    MiktarMl = table.Column<decimal>(type: "numeric(8,1)", precision: 8, scale: 1, nullable: false),
                    OlusturulmaTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    GuncellenmeTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    SilindiMi = table.Column<bool>(type: "boolean", nullable: false),
                    SilinmeTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SiviAlimlari", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SiviAlimlari_Sakinler_SakinId",
                        column: x => x.SakinId,
                        principalTable: "Sakinler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Yemekler",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Ad = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Kategori = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    Kalori = table.Column<decimal>(type: "numeric(8,1)", precision: 8, scale: 1, nullable: true),
                    Protein = table.Column<decimal>(type: "numeric(8,1)", precision: 8, scale: 1, nullable: true),
                    Karbonhidrat = table.Column<decimal>(type: "numeric(8,1)", precision: 8, scale: 1, nullable: true),
                    Yag = table.Column<decimal>(type: "numeric(8,1)", precision: 8, scale: 1, nullable: true),
                    Alerjenler = table.Column<string>(type: "text", nullable: true),
                    Tekstur = table.Column<string>(type: "text", nullable: true),
                    AktifMi = table.Column<bool>(type: "boolean", nullable: false),
                    OlusturulmaTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    GuncellenmeTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    SilindiMi = table.Column<bool>(type: "boolean", nullable: false),
                    SilinmeTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Yemekler", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "KutuphaneRaflari",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DolapId = table.Column<int>(type: "integer", nullable: false),
                    Ad = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    OlusturulmaTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    GuncellenmeTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    SilindiMi = table.Column<bool>(type: "boolean", nullable: false),
                    SilinmeTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KutuphaneRaflari", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KutuphaneRaflari_KutuphaneDolaplari_DolapId",
                        column: x => x.DolapId,
                        principalTable: "KutuphaneDolaplari",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GunlukMenuler",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Tarih = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    OgunTipi = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    YemekId = table.Column<int>(type: "integer", nullable: false),
                    OlusturulmaTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    GuncellenmeTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    SilindiMi = table.Column<bool>(type: "boolean", nullable: false),
                    SilinmeTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GunlukMenuler", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GunlukMenuler_Yemekler_YemekId",
                        column: x => x.YemekId,
                        principalTable: "Yemekler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OzelMenuPlanlari",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SakinId = table.Column<int>(type: "integer", nullable: false),
                    Tarih = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    OgunTipi = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    YemekId = table.Column<int>(type: "integer", nullable: false),
                    TeksturMod = table.Column<string>(type: "text", nullable: true),
                    SiviMl = table.Column<decimal>(type: "numeric", nullable: true),
                    Notlar = table.Column<string>(type: "text", nullable: true),
                    OlusturulmaTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    GuncellenmeTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    SilindiMi = table.Column<bool>(type: "boolean", nullable: false),
                    SilinmeTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OzelMenuPlanlari", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OzelMenuPlanlari_Sakinler_SakinId",
                        column: x => x.SakinId,
                        principalTable: "Sakinler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OzelMenuPlanlari_Yemekler_YemekId",
                        column: x => x.YemekId,
                        principalTable: "Yemekler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "YemekTuketimleri",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SakinId = table.Column<int>(type: "integer", nullable: false),
                    Tarih = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    OgunTipi = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    YemekId = table.Column<int>(type: "integer", nullable: false),
                    TuketildiMi = table.Column<bool>(type: "boolean", nullable: false),
                    TuketilmemeNedeni = table.Column<string>(type: "text", nullable: true),
                    OlusturulmaTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    GuncellenmeTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    SilindiMi = table.Column<bool>(type: "boolean", nullable: false),
                    SilinmeTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_YemekTuketimleri", x => x.Id);
                    table.ForeignKey(
                        name: "FK_YemekTuketimleri_Sakinler_SakinId",
                        column: x => x.SakinId,
                        principalTable: "Sakinler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_YemekTuketimleri_Yemekler_YemekId",
                        column: x => x.YemekId,
                        principalTable: "Yemekler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "KitapKopyalari",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    KitapId = table.Column<int>(type: "integer", nullable: false),
                    Barkod = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    Durum = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    RafId = table.Column<int>(type: "integer", nullable: true),
                    Konum = table.Column<string>(type: "text", nullable: true),
                    OlusturulmaTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    GuncellenmeTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    SilindiMi = table.Column<bool>(type: "boolean", nullable: false),
                    SilinmeTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KitapKopyalari", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KitapKopyalari_Kitaplar_KitapId",
                        column: x => x.KitapId,
                        principalTable: "Kitaplar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_KitapKopyalari_KutuphaneRaflari_RafId",
                        column: x => x.RafId,
                        principalTable: "KutuphaneRaflari",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "KitapOduncleri",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    KopyaId = table.Column<int>(type: "integer", nullable: false),
                    SakinId = table.Column<int>(type: "integer", nullable: false),
                    OduncTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    PlanlananTeslim = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    IadeTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DurumNotu = table.Column<string>(type: "text", nullable: true),
                    OlusturulmaTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    GuncellenmeTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    SilindiMi = table.Column<bool>(type: "boolean", nullable: false),
                    SilinmeTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KitapOduncleri", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KitapOduncleri_KitapKopyalari_KopyaId",
                        column: x => x.KopyaId,
                        principalTable: "KitapKopyalari",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_KitapOduncleri_Sakinler_SakinId",
                        column: x => x.SakinId,
                        principalTable: "Sakinler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BeslenmeProfilleri_SakinId",
                table: "BeslenmeProfilleri",
                column: "SakinId");

            migrationBuilder.CreateIndex(
                name: "IX_GunlukMenuler_Tarih",
                table: "GunlukMenuler",
                column: "Tarih");

            migrationBuilder.CreateIndex(
                name: "IX_GunlukMenuler_YemekId",
                table: "GunlukMenuler",
                column: "YemekId");

            migrationBuilder.CreateIndex(
                name: "IX_KitapKopyalari_KitapId",
                table: "KitapKopyalari",
                column: "KitapId");

            migrationBuilder.CreateIndex(
                name: "IX_KitapKopyalari_RafId",
                table: "KitapKopyalari",
                column: "RafId");

            migrationBuilder.CreateIndex(
                name: "IX_KitapOduncleri_KopyaId",
                table: "KitapOduncleri",
                column: "KopyaId");

            migrationBuilder.CreateIndex(
                name: "IX_KitapOduncleri_SakinId",
                table: "KitapOduncleri",
                column: "SakinId");

            migrationBuilder.CreateIndex(
                name: "IX_KutuphaneRaflari_DolapId",
                table: "KutuphaneRaflari",
                column: "DolapId");

            migrationBuilder.CreateIndex(
                name: "IX_OzelMenuPlanlari_SakinId",
                table: "OzelMenuPlanlari",
                column: "SakinId");

            migrationBuilder.CreateIndex(
                name: "IX_OzelMenuPlanlari_YemekId",
                table: "OzelMenuPlanlari",
                column: "YemekId");

            migrationBuilder.CreateIndex(
                name: "IX_SiviAlimlari_SakinId",
                table: "SiviAlimlari",
                column: "SakinId");

            migrationBuilder.CreateIndex(
                name: "IX_YemekTuketimleri_SakinId",
                table: "YemekTuketimleri",
                column: "SakinId");

            migrationBuilder.CreateIndex(
                name: "IX_YemekTuketimleri_Tarih",
                table: "YemekTuketimleri",
                column: "Tarih");

            migrationBuilder.CreateIndex(
                name: "IX_YemekTuketimleri_YemekId",
                table: "YemekTuketimleri",
                column: "YemekId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BeslenmeProfilleri");

            migrationBuilder.DropTable(
                name: "GunlukMenuler");

            migrationBuilder.DropTable(
                name: "KitapOduncleri");

            migrationBuilder.DropTable(
                name: "OzelMenuPlanlari");

            migrationBuilder.DropTable(
                name: "SiviAlimlari");

            migrationBuilder.DropTable(
                name: "YemekTuketimleri");

            migrationBuilder.DropTable(
                name: "KitapKopyalari");

            migrationBuilder.DropTable(
                name: "Yemekler");

            migrationBuilder.DropTable(
                name: "Kitaplar");

            migrationBuilder.DropTable(
                name: "KutuphaneRaflari");

            migrationBuilder.DropTable(
                name: "KutuphaneDolaplari");
        }
    }
}
