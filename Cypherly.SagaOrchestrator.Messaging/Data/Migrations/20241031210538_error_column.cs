using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cypherly.SagaOrchestrator.Messaging.Data.Migrations
{
    /// <inheritdoc />
    public partial class error_column : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "UserDeleteSaga",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<string>(
                name: "Error",
                table: "UserDeleteSaga",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Error",
                table: "UserDeleteSaga");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "UserDeleteSaga",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);
        }
    }
}
