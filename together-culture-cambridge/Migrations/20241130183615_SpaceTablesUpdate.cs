using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace together_culture_cambridge.Migrations
{
    /// <inheritdoc />
    public partial class SpaceTablesUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Number",
                table: "Space");

            migrationBuilder.RenameColumn(
                name: "TotalSpaces",
                table: "Space",
                newName: "TotalSeats");

            migrationBuilder.AddColumn<string>(
                name: "RoomId",
                table: "Space",
                type: "varchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RoomId",
                table: "Space");

            migrationBuilder.RenameColumn(
                name: "TotalSeats",
                table: "Space",
                newName: "TotalSpaces");

            migrationBuilder.AddColumn<int>(
                name: "Number",
                table: "Space",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
