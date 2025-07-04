using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ElsWebApp.Migrations
{
    /// <inheritdoc />
    public partial class update : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "FixedOrder",
                table: "QuestionCatalog",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<byte>(
                name: "OrderNo",
                table: "AnswerGroup",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FixedOrder",
                table: "QuestionCatalog");

            migrationBuilder.DropColumn(
                name: "OrderNo",
                table: "AnswerGroup");
        }
    }
}
