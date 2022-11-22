using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Api.Migrations
{
    /// <inheritdoc />
    public partial class follow1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Relation_Users_FollowerId",
                table: "Relation");

            migrationBuilder.DropForeignKey(
                name: "FK_Relation_Users_FollowingId",
                table: "Relation");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Relation",
                table: "Relation");

            migrationBuilder.RenameTable(
                name: "Relation",
                newName: "Relations");

            migrationBuilder.RenameIndex(
                name: "IX_Relation_FollowingId",
                table: "Relations",
                newName: "IX_Relations_FollowingId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Relations",
                table: "Relations",
                columns: new[] { "FollowerId", "FollowingId" });

            migrationBuilder.AddForeignKey(
                name: "FK_Relations_Users_FollowerId",
                table: "Relations",
                column: "FollowerId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Relations_Users_FollowingId",
                table: "Relations",
                column: "FollowingId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Relations_Users_FollowerId",
                table: "Relations");

            migrationBuilder.DropForeignKey(
                name: "FK_Relations_Users_FollowingId",
                table: "Relations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Relations",
                table: "Relations");

            migrationBuilder.RenameTable(
                name: "Relations",
                newName: "Relation");

            migrationBuilder.RenameIndex(
                name: "IX_Relations_FollowingId",
                table: "Relation",
                newName: "IX_Relation_FollowingId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Relation",
                table: "Relation",
                columns: new[] { "FollowerId", "FollowingId" });

            migrationBuilder.AddForeignKey(
                name: "FK_Relation_Users_FollowerId",
                table: "Relation",
                column: "FollowerId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Relation_Users_FollowingId",
                table: "Relation",
                column: "FollowingId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
