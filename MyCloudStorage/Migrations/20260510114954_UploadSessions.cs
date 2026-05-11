using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyCloudStorage.Migrations
{
    /// <inheritdoc />
    public partial class UploadSessions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UploadSessions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FileName = table.Column<string>(type: "text", nullable: false),
                    FileType = table.Column<string>(type: "text", nullable: false),
                    TotalSize = table.Column<long>(type: "bigint", nullable: false),
                    TotalChunks = table.Column<int>(type: "integer", nullable: false),
                    ReceivedChunks = table.Column<int>(type: "integer", nullable: false),
                    FolderId = table.Column<Guid>(type: "uuid", nullable: true),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    TempDirectory = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UploadSessions", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UploadSessions");
        }
    }
}
