using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace p3_movie_platform.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueConstraintToMovieTitle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameIndex(
                name: "IX_Movies_Title",
                table: "Movies",
                newName: "IX_Movies_Title_Unique");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameIndex(
                name: "IX_Movies_Title_Unique",
                table: "Movies",
                newName: "IX_Movies_Title");
        }
    }
}
