using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UniversitySystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddStructureAcademique : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "enseignants",
                columns: table => new
                {
                    id_enseignant = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    id_utilisateur = table.Column<int>(type: "int", nullable: false),
                    matricule = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    grade = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    specialite = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    statut = table.Column<int>(type: "int", nullable: false),
                    date_creation = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_enseignants", x => x.id_enseignant);
                    table.ForeignKey(
                        name: "FK_enseignants_utilisateurs_id_utilisateur",
                        column: x => x.id_utilisateur,
                        principalTable: "utilisateurs",
                        principalColumn: "id_utilisateur",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "facultes",
                columns: table => new
                {
                    id_faculte = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    code = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    nom = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    description = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    actif = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_facultes", x => x.id_faculte);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ues",
                columns: table => new
                {
                    id_ue = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    code = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    intitule = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    credits = table.Column<int>(type: "int", nullable: false),
                    volume_horaire = table.Column<int>(type: "int", nullable: false),
                    description = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    actif = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ues", x => x.id_ue);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "filieres",
                columns: table => new
                {
                    id_filiere = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    code = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    nom = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    description = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    id_faculte = table.Column<int>(type: "int", nullable: false),
                    actif = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_filieres", x => x.id_filiere);
                    table.ForeignKey(
                        name: "FK_filieres_facultes_id_faculte",
                        column: x => x.id_faculte,
                        principalTable: "facultes",
                        principalColumn: "id_faculte",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "etudiants",
                columns: table => new
                {
                    id_etudiant = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    id_utilisateur = table.Column<int>(type: "int", nullable: false),
                    matricule = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    date_naissance = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    lieu_naissance = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    genre = table.Column<int>(type: "int", nullable: false),
                    adresse = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    nationalite = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    id_filiere = table.Column<int>(type: "int", nullable: true),
                    date_creation = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_etudiants", x => x.id_etudiant);
                    table.ForeignKey(
                        name: "FK_etudiants_filieres_id_filiere",
                        column: x => x.id_filiere,
                        principalTable: "filieres",
                        principalColumn: "id_filiere",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_etudiants_utilisateurs_id_utilisateur",
                        column: x => x.id_utilisateur,
                        principalTable: "utilisateurs",
                        principalColumn: "id_utilisateur",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "filiere_ues",
                columns: table => new
                {
                    id_filiere_ue = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    id_filiere = table.Column<int>(type: "int", nullable: false),
                    id_ue = table.Column<int>(type: "int", nullable: false),
                    semestre = table.Column<int>(type: "int", nullable: false),
                    obligatoire = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_filiere_ues", x => x.id_filiere_ue);
                    table.ForeignKey(
                        name: "FK_filiere_ues_filieres_id_filiere",
                        column: x => x.id_filiere,
                        principalTable: "filieres",
                        principalColumn: "id_filiere",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_filiere_ues_ues_id_ue",
                        column: x => x.id_ue,
                        principalTable: "ues",
                        principalColumn: "id_ue",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_enseignants_id_utilisateur",
                table: "enseignants",
                column: "id_utilisateur",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_enseignants_matricule",
                table: "enseignants",
                column: "matricule",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_etudiants_id_filiere",
                table: "etudiants",
                column: "id_filiere");

            migrationBuilder.CreateIndex(
                name: "IX_etudiants_id_utilisateur",
                table: "etudiants",
                column: "id_utilisateur",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_etudiants_matricule",
                table: "etudiants",
                column: "matricule",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_facultes_code",
                table: "facultes",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_filiere_ues_id_filiere_id_ue_semestre",
                table: "filiere_ues",
                columns: new[] { "id_filiere", "id_ue", "semestre" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_filiere_ues_id_ue",
                table: "filiere_ues",
                column: "id_ue");

            migrationBuilder.CreateIndex(
                name: "IX_filieres_code",
                table: "filieres",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_filieres_id_faculte",
                table: "filieres",
                column: "id_faculte");

            migrationBuilder.CreateIndex(
                name: "IX_ues_code",
                table: "ues",
                column: "code",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "enseignants");

            migrationBuilder.DropTable(
                name: "etudiants");

            migrationBuilder.DropTable(
                name: "filiere_ues");

            migrationBuilder.DropTable(
                name: "filieres");

            migrationBuilder.DropTable(
                name: "ues");

            migrationBuilder.DropTable(
                name: "facultes");
        }
    }
}
