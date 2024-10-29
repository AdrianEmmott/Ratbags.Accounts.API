using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ratbags.Accounts.API.Migrations
{
    /// <inheritdoc />
    public partial class refreshToken_addedRevoked : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Revoked",
                table: "RefreshTokens",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Revoked",
                table: "RefreshTokens");
        }
    }
}
