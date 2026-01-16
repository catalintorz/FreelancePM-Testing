using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FreelancePM.Migrations
{
    /// <inheritdoc />
    public partial class RemoveIsCompleted : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsCompleted",
                table: "WorkTasks");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsCompleted",
                table: "WorkTasks",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
