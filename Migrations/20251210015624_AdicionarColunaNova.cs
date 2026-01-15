using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Jogos_Backlogger.Migrations
{
    /// <inheritdoc />
    public partial class AdicionarColunaNova : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "VezesFinalizado",
                table: "ItemBacklog",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VezesFinalizado",
                table: "ItemBacklog");
        }
    }
}
