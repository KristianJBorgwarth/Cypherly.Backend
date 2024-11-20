using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cypherly.Authentication.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class added_isVerified_column_device : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsVerified",
                table: "Device",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsVerified",
                table: "Device");
        }
    }
}
