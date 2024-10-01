using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cypherly.UserManagement.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class blocked_user_entity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BlockedUser",
                columns: table => new
                {
                    BlockingUserProfileId = table.Column<Guid>(type: "uuid", nullable: false),
                    BlockedUserProfileId = table.Column<Guid>(type: "uuid", nullable: false),
                    LastModified = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlockedUser", x => new { x.BlockingUserProfileId, x.BlockedUserProfileId });
                    table.ForeignKey(
                        name: "FK_BlockedUser_UserProfile_BlockingUserProfileId",
                        column: x => x.BlockingUserProfileId,
                        principalTable: "UserProfile",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BlockedUser");
        }
    }
}
