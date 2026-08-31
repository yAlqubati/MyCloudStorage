using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyCloudStorage.Migrations
{
    /// <inheritdoc />
    public partial class addStorageLimitToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "StorageQuota",
                table: "AspNetUsers",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "StorageUsed",
                table: "AspNetUsers",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StorageQuota",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "StorageUsed",
                table: "AspNetUsers");
        }
    }
}
