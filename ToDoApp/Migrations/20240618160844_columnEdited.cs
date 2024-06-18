using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ToDoApp.Migrations
{
    public partial class columnEdited : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsActive",
                table: "Todos",
                newName: "Completed");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Completed",
                table: "Todos",
                newName: "IsActive");
        }
    }
}
