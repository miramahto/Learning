using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddIsAttendedToEventGuestMapping : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GuestLists_AppMasters_AppID",
                table: "GuestLists");

            migrationBuilder.AddColumn<bool>(
                name: "IsAttended",
                table: "EventGuestMapping",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddForeignKey(
                name: "FK_GuestLists_CityTownVillages_AppID",
                table: "GuestLists",
                column: "AppID",
                principalTable: "CityTownVillages",
                principalColumn: "CityTownVillageID",
                onDelete: ReferentialAction.NoAction);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GuestLists_CityTownVillages_AppID",
                table: "GuestLists");

            migrationBuilder.DropColumn(
                name: "IsAttended",
                table: "EventGuestMapping");

            migrationBuilder.AddForeignKey(
                name: "FK_GuestLists_AppMasters_AppID",
                table: "GuestLists",
                column: "AppID",
                principalTable: "AppMasters",
                principalColumn: "AppID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
