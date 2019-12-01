using Microsoft.EntityFrameworkCore.Migrations;

namespace AirplaneBookingSystem.Migrations
{
    public partial class initial3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OverbookedUsers_AspNetUsers_UserId",
                table: "OverbookedUsers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OverbookedUsers",
                table: "OverbookedUsers");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "OverbookedUsers",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "OverbookedUsers",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OverbookedUsers",
                table: "OverbookedUsers",
                columns: new[] { "FlightId", "Email" });

            migrationBuilder.AddForeignKey(
                name: "FK_OverbookedUsers_AspNetUsers_UserId",
                table: "OverbookedUsers",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OverbookedUsers_AspNetUsers_UserId",
                table: "OverbookedUsers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OverbookedUsers",
                table: "OverbookedUsers");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "OverbookedUsers");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "OverbookedUsers",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_OverbookedUsers",
                table: "OverbookedUsers",
                columns: new[] { "FlightId", "UserId" });

            migrationBuilder.AddForeignKey(
                name: "FK_OverbookedUsers_AspNetUsers_UserId",
                table: "OverbookedUsers",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
