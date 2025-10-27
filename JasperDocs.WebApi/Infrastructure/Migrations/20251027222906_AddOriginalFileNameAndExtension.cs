using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JasperDocs.WebApi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddOriginalFileNameAndExtension : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FileExtension",
                table: "DocumentVersions",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OriginalFileName",
                table: "DocumentVersions",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FileExtension",
                table: "DocumentVersions");

            migrationBuilder.DropColumn(
                name: "OriginalFileName",
                table: "DocumentVersions");
        }
    }
}
