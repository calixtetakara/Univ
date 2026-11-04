using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UniversitySystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AllowNullMatricule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "matricule",
                table: "etudiants",
                type: "varchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(30)",
                oldMaxLength: 30)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "etudiants",
                keyColumn: "matricule",
                keyValue: null,
                column: "matricule",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "matricule",
                table: "etudiants",
                type: "varchar(30)",
                maxLength: 30,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldMaxLength: 50,
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");
        }
    }
}
