using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Huzurevi.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class SakinYerlesimGecmisi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SakinYerlesimleri",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SakinId = table.Column<int>(type: "integer", nullable: false),
                    OdaId = table.Column<int>(type: "integer", nullable: true),
                    YatakId = table.Column<int>(type: "integer", nullable: true),
                    OdaNumarasi = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    YatakNumarasi = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Blok = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Kat = table.Column<int>(type: "integer", nullable: false),
                    GirisTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CikisTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    OlusturulmaTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    GuncellenmeTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    SilindiMi = table.Column<bool>(type: "boolean", nullable: false),
                    SilinmeTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SakinYerlesimleri", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SakinYerlesimleri_Odalar_OdaId",
                        column: x => x.OdaId,
                        principalTable: "Odalar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_SakinYerlesimleri_Sakinler_SakinId",
                        column: x => x.SakinId,
                        principalTable: "Sakinler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SakinYerlesimleri_Yataklar_YatakId",
                        column: x => x.YatakId,
                        principalTable: "Yataklar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SakinYerlesimleri_OdaId",
                table: "SakinYerlesimleri",
                column: "OdaId");

            migrationBuilder.CreateIndex(
                name: "IX_SakinYerlesimleri_SakinId_CikisTarihi",
                table: "SakinYerlesimleri",
                columns: new[] { "SakinId", "CikisTarihi" });

            migrationBuilder.CreateIndex(
                name: "IX_SakinYerlesimleri_YatakId",
                table: "SakinYerlesimleri",
                column: "YatakId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SakinYerlesimleri");
        }
    }
}
