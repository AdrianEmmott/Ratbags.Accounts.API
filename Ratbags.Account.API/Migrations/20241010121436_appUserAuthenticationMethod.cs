using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ratbags.Account.API.Migrations
{
    /// <inheritdoc />
    public partial class appUserAuthenticationMethod : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AuthenticationMethod",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AuthenticationMethod",
                table: "AspNetUsers");
        }
    }
}
