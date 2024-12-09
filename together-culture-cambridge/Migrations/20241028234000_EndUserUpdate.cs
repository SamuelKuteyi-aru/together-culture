using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace together_culture_cambridge.Migrations
{
    /// <inheritdoc />
    public partial class EndUserUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Approved",
                table: "EndUser",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Approved",
                table: "EndUser");
        }
    }
}
