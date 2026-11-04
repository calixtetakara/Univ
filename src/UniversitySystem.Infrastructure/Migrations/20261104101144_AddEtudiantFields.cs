using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UniversitySystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddEtudiantFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MotifRejet",
                table: "etudiants",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "NumTableBac",
                table: "etudiants",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "SessionBac",
                table: "etudiants",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "StatutDossier",
                table: "etudiants",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MotifRejet",
                table: "etudiants");

            migrationBuilder.DropColumn(
                name: "NumTableBac",
                table: "etudiants");

            migrationBuilder.DropColumn(
                name: "SessionBac",
                table: "etudiants");

            migrationBuilder.DropColumn(
                name: "StatutDossier",
                table: "etudiants");
        }
    }
}
