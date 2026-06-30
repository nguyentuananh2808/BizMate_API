using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BizMate.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class BackfillTechnicianUserLinks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                WITH technician_users AS (
                    SELECT DISTINCT
                        u."Id",
                        u."StoreId",
                        u."FullName",
                        u."IsActive",
                        u."CreatedDate"
                    FROM "Users" u
                    LEFT JOIN "UserRoles" ur
                        ON ur."UserId" = u."Id"
                       AND ur."StoreId" = u."StoreId"
                       AND ur."IsDeleted" = FALSE
                    LEFT JOIN "Roles" r
                        ON r."Id" = ur."RoleId"
                       AND r."IsDeleted" = FALSE
                    WHERE u."IsDeleted" = FALSE
                      AND (
                          LOWER(TRIM(u."Role")) = 'technician'
                          OR LOWER(TRIM(r."Name")) = 'technician'
                      )
                ),
                matches AS (
                    SELECT
                        u."Id" AS "UserId",
                        (
                            SELECT t."Id"
                            FROM "Technicians" t
                            WHERE t."StoreId" = u."StoreId"
                              AND t."UserId" IS NULL
                              AND t."IsDeleted" = FALSE
                              AND LOWER(TRIM(t."Name")) = LOWER(TRIM(u."FullName"))
                            ORDER BY t."IsActive" DESC, t."CreatedDate", t."Id"
                            LIMIT 1
                        ) AS "TechnicianId"
                    FROM technician_users u
                    WHERE NOT EXISTS (
                        SELECT 1
                        FROM "Technicians" linked
                        WHERE linked."UserId" = u."Id"
                          AND linked."IsDeleted" = FALSE
                    )
                )
                UPDATE "Technicians" t
                SET "UserId" = m."UserId",
                    "UpdatedDate" = NOW()
                FROM matches m
                WHERE m."TechnicianId" IS NOT NULL
                  AND t."Id" = m."TechnicianId"
                  AND t."UserId" IS NULL;

                WITH technician_users AS (
                    SELECT DISTINCT
                        u."Id",
                        u."StoreId",
                        u."FullName",
                        u."IsActive",
                        u."CreatedDate"
                    FROM "Users" u
                    LEFT JOIN "UserRoles" ur
                        ON ur."UserId" = u."Id"
                       AND ur."StoreId" = u."StoreId"
                       AND ur."IsDeleted" = FALSE
                    LEFT JOIN "Roles" r
                        ON r."Id" = ur."RoleId"
                       AND r."IsDeleted" = FALSE
                    WHERE u."IsDeleted" = FALSE
                      AND (
                          LOWER(TRIM(u."Role")) = 'technician'
                          OR LOWER(TRIM(r."Name")) = 'technician'
                      )
                )
                INSERT INTO "Technicians"
                    ("Id", "UserId", "Name", "Phone", "ZaloPhone",
                     "CreatedDate", "CreatedBy", "RowVersion", "IsDeleted",
                     "StoreId", "IsActive", "Code")
                SELECT
                    MD5(u."Id"::text || ':technician')::uuid,
                    u."Id",
                    u."FullName",
                    NULL,
                    NULL,
                    COALESCE(u."CreatedDate", NOW()),
                    u."Id",
                    MD5(u."Id"::text || ':technician-row')::uuid,
                    FALSE,
                    u."StoreId",
                    u."IsActive",
                    '#KT-' || UPPER(SUBSTRING(REPLACE(u."Id"::text, '-', ''), 1, 8))
                FROM technician_users u
                WHERE NOT EXISTS (
                    SELECT 1
                    FROM "Technicians" t
                    WHERE t."UserId" = u."Id"
                );
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // User links may already be referenced by borrowing data, so backfill is intentionally irreversible.
        }
    }
}
