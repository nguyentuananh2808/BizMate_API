using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BizMate.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedTechnicianRole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                INSERT INTO "Roles"
                    ("Id", "Name", "DisplayName", "IsSystem", "CreatedDate", "RowVersion", "IsDeleted")
                VALUES
                    ('00000000-0000-0000-0000-000000000005',
                     'Technician',
                     'Kỹ thuật viên',
                     TRUE,
                     NOW(),
                     '00000000-0000-0000-0000-000000000105',
                     FALSE)
                ON CONFLICT ("Id") DO UPDATE
                SET "Name" = EXCLUDED."Name",
                    "DisplayName" = EXCLUDED."DisplayName",
                    "IsSystem" = TRUE,
                    "IsDeleted" = FALSE;

                INSERT INTO "RolePermissions"
                    ("Id", "RoleId", "PermissionId", "CreatedDate", "RowVersion", "IsDeleted")
                SELECT
                    CASE p."Name"
                        WHEN 'product.view' THEN '00000000-0000-0000-0000-000000000501'::uuid
                        ELSE '00000000-0000-0000-0000-000000000502'::uuid
                    END,
                    '00000000-0000-0000-0000-000000000005'::uuid,
                    p."Id",
                    NOW(),
                    CASE p."Name"
                        WHEN 'product.view' THEN '00000000-0000-0000-0000-000000000601'::uuid
                        ELSE '00000000-0000-0000-0000-000000000602'::uuid
                    END,
                    FALSE
                FROM "Permissions" p
                WHERE p."Name" IN ('product.view', 'stock.view')
                ON CONFLICT ("RoleId", "PermissionId") DO NOTHING;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                DELETE FROM "RolePermissions"
                WHERE "RoleId" = '00000000-0000-0000-0000-000000000005';

                DELETE FROM "Roles" r
                WHERE r."Id" = '00000000-0000-0000-0000-000000000005'
                  AND NOT EXISTS (
                      SELECT 1
                      FROM "UserRoles" ur
                      WHERE ur."RoleId" = r."Id"
                  );
                """);
        }
    }
}
