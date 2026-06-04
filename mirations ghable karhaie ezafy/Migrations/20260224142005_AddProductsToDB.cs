using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BulkyWeb.Migrations
{
    /// <inheritdoc />
    public partial class AddProductsToDB : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ISBN = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Author = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ListPrice = table.Column<double>(type: "float", nullable: false),
                    Price = table.Column<double>(type: "float", nullable: false),
                    Price50 = table.Column<double>(type: "float", nullable: false),
                    Price100 = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "Author", "Description", "ISBN", "ListPrice", "Price", "Price100", "Price50", "Title" },
                values: new object[,]
                {
                    { 1, "Ron Parker", "Horror", "FOT000000001", 45.0, 40.0, 35.0, 38.0, "Shadows of Fear" },
                    { 2, "Emily Stone", "Psychological Thriller", "FOT000000002", 30.0, 28.0, 24.0, 26.0, "The Silent Mind" },
                    { 3, "Michael Reeves", "Science Fiction", "FOT000000003", 50.0, 47.0, 42.0, 45.0, "Code of Destiny" },
                    { 4, "Sophia Miller", "Mystery", "FOT000000004", 35.0, 32.0, 27.0, 30.0, "Whispers in the Dark" },
                    { 5, "Liam Anderson", "Fantasy", "FOT000000005", 48.0, 44.0, 39.0, 42.0, "Broken Kingdom" },
                    { 6, "Olivia Brown", "Adventure", "FOT000000006", 28.0, 25.0, 20.0, 23.0, "Beyond the Horizon" },
                    { 7, "Daniel Cooper", "Drama", "FOT000000007", 40.0, 36.0, 31.0, 34.0, "Fragments of Truth" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Products");
        }
    }
}
