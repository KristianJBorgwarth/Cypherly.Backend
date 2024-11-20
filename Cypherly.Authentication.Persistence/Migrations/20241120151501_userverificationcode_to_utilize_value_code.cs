using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cypherly.Authentication.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class userverificationcode_to_utilize_value_code : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsUsed",
                table: "UserVerificationCode",
                newName: "Code_IsUsed");

            migrationBuilder.RenameColumn(
                name: "ExpirationDate",
                table: "UserVerificationCode",
                newName: "Code_ExpirationDate");

            migrationBuilder.CreateIndex(
                name: "IX_UserVerificationCode_Code",
                table: "UserVerificationCode",
                column: "Code");

            migrationBuilder.CreateIndex(
                name: "IX_UserVerificationCode_Code_ExpirationDate",
                table: "UserVerificationCode",
                column: "Code_ExpirationDate");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UserVerificationCode_Code",
                table: "UserVerificationCode");

            migrationBuilder.DropIndex(
                name: "IX_UserVerificationCode_Code_ExpirationDate",
                table: "UserVerificationCode");

            migrationBuilder.RenameColumn(
                name: "Code_IsUsed",
                table: "UserVerificationCode",
                newName: "IsUsed");

            migrationBuilder.RenameColumn(
                name: "Code_ExpirationDate",
                table: "UserVerificationCode",
                newName: "ExpirationDate");
        }
    }
}
