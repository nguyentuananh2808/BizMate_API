using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BizMate.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class LinkTechnicianUserAndMergeBorrowTypes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "Technicians",
                type: "uuid",
                nullable: true);

            migrationBuilder.Sql(
                """
                UPDATE "TechnicianHoldings" AS target
                SET "Quantity" = target."Quantity" + source."Quantity",
                    "LastBorrowedAt" = GREATEST(target."LastBorrowedAt", source."LastBorrowedAt"),
                    "UpdatedDate" = NOW()
                FROM "TechnicianHoldings" AS source
                WHERE target."StoreId" = source."StoreId"
                  AND target."TechnicianId" = source."TechnicianId"
                  AND target."ProductId" = source."ProductId"
                  AND target."BorrowType" = 2
                  AND source."BorrowType" = 3;

                DELETE FROM "TechnicianHoldings" AS source
                USING "TechnicianHoldings" AS target
                WHERE source."StoreId" = target."StoreId"
                  AND source."TechnicianId" = target."TechnicianId"
                  AND source."ProductId" = target."ProductId"
                  AND source."BorrowType" = 3
                  AND target."BorrowType" = 2;

                UPDATE "TechnicianHoldings"
                SET "BorrowType" = 2,
                    "UpdatedDate" = NOW()
                WHERE "BorrowType" = 3;

                UPDATE "HoldingTransactions"
                SET "BorrowType" = 2
                WHERE "BorrowType" = 3;

                UPDATE "TechnicianBorrowRequests"
                SET "BorrowType" = 2,
                    "UpdatedDate" = NOW()
                WHERE "BorrowType" = 3;
                """);

            migrationBuilder.CreateIndex(
                name: "IX_Technicians_UserId",
                table: "Technicians",
                column: "UserId",
                unique: true,
                filter: "\"UserId\" IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Technicians_Users_UserId",
                table: "Technicians",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Technicians_Users_UserId",
                table: "Technicians");

            migrationBuilder.DropIndex(
                name: "IX_Technicians_UserId",
                table: "Technicians");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Technicians");
        }
    }
}
