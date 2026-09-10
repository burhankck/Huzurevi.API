using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Huzurevi.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class IliskiDuzenlendi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sakinler_Yataklar_YatakId",
                table: "Sakinler");

            migrationBuilder.DropIndex(
                name: "IX_Sakinler_YatakId",
                table: "Sakinler");

            migrationBuilder.AddColumn<int>(
                name: "SakinId",
                table: "Yataklar",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "YatakId2",
                table: "Sakinler",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Yataklar_SakinId",
                table: "Yataklar",
                column: "SakinId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Sakinler_YatakId2",
                table: "Sakinler",
                column: "YatakId2");

            migrationBuilder.AddForeignKey(
                name: "FK_Sakinler_Yataklar_YatakId2",
                table: "Sakinler",
                column: "YatakId2",
                principalTable: "Yataklar",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Yataklar_Sakinler_SakinId",
                table: "Yataklar",
                column: "SakinId",
                principalTable: "Sakinler",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sakinler_Yataklar_YatakId2",
                table: "Sakinler");

            migrationBuilder.DropForeignKey(
                name: "FK_Yataklar_Sakinler_SakinId",
                table: "Yataklar");

            migrationBuilder.DropIndex(
                name: "IX_Yataklar_SakinId",
                table: "Yataklar");

            migrationBuilder.DropIndex(
                name: "IX_Sakinler_YatakId2",
                table: "Sakinler");

            migrationBuilder.DropColumn(
                name: "SakinId",
                table: "Yataklar");

            migrationBuilder.DropColumn(
                name: "YatakId2",
                table: "Sakinler");

            migrationBuilder.CreateIndex(
                name: "IX_Sakinler_YatakId",
                table: "Sakinler",
                column: "YatakId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Sakinler_Yataklar_YatakId",
                table: "Sakinler",
                column: "YatakId",
                principalTable: "Yataklar",
                principalColumn: "Id");
        }
    }
}
