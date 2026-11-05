using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UniversitySystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUtilisateurPermissions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "utilisateur_permissions",
                columns: table => new
                {
                    IdUtilisateur = table.Column<int>(type: "int", nullable: false),
                    IdPermission = table.Column<int>(type: "int", nullable: false),
                    accorde = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    modifie_par = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_utilisateur_permissions", x => new { x.IdUtilisateur, x.IdPermission });
                    table.ForeignKey(
                        name: "FK_utilisateur_permissions_permissions_IdPermission",
                        column: x => x.IdPermission,
                        principalTable: "permissions",
                        principalColumn: "id_permission",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_utilisateur_permissions_utilisateurs_IdUtilisateur",
                        column: x => x.IdUtilisateur,
                        principalTable: "utilisateurs",
                        principalColumn: "id_utilisateur",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_utilisateur_permissions_IdPermission",
                table: "utilisateur_permissions",
                column: "IdPermission");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "utilisateur_permissions");
        }
    }
}
