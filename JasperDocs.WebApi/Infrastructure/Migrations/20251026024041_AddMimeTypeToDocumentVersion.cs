using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JasperDocs.WebApi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMimeTypeToDocumentVersion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MimeType",
                table: "DocumentVersions",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MimeType",
                table: "DocumentVersions");
        }
    }
}
