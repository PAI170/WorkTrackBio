using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WorkTrackBio.API.Migrations
{
    /// <inheritdoc />
    public partial class FixVacationRequestApprovedByFK : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VacationRequests_EmployeeInfos_ApprovedById",
                table: "VacationRequests");

            migrationBuilder.AddForeignKey(
                name: "FK_VacationRequests_AppUsers_ApprovedById",
                table: "VacationRequests",
                column: "ApprovedById",
                principalTable: "AppUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VacationRequests_AppUsers_ApprovedById",
                table: "VacationRequests");

            migrationBuilder.AddForeignKey(
                name: "FK_VacationRequests_EmployeeInfos_ApprovedById",
                table: "VacationRequests",
                column: "ApprovedById",
                principalTable: "EmployeeInfos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
