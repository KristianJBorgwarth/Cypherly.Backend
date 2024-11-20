using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cypherly.Authentication.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class remove_isVerified_column : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsVerified",
                table: "Device");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsVerified",
                table: "Device",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
