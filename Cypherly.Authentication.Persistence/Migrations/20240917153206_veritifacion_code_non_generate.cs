using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cypherly.Authentication.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class veritifacion_code_non_generate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "UserId1",
                table: "VerificationCode",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_VerificationCode_UserId1",
                table: "VerificationCode",
                column: "UserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_VerificationCode_User_UserId1",
                table: "VerificationCode",
                column: "UserId1",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VerificationCode_User_UserId1",
                table: "VerificationCode");

            migrationBuilder.DropIndex(
                name: "IX_VerificationCode_UserId1",
                table: "VerificationCode");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "VerificationCode");
        }
    }
}
