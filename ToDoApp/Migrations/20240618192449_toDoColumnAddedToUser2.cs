using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ToDoApp.Migrations
{
    public partial class toDoColumnAddedToUser2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "CustomUserId",
                table: "Todos",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_Todos_CustomUserId",
                table: "Todos",
                column: "CustomUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Todos_AspNetUsers_CustomUserId",
                table: "Todos",
                column: "CustomUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Todos_AspNetUsers_CustomUserId",
                table: "Todos");

            migrationBuilder.DropIndex(
                name: "IX_Todos_CustomUserId",
                table: "Todos");

            migrationBuilder.AlterColumn<string>(
                name: "CustomUserId",
                table: "Todos",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }
    }
}
