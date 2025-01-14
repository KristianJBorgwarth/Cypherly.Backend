using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cypherly.Authentication.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class removed_device_code__status : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DeviceVerificationCode");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Device");

            migrationBuilder.AddColumn<DateTime>(
                name: "LastSeen",
                table: "Device",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastSeen",
                table: "Device");

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Device",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "DeviceVerificationCode",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeviceId = table.Column<Guid>(type: "uuid", nullable: false),
                    LastModified = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Code_ExpirationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Code_IsUsed = table.Column<bool>(type: "boolean", nullable: false),
                    Code = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeviceVerificationCode", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DeviceVerificationCode_Device_DeviceId",
                        column: x => x.DeviceId,
                        principalTable: "Device",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DeviceVerificationCode_Code",
                table: "DeviceVerificationCode",
                column: "Code");

            migrationBuilder.CreateIndex(
                name: "IX_DeviceVerificationCode_Code_ExpirationDate",
                table: "DeviceVerificationCode",
                column: "Code_ExpirationDate");

            migrationBuilder.CreateIndex(
                name: "IX_DeviceVerificationCode_DeviceId",
                table: "DeviceVerificationCode",
                column: "DeviceId");
        }
    }
}
