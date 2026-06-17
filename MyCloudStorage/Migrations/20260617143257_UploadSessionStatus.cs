using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyCloudStorage.Migrations
{
    /// <inheritdoc />
    public partial class UploadSessionStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "UploadSessions",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "StatusMessage",
                table: "UploadSessions",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "UploadSessions");

            migrationBuilder.DropColumn(
                name: "StatusMessage",
                table: "UploadSessions");
        }
    }
}
