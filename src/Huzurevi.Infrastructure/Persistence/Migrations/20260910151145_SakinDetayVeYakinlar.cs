using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Huzurevi.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class SakinDetayVeYakinlar : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AcilTelefon",
                table: "Sakinler",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Adres",
                table: "Sakinler",
                type: "character varying(300)",
                maxLength: 300,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Cinsiyet",
                table: "Sakinler",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DogumYeri",
                table: "Sakinler",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "KanGrubu",
                table: "Sakinler",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MedeniDurum",
                table: "Sakinler",
                type: "character varying(30)",
                maxLength: 30,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Notlar",
                table: "Sakinler",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Uyruk",
                table: "Sakinler",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Yakinlar",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SakinId = table.Column<int>(type: "integer", nullable: false),
                    Ad = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Soyad = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Yakinlik = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Telefon = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Eposta = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    Adres = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    AcilDurumKisisiMi = table.Column<bool>(type: "boolean", nullable: false),
                    OlusturulmaTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    GuncellenmeTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    SilindiMi = table.Column<bool>(type: "boolean", nullable: false),
                    SilinmeTarihi = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Yakinlar", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Yakinlar_Sakinler_SakinId",
                        column: x => x.SakinId,
                        principalTable: "Sakinler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Yakinlar_SakinId",
                table: "Yakinlar",
                column: "SakinId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Yakinlar");

            migrationBuilder.DropColumn(
                name: "AcilTelefon",
                table: "Sakinler");

            migrationBuilder.DropColumn(
                name: "Adres",
                table: "Sakinler");

            migrationBuilder.DropColumn(
                name: "Cinsiyet",
                table: "Sakinler");

            migrationBuilder.DropColumn(
                name: "DogumYeri",
                table: "Sakinler");

            migrationBuilder.DropColumn(
                name: "KanGrubu",
                table: "Sakinler");

            migrationBuilder.DropColumn(
                name: "MedeniDurum",
                table: "Sakinler");

            migrationBuilder.DropColumn(
                name: "Notlar",
                table: "Sakinler");

            migrationBuilder.DropColumn(
                name: "Uyruk",
                table: "Sakinler");
        }
    }
}
