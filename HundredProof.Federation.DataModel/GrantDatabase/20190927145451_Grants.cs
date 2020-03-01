using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace HundredProof.Federation.DataModel.GrantDatabase {
    public partial class Grants : Migration {
        protected override void Up(MigrationBuilder migrationBuilder) {
            migrationBuilder.CreateTable(
                "DeviceCodes",
                table => new {
                    UserCode = table.Column<string>(maxLength: 200),
                    DeviceCode = table.Column<string>(maxLength: 200),
                    SubjectId = table.Column<string>(maxLength: 200, nullable: true),
                    ClientId = table.Column<string>(maxLength: 200),
                    CreationTime = table.Column<DateTime>(),
                    Expiration = table.Column<DateTime>(),
                    Data = table.Column<string>(maxLength: 50000)
                },
                constraints: table => { table.PrimaryKey("PK_DeviceCodes", x => x.UserCode); });

            migrationBuilder.CreateTable(
                "PersistedGrants",
                table => new {
                    Key = table.Column<string>(maxLength: 200),
                    Type = table.Column<string>(maxLength: 50),
                    SubjectId = table.Column<string>(maxLength: 200, nullable: true),
                    ClientId = table.Column<string>(maxLength: 200),
                    CreationTime = table.Column<DateTime>(),
                    Expiration = table.Column<DateTime>(nullable: true),
                    Data = table.Column<string>(maxLength: 50000)
                },
                constraints: table => { table.PrimaryKey("PK_PersistedGrants", x => x.Key); });

            migrationBuilder.CreateIndex(
                "IX_DeviceCodes_DeviceCode",
                "DeviceCodes",
                "DeviceCode",
                unique: true);

            migrationBuilder.CreateIndex(
                "IX_DeviceCodes_Expiration",
                "DeviceCodes",
                "Expiration");

            migrationBuilder.CreateIndex(
                "IX_PersistedGrants_Expiration",
                "PersistedGrants",
                "Expiration");

            migrationBuilder.CreateIndex(
                "IX_PersistedGrants_SubjectId_ClientId_Type",
                "PersistedGrants",
                new[] {"SubjectId", "ClientId", "Type"});
        }

        protected override void Down(MigrationBuilder migrationBuilder) {
            migrationBuilder.DropTable(
                "DeviceCodes");

            migrationBuilder.DropTable(
                "PersistedGrants");
        }
    }
}