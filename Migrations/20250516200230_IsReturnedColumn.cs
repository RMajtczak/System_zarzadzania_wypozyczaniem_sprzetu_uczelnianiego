using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Wypożyczlania_sprzętu.Migrations
{
    /// <inheritdoc />
    public partial class IsReturnedColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsReturned",
                table: "Borrowings",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsReturned",
                table: "Borrowings");
        }
    }
}
