using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cypherly.Authentication.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Verification_Code_Type : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "VerificationCode",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(6)",
                oldMaxLength: 6);

            migrationBuilder.AddColumn<string>(
                name: "CodeType",
                table: "VerificationCode",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CodeType",
                table: "VerificationCode");

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "VerificationCode",
                type: "character varying(6)",
                maxLength: 6,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20);
        }
    }
}
