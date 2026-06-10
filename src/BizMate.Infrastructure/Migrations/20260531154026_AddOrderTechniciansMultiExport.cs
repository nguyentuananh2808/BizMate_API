using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BizMate.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddOrderTechniciansMultiExport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OrderTechnicians",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OrderId = table.Column<Guid>(type: "uuid", nullable: false),
                    TechnicianId = table.Column<Guid>(type: "uuid", nullable: false),
                    AssignedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderTechnicians", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderTechnicians_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderTechnicians_Technicians_TechnicianId",
                        column: x => x.TechnicianId,
                        principalTable: "Technicians",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrderTechnicians_Order_Technician",
                table: "OrderTechnicians",
                columns: new[] { "OrderId", "TechnicianId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrderTechnicians_TechnicianId",
                table: "OrderTechnicians",
                column: "TechnicianId");

            migrationBuilder.Sql(@"CREATE EXTENSION IF NOT EXISTS pgcrypto;");

            migrationBuilder.Sql(@"
INSERT INTO ""OrderTechnicians"" (""Id"", ""OrderId"", ""TechnicianId"", ""AssignedAt"", ""CreatedDate"")
SELECT gen_random_uuid(), ""Id"", ""TechnicianId"", COALESCE(""CreatedDate"", NOW()), COALESCE(""CreatedDate"", NOW())
FROM ""Orders""
WHERE ""TechnicianId"" IS NOT NULL
ON CONFLICT (""OrderId"", ""TechnicianId"") DO NOTHING;
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrderTechnicians");
        }
    }
}
