using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace trackDieselApi.Migrations
{
    /// <inheritdoc />
    public partial class mig_3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IssueType",
                table: "Issue");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "IssueType",
                table: "Issue",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
