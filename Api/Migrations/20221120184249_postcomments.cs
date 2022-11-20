using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Api.Migrations
{
    /// <inheritdoc />
    public partial class postcomments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PostComments_Users_UserId",
                table: "PostComments");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "PostComments",
                newName: "AuthorId");

            migrationBuilder.RenameIndex(
                name: "IX_PostComments_UserId",
                table: "PostComments",
                newName: "IX_PostComments_AuthorId");

            migrationBuilder.AlterColumn<string>(
                name: "Caption",
                table: "Posts",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Caption",
                table: "PostComments",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreatedDate",
                table: "PostComments",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.CreateTable(
                name: "PostCommentAttachments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PostCommentId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PostCommentAttachments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PostCommentAttachments_Attachments_Id",
                        column: x => x.Id,
                        principalTable: "Attachments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PostCommentAttachments_PostComments_PostCommentId",
                        column: x => x.PostCommentId,
                        principalTable: "PostComments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PostCommentAttachments_PostCommentId",
                table: "PostCommentAttachments",
                column: "PostCommentId");

            migrationBuilder.AddForeignKey(
                name: "FK_PostComments_Users_AuthorId",
                table: "PostComments",
                column: "AuthorId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PostComments_Users_AuthorId",
                table: "PostComments");

            migrationBuilder.DropTable(
                name: "PostCommentAttachments");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "PostComments");

            migrationBuilder.RenameColumn(
                name: "AuthorId",
                table: "PostComments",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_PostComments_AuthorId",
                table: "PostComments",
                newName: "IX_PostComments_UserId");

            migrationBuilder.AlterColumn<string>(
                name: "Caption",
                table: "Posts",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Caption",
                table: "PostComments",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_PostComments_Users_UserId",
                table: "PostComments",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
