using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DOOR.Migrations
{
    /// <inheritdoc />
    public partial class aazxczxcd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DoorId",
                table: "Services",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CategoryId",
                table: "Doors",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Services_DoorId",
                table: "Services",
                column: "DoorId");

            migrationBuilder.CreateIndex(
                name: "IX_Doors_CategoryId",
                table: "Doors",
                column: "CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Doors_Categories_CategoryId",
                table: "Doors",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Services_Doors_DoorId",
                table: "Services",
                column: "DoorId",
                principalTable: "Doors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Doors_Categories_CategoryId",
                table: "Doors");

            migrationBuilder.DropForeignKey(
                name: "FK_Services_Doors_DoorId",
                table: "Services");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropIndex(
                name: "IX_Services_DoorId",
                table: "Services");

            migrationBuilder.DropIndex(
                name: "IX_Doors_CategoryId",
                table: "Doors");

            migrationBuilder.DropColumn(
                name: "DoorId",
                table: "Services");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "Doors");
        }
    }
}
