using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace System_Analysis.Migrations
{
    public partial class AddNavigationProps : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Groups_Users_AdminId",
                table: "Groups");

            migrationBuilder.DropForeignKey(
                name: "FK_PrivateMessageUser_PrivateMessages_PrivateMessagesId",
                table: "PrivateMessageUser");

            migrationBuilder.DropForeignKey(
                name: "FK_PrivateMessageUser_Users_UsersId",
                table: "PrivateMessageUser");

            migrationBuilder.DropIndex(
                name: "IX_Groups_AdminId",
                table: "Groups");

            migrationBuilder.RenameColumn(
                name: "UsersId",
                table: "PrivateMessageUser",
                newName: "PrivateMessageId");

            migrationBuilder.RenameColumn(
                name: "PrivateMessagesId",
                table: "PrivateMessageUser",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_PrivateMessageUser_UsersId",
                table: "PrivateMessageUser",
                newName: "IX_PrivateMessageUser_PrivateMessageId");

            migrationBuilder.AlterColumn<Guid>(
                name: "AdminId",
                table: "Groups",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "Groups",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Groups_UserId",
                table: "Groups",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Groups_Users_UserId",
                table: "Groups",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PrivateMessageUser_PrivateMessages_PrivateMessageId",
                table: "PrivateMessageUser",
                column: "PrivateMessageId",
                principalTable: "PrivateMessages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PrivateMessageUser_Users_UserId",
                table: "PrivateMessageUser",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Groups_Users_UserId",
                table: "Groups");

            migrationBuilder.DropForeignKey(
                name: "FK_PrivateMessageUser_PrivateMessages_PrivateMessageId",
                table: "PrivateMessageUser");

            migrationBuilder.DropForeignKey(
                name: "FK_PrivateMessageUser_Users_UserId",
                table: "PrivateMessageUser");

            migrationBuilder.DropIndex(
                name: "IX_Groups_UserId",
                table: "Groups");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Groups");

            migrationBuilder.RenameColumn(
                name: "PrivateMessageId",
                table: "PrivateMessageUser",
                newName: "UsersId");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "PrivateMessageUser",
                newName: "PrivateMessagesId");

            migrationBuilder.RenameIndex(
                name: "IX_PrivateMessageUser_PrivateMessageId",
                table: "PrivateMessageUser",
                newName: "IX_PrivateMessageUser_UsersId");

            migrationBuilder.AlterColumn<Guid>(
                name: "AdminId",
                table: "Groups",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.CreateIndex(
                name: "IX_Groups_AdminId",
                table: "Groups",
                column: "AdminId");

            migrationBuilder.AddForeignKey(
                name: "FK_Groups_Users_AdminId",
                table: "Groups",
                column: "AdminId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PrivateMessageUser_PrivateMessages_PrivateMessagesId",
                table: "PrivateMessageUser",
                column: "PrivateMessagesId",
                principalTable: "PrivateMessages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PrivateMessageUser_Users_UsersId",
                table: "PrivateMessageUser",
                column: "UsersId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
