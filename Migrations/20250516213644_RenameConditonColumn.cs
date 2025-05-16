using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Wypożyczlania_sprzętu.Migrations
{
    /// <inheritdoc />
    public partial class RenameConditonColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ConditionOnReturn",
                table: "Borrowings",
                newName: "Condition");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Condition",
                table: "Borrowings",
                newName: "ConditionOnReturn");
        }
    }
}
