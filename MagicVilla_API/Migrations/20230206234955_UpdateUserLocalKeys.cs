using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MagicVillaAPI.Migrations
{
    /// <inheritdoc />
    public partial class UpdateUserLocalKeys : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LocalUserRole_LocalUser_LocalUserId",
                table: "LocalUserRole");

            migrationBuilder.AlterColumn<int>(
                name: "LocalUserId",
                table: "LocalUserRole",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_LocalUserRole_LocalUser_LocalUserId",
                table: "LocalUserRole",
                column: "LocalUserId",
                principalTable: "LocalUser",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LocalUserRole_LocalUser_LocalUserId",
                table: "LocalUserRole");

            migrationBuilder.AlterColumn<int>(
                name: "LocalUserId",
                table: "LocalUserRole",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_LocalUserRole_LocalUser_LocalUserId",
                table: "LocalUserRole",
                column: "LocalUserId",
                principalTable: "LocalUser",
                principalColumn: "Id");
        }
    }
}
