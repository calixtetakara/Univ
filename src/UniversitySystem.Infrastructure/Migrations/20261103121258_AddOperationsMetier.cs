using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UniversitySystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddOperationsMetier : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "documents_etudiant",
                columns: table => new
                {
                    id_document = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    id_etudiant = table.Column<int>(type: "int", nullable: false),
                    type = table.Column<int>(type: "int", nullable: false),
                    nom_fichier = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    chemin_fichier = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    taille_octets = table.Column<long>(type: "bigint", nullable: false),
                    content_type = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    date_upload = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_documents_etudiant", x => x.id_document);
                    table.ForeignKey(
                        name: "FK_documents_etudiant_etudiants_id_etudiant",
                        column: x => x.id_etudiant,
                        principalTable: "etudiants",
                        principalColumn: "id_etudiant",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "inscriptions",
                columns: table => new
                {
                    id_inscription = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    id_etudiant = table.Column<int>(type: "int", nullable: false),
                    id_filiere = table.Column<int>(type: "int", nullable: false),
                    annee_academique = table.Column<string>(type: "varchar(9)", maxLength: 9, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    niveau = table.Column<int>(type: "int", nullable: false),
                    statut = table.Column<int>(type: "int", nullable: false),
                    date_soumission = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    date_traitement = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    commentaire = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    date_creation = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_inscriptions", x => x.id_inscription);
                    table.ForeignKey(
                        name: "FK_inscriptions_etudiants_id_etudiant",
                        column: x => x.id_etudiant,
                        principalTable: "etudiants",
                        principalColumn: "id_etudiant",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_inscriptions_filieres_id_filiere",
                        column: x => x.id_filiere,
                        principalTable: "filieres",
                        principalColumn: "id_filiere",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "notes_bac",
                columns: table => new
                {
                    id_notes_bac = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    id_etudiant = table.Column<int>(type: "int", nullable: false),
                    matiere = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    note = table.Column<decimal>(type: "decimal(4,2)", precision: 4, scale: 2, nullable: false),
                    coefficient = table.Column<decimal>(type: "decimal(4,2)", precision: 4, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_notes_bac", x => x.id_notes_bac);
                    table.ForeignKey(
                        name: "FK_notes_bac_etudiants_id_etudiant",
                        column: x => x.id_etudiant,
                        principalTable: "etudiants",
                        principalColumn: "id_etudiant",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "salles",
                columns: table => new
                {
                    id_salle = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    code = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    nom = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    capacite = table.Column<int>(type: "int", nullable: false),
                    type = table.Column<int>(type: "int", nullable: false),
                    batiment = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    actif = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_salles", x => x.id_salle);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "paiements",
                columns: table => new
                {
                    id_paiement = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    id_inscription = table.Column<int>(type: "int", nullable: false),
                    montant = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: false),
                    reference_transaction = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    mode_paiement = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    statut = table.Column<int>(type: "int", nullable: false),
                    date_paiement = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    date_traitement = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_paiements", x => x.id_paiement);
                    table.ForeignKey(
                        name: "FK_paiements_inscriptions_id_inscription",
                        column: x => x.id_inscription,
                        principalTable: "inscriptions",
                        principalColumn: "id_inscription",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "programmations",
                columns: table => new
                {
                    id_programmation = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    id_ue = table.Column<int>(type: "int", nullable: false),
                    id_enseignant = table.Column<int>(type: "int", nullable: false),
                    id_salle = table.Column<int>(type: "int", nullable: false),
                    id_filiere = table.Column<int>(type: "int", nullable: false),
                    jour_semaine = table.Column<int>(type: "int", nullable: false),
                    heure_debut = table.Column<TimeSpan>(type: "time(6)", nullable: false),
                    heure_fin = table.Column<TimeSpan>(type: "time(6)", nullable: false),
                    annee_academique = table.Column<string>(type: "varchar(9)", maxLength: 9, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    semestre = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_programmations", x => x.id_programmation);
                    table.ForeignKey(
                        name: "FK_programmations_enseignants_id_enseignant",
                        column: x => x.id_enseignant,
                        principalTable: "enseignants",
                        principalColumn: "id_enseignant",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_programmations_filieres_id_filiere",
                        column: x => x.id_filiere,
                        principalTable: "filieres",
                        principalColumn: "id_filiere",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_programmations_salles_id_salle",
                        column: x => x.id_salle,
                        principalTable: "salles",
                        principalColumn: "id_salle",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_programmations_ues_id_ue",
                        column: x => x.id_ue,
                        principalTable: "ues",
                        principalColumn: "id_ue",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_documents_etudiant_id_etudiant",
                table: "documents_etudiant",
                column: "id_etudiant");

            migrationBuilder.CreateIndex(
                name: "IX_inscriptions_id_etudiant_annee_academique",
                table: "inscriptions",
                columns: new[] { "id_etudiant", "annee_academique" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_inscriptions_id_filiere",
                table: "inscriptions",
                column: "id_filiere");

            migrationBuilder.CreateIndex(
                name: "IX_notes_bac_id_etudiant",
                table: "notes_bac",
                column: "id_etudiant");

            migrationBuilder.CreateIndex(
                name: "IX_paiements_id_inscription",
                table: "paiements",
                column: "id_inscription");

            migrationBuilder.CreateIndex(
                name: "IX_programmations_id_enseignant",
                table: "programmations",
                column: "id_enseignant");

            migrationBuilder.CreateIndex(
                name: "IX_programmations_id_filiere",
                table: "programmations",
                column: "id_filiere");

            migrationBuilder.CreateIndex(
                name: "IX_programmations_id_salle",
                table: "programmations",
                column: "id_salle");

            migrationBuilder.CreateIndex(
                name: "IX_programmations_id_ue",
                table: "programmations",
                column: "id_ue");

            migrationBuilder.CreateIndex(
                name: "IX_salles_code",
                table: "salles",
                column: "code",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "documents_etudiant");

            migrationBuilder.DropTable(
                name: "notes_bac");

            migrationBuilder.DropTable(
                name: "paiements");

            migrationBuilder.DropTable(
                name: "programmations");

            migrationBuilder.DropTable(
                name: "inscriptions");

            migrationBuilder.DropTable(
                name: "salles");
        }
    }
}
