using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BizMate.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class TechnicianBorrowRequests : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TechnicianHoldings_Store_Technician_Product",
                table: "TechnicianHoldings");

            migrationBuilder.AddColumn<int>(
                name: "BorrowType",
                table: "TechnicianHoldings",
                type: "integer",
                nullable: false,
                defaultValue: 2);

            migrationBuilder.AddColumn<int>(
                name: "BorrowType",
                table: "HoldingTransactions",
                type: "integer",
                nullable: false,
                defaultValue: 2);

            migrationBuilder.CreateTable(
                name: "TechnicianBorrowRequests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TechnicianId = table.Column<Guid>(type: "uuid", nullable: false),
                    BorrowType = table.Column<int>(type: "integer", nullable: false),
                    RequestStatus = table.Column<int>(type: "integer", nullable: false),
                    NeededDate = table.Column<DateOnly>(type: "date", nullable: false),
                    ApprovedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ApprovedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    RejectionReason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    RowVersion = table.Column<Guid>(type: "uuid", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    StoreId = table.Column<Guid>(type: "uuid", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    Code = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TechnicianBorrowRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TechnicianBorrowRequests_Stores_StoreId",
                        column: x => x.StoreId,
                        principalTable: "Stores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TechnicianBorrowRequests_Technicians_TechnicianId",
                        column: x => x.TechnicianId,
                        principalTable: "Technicians",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TechnicianBorrowRequestItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TechnicianBorrowRequestId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductName = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    ProductCode = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: true),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    RowVersion = table.Column<Guid>(type: "uuid", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TechnicianBorrowRequestItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TechnicianBorrowRequestItems_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TechnicianBorrowRequestItems_TechnicianBorrowRequests_Techn~",
                        column: x => x.TechnicianBorrowRequestId,
                        principalTable: "TechnicianBorrowRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TechnicianHoldings_Store_Technician_Product_Type",
                table: "TechnicianHoldings",
                columns: new[] { "StoreId", "TechnicianId", "ProductId", "BorrowType" },
                unique: true,
                filter: "\"IsDeleted\" = false");

            migrationBuilder.CreateIndex(
                name: "IX_TechnicianBorrowRequestItems_ProductId",
                table: "TechnicianBorrowRequestItems",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_TechnicianBorrowRequestItems_Request_Product",
                table: "TechnicianBorrowRequestItems",
                columns: new[] { "TechnicianBorrowRequestId", "ProductId" });

            migrationBuilder.CreateIndex(
                name: "IX_TechnicianBorrowRequests_Store_Status_Date",
                table: "TechnicianBorrowRequests",
                columns: new[] { "StoreId", "RequestStatus", "NeededDate" });

            migrationBuilder.CreateIndex(
                name: "IX_TechnicianBorrowRequests_TechnicianId",
                table: "TechnicianBorrowRequests",
                column: "TechnicianId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TechnicianBorrowRequestItems");

            migrationBuilder.DropTable(
                name: "TechnicianBorrowRequests");

            migrationBuilder.DropIndex(
                name: "IX_TechnicianHoldings_Store_Technician_Product_Type",
                table: "TechnicianHoldings");

            migrationBuilder.DropColumn(
                name: "BorrowType",
                table: "TechnicianHoldings");

            migrationBuilder.DropColumn(
                name: "BorrowType",
                table: "HoldingTransactions");

            migrationBuilder.CreateIndex(
                name: "IX_TechnicianHoldings_Store_Technician_Product",
                table: "TechnicianHoldings",
                columns: new[] { "StoreId", "TechnicianId", "ProductId" },
                unique: true,
                filter: "\"IsDeleted\" = false");
        }
    }
}
