using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ratbags.Accounts.API.Migrations
{
    /// <inheritdoc />
    public partial class refreshTokens_slightMod : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RefreshToken",
                table: "RefreshTokens",
                newName: "Token");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Token",
                table: "RefreshTokens",
                newName: "RefreshToken");
        }
    }
}
