using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using UniversitySystem.Infrastructure.Export.Dtos;

namespace UniversitySystem.Infrastructure.Export;

/// <summary>
/// Contrat du générateur de PDF (M7-03, M7-04).
/// </summary>
public interface IPdfGenerator
{
    /// <summary>Génère un PDF pour la liste des étudiants (A4 paysage).</summary>
    byte[] GenerateStudentsPdf(IReadOnlyList<EtudiantExportDto> students, string agentName, string agentEmail);

    /// <summary>Génère un PDF pour le rapport financier (A4 portrait).</summary>
    byte[] GenerateFinancesPdf(IReadOnlyList<TransactionExportDto> transactions, string agentName);
}

/// <summary>
/// Générateur de PDF via QuestPDF.
/// </summary>
public class PdfGenerator : IPdfGenerator
{
    static PdfGenerator()
    {
        // Licence Community obligatoire pour QuestPDF
        QuestPDF.Settings.License = LicenseType.Community;
    }

    // =========================================================
    // PDF ÉTUDIANTS (M7-03)
    // =========================================================
    public byte[] GenerateStudentsPdf(IReadOnlyList<EtudiantExportDto> students, string agentName, string agentEmail)
    {
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4.Landscape());
                page.Margin(1.5f, Unit.Centimetre);
                page.DefaultTextStyle(x => x.FontSize(10).FontFamily(Fonts.Arial));

                // En-tête
                page.Header().Element(container =>
                {
                    container.Row(row =>
                    {
                        row.RelativeItem().Column(column =>
                        {
                            column.Item().Text("UNIVERSITÉ").FontSize(18).SemiBold().FontColor(Colors.Blue.Darken2);
                            column.Item().Text("Liste des étudiants").FontSize(14).FontColor(Colors.Grey.Darken1);
                        });

                        row.ConstantItem(250).AlignRight().Column(column =>
                        {
                            column.Item().Text($"Date : {DateTime.UtcNow:dd/MM/yyyy HH:mm}").FontSize(9);
                            column.Item().Text($"Généré par : {agentName} ({agentEmail})").FontSize(9);
                        });
                    });
                });

                // Contenu
                page.Content().PaddingVertical(20).Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.ConstantColumn(80);  // Matricule
                        columns.RelativeColumn(2);   // Nom
                        columns.RelativeColumn(2);   // Prénom
                        columns.RelativeColumn(2);   // Faculté
                        columns.RelativeColumn(2);   // Filière
                        columns.ConstantColumn(60);  // Niveau
                        columns.ConstantColumn(70);  // Statut dossier
                        columns.ConstantColumn(70);  // Statut paiement
                    });

                    table.Header(header =>
                    {
                        header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Matricule").SemiBold();
                        header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Nom").SemiBold();
                        header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Prénom").SemiBold();
                        header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Faculté").SemiBold();
                        header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Filière").SemiBold();
                        header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Niveau").SemiBold();
                        header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Statut").SemiBold();
                        header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Paiement").SemiBold();
                    });

                    foreach (var s in students)
                    {
                        table.Cell().Padding(3).Text(s.Matricule);
                        table.Cell().Padding(3).Text(s.Nom);
                        table.Cell().Padding(3).Text(s.Prenom);
                        table.Cell().Padding(3).Text(s.Faculte);
                        table.Cell().Padding(3).Text(s.Filiere);
                        table.Cell().Padding(3).Text(s.Niveau);
                        table.Cell().Padding(3).Text(s.StatutDossier);
                        table.Cell().Padding(3).Text(s.StatutPaiement);
                    }
                });

                // Pied de page
                page.Footer().Row(row =>
                {
                    row.RelativeItem().Text("Document généré automatiquement — confidentiel").FontSize(8).FontColor(Colors.Grey.Medium);
                    row.ConstantItem(100).AlignRight().Text(text =>
                    {
                        text.Span("Page ");
                        text.CurrentPageNumber();
                        text.Span(" / ");
                        text.TotalPages();
                    });
                });
            });
        });

        return document.GeneratePdf();
    }

    // =========================================================
    // PDF FINANCES (M7-04)
    // =========================================================
    public byte[] GenerateFinancesPdf(IReadOnlyList<TransactionExportDto> transactions, string agentName)
    {
        var totalGlobal = transactions.Sum(t => t.Montant);

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(1.5f, Unit.Centimetre);
                page.DefaultTextStyle(x => x.FontSize(10).FontFamily(Fonts.Arial));

                // En-tête
                page.Header().Element(container =>
                {
                    container.Row(row =>
                    {
                        row.RelativeItem().Column(column =>
                        {
                            column.Item().Text("UNIVERSITÉ").FontSize(18).SemiBold().FontColor(Colors.Blue.Darken2);
                            column.Item().Text("Rapport financier").FontSize(14).FontColor(Colors.Grey.Darken1);
                        });

                        row.ConstantItem(250).AlignRight().Column(column =>
                        {
                            column.Item().Text($"Date : {DateTime.UtcNow:dd/MM/yyyy HH:mm}").FontSize(9);
                            column.Item().Text($"Généré par : {agentName}").FontSize(9);
                        });
                    });
                });

                // Contenu
                page.Content().PaddingVertical(20).Column(column =>
                {
                    column.Item().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(3);   // Référence
                            columns.ConstantColumn(70);  // Matricule
                            columns.RelativeColumn(3);   // Nom
                            columns.RelativeColumn(2);   // Type
                            columns.RelativeColumn(2);   // Moyen
                            columns.ConstantColumn(80);  // Montant
                            columns.ConstantColumn(80);  // Date
                        });

                        table.Header(header =>
                        {
                            header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Référence").SemiBold();
                            header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Matricule").SemiBold();
                            header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Étudiant").SemiBold();
                            header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Type").SemiBold();
                            header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Moyen").SemiBold();
                            header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Montant").SemiBold();
                            header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Date").SemiBold();
                        });

                        foreach (var t in transactions)
                        {
                            table.Cell().Padding(3).Text(t.Reference);
                            table.Cell().Padding(3).Text(t.Matricule);
                            table.Cell().Padding(3).Text(t.NomPrenom);
                            table.Cell().Padding(3).Text(t.TypePaiement);
                            table.Cell().Padding(3).Text(t.MoyenPaiement);
                            table.Cell().Padding(3).Text($"{t.Montant:N0} FCFA");
                            table.Cell().Padding(3).Text(t.DatePaiement.ToString("dd/MM/yyyy"));
                        }
                    });

                    column.Item().PaddingTop(15).AlignRight().Column(inner =>
                    {
                        inner.Item().Text($"Total global : {totalGlobal:N0} FCFA").FontSize(12).Bold();
                    });
                });

                // Pied de page
                page.Footer().Row(row =>
                {
                    row.RelativeItem().Text("Document généré automatiquement — confidentiel").FontSize(8).FontColor(Colors.Grey.Medium);
                    row.ConstantItem(100).AlignRight().Text(text =>
                    {
                        text.Span("Page ");
                        text.CurrentPageNumber();
                        text.Span(" / ");
                        text.TotalPages();
                    });
                });
            });
        });

        return document.GeneratePdf();
    }
}