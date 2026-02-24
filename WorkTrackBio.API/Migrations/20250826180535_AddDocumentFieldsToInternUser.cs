using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WorkTrackBio.API.Migrations
{
    public partial class AddDocumentFieldsToInternUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateOnly>(
                name: "DocumentExpire",
                table: "InternUsers",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DocumentNumber",
                table: "InternUsers",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DocumentTypeId",
                table: "InternUsers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_InternUsers_DocumentTypeId",
                table: "InternUsers",
                column: "DocumentTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_InternUsers_DocumentType_DocumentTypeId",
                table: "InternUsers",
                column: "DocumentTypeId",
                principalTable: "DocumentType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InternUsers_DocumentType_DocumentTypeId",
                table: "InternUsers");

            migrationBuilder.DropIndex(
                name: "IX_InternUsers_DocumentTypeId",
                table: "InternUsers");

            migrationBuilder.DropColumn(
                name: "DocumentExpire",
                table: "InternUsers");

            migrationBuilder.DropColumn(
                name: "DocumentNumber",
                table: "InternUsers");

            migrationBuilder.DropColumn(
                name: "DocumentTypeId",
                table: "InternUsers");
        }
    }
}
