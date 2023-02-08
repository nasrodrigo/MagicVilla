using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MagicVillaAPI.Migrations
{
    /// <inheritdoc />
    public partial class UpdateUserLocalRolesField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LocalUserRole",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LocalUserId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LocalUserRole", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LocalUserRole_LocalUser_LocalUserId",
                        column: x => x.LocalUserId,
                        principalTable: "LocalUser",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_LocalUserRole_LocalUserId",
                table: "LocalUserRole",
                column: "LocalUserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LocalUserRole");
        }
    }
}
