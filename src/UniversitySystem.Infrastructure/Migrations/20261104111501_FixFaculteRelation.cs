using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UniversitySystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixFaculteRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "actif",
                table: "filieres");

            migrationBuilder.DropColumn(
                name: "description",
                table: "filieres");

            migrationBuilder.RenameColumn(
                name: "nom",
                table: "filieres",
                newName: "nom_filiere");

            migrationBuilder.RenameColumn(
                name: "code",
                table: "filieres",
                newName: "code_filiere");

            migrationBuilder.RenameIndex(
                name: "IX_filieres_code",
                table: "filieres",
                newName: "IX_filieres_code_filiere");

            migrationBuilder.RenameColumn(
                name: "nom",
                table: "facultes",
                newName: "nom_faculte");

            migrationBuilder.RenameColumn(
                name: "code",
                table: "facultes",
                newName: "code_faculte");

            migrationBuilder.RenameIndex(
                name: "IX_facultes_code",
                table: "facultes",
                newName: "IX_facultes_code_faculte");

            migrationBuilder.AddColumn<int>(
                name: "UeIdUe",
                table: "filiere_ues",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "type_faculte",
                table: "facultes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_filiere_ues_UeIdUe",
                table: "filiere_ues",
                column: "UeIdUe");

            migrationBuilder.AddForeignKey(
                name: "FK_filiere_ues_ues_UeIdUe",
                table: "filiere_ues",
                column: "UeIdUe",
                principalTable: "ues",
                principalColumn: "id_ue");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_filiere_ues_ues_UeIdUe",
                table: "filiere_ues");

            migrationBuilder.DropIndex(
                name: "IX_filiere_ues_UeIdUe",
                table: "filiere_ues");

            migrationBuilder.DropColumn(
                name: "UeIdUe",
                table: "filiere_ues");

            migrationBuilder.DropColumn(
                name: "type_faculte",
                table: "facultes");

            migrationBuilder.RenameColumn(
                name: "nom_filiere",
                table: "filieres",
                newName: "nom");

            migrationBuilder.RenameColumn(
                name: "code_filiere",
                table: "filieres",
                newName: "code");

            migrationBuilder.RenameIndex(
                name: "IX_filieres_code_filiere",
                table: "filieres",
                newName: "IX_filieres_code");

            migrationBuilder.RenameColumn(
                name: "nom_faculte",
                table: "facultes",
                newName: "nom");

            migrationBuilder.RenameColumn(
                name: "code_faculte",
                table: "facultes",
                newName: "code");

            migrationBuilder.RenameIndex(
                name: "IX_facultes_code_faculte",
                table: "facultes",
                newName: "IX_facultes_code");

            migrationBuilder.AddColumn<bool>(
                name: "actif",
                table: "filieres",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<string>(
                name: "description",
                table: "filieres",
                type: "varchar(500)",
                maxLength: 500,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }
    }
}
