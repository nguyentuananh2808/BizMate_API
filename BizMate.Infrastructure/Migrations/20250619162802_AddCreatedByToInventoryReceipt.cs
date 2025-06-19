using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BizMate.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCreatedByToInventoryReceipt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CreatedByUserId",
                table: "InventoryReceipts",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_InventoryReceipts_CreatedByUserId",
                table: "InventoryReceipts",
                column: "CreatedByUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryReceipts_Users_CreatedByUserId",
                table: "InventoryReceipts",
                column: "CreatedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InventoryReceipts_Users_CreatedByUserId",
                table: "InventoryReceipts");

            migrationBuilder.DropIndex(
                name: "IX_InventoryReceipts_CreatedByUserId",
                table: "InventoryReceipts");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "InventoryReceipts");
        }
    }
}
