using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class AddUserInfoToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RapidRating",
                table: "Users",
                newName: "Wins_Rapid");

            migrationBuilder.RenameColumn(
                name: "BulletRating",
                table: "Users",
                newName: "Wins_Bullet");

            migrationBuilder.RenameColumn(
                name: "BlitzRating",
                table: "Users",
                newName: "Wins_Blitz");

            migrationBuilder.AddColumn<int>(
                name: "Draws_Blitz",
                table: "Users",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Draws_Bullet",
                table: "Users",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Draws_Rapid",
                table: "Users",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GamesPlayed",
                table: "Users",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Losses_Blitz",
                table: "Users",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Losses_Bullet",
                table: "Users",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Losses_Rapid",
                table: "Users",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Rating_Blitz",
                table: "Users",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Rating_Bullet",
                table: "Users",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Rating_Rapid",
                table: "Users",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Draws_Blitz",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Draws_Bullet",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Draws_Rapid",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "GamesPlayed",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Losses_Blitz",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Losses_Bullet",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Losses_Rapid",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Rating_Blitz",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Rating_Bullet",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Rating_Rapid",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "Wins_Rapid",
                table: "Users",
                newName: "RapidRating");

            migrationBuilder.RenameColumn(
                name: "Wins_Bullet",
                table: "Users",
                newName: "BulletRating");

            migrationBuilder.RenameColumn(
                name: "Wins_Blitz",
                table: "Users",
                newName: "BlitzRating");
        }
    }
}
