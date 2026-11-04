using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using UniversitySystem.Domain.Entities;

namespace UniversitySystem.Infrastructure.Receipts;

public interface IReceiptGenerator
{
    byte[] Generate(Paiement paiement, Etudiant etudiant, Inscription inscription);
}

public class QuestPdfReceiptGenerator : IReceiptGenerator
{
    public byte[] Generate(Paiement paiement, Etudiant etudiant, Inscription inscription)
    {
        // Configuration de la licence QuestPDF
        QuestPDF.Settings.License = LicenseType.Community;

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(12).FontFamily(Fonts.Arial));

                // En-tête
                page.Header().Element(ComposeHeader);

                // Contenu
                page.Content().Element(x => ComposeContent(x, paiement, etudiant, inscription));

                // Pied de page
                page.Footer().AlignCenter().Text(x =>
                {
                    x.CurrentPageNumber();
                    x.Span(" / ");
                    x.TotalPages();
                });
            });
        });

        return document.GeneratePdf();
    }

    private void ComposeHeader(IContainer container)
    {
        container.Row(row =>
        {
            row.RelativeItem().Column(column =>
            {
                column.Item().Text("UNIVERSITÉ").FontSize(20).SemiBold().FontColor(Colors.Blue.Darken2);
                column.Item().Text("Reçu de paiement").FontSize(14).FontColor(Colors.Grey.Darken1);
            });

            row.ConstantItem(100).AlignRight().Text($"Date : {DateTime.UtcNow:dd/MM/yyyy}").FontSize(10);
        });
    }

    private void ComposeContent(IContainer container, Paiement paiement, Etudiant etudiant, Inscription inscription)
    {
        container.PaddingVertical(1, Unit.Centimetre).Column(column =>
        {
            column.Spacing(10);

            // Bloc Étudiant
            column.Item().Background(Colors.Grey.Lighten3).Padding(10).Column(col =>
            {
                col.Item().Text("Informations de l'étudiant").SemiBold().FontSize(14);
                col.Item().Text($"Nom : {etudiant.Utilisateur.Nom} {etudiant.Utilisateur.Prenom}");
                col.Item().Text($"Matricule : {etudiant.Matricule}");
                col.Item().Text($"Filière : {inscription.Filiere?.NomFiliere}");
            });

            // Bloc Paiement
            column.Item().Background(Colors.Green.Lighten4).Padding(10).Column(col =>
            {
                col.Item().Text("Détails du paiement").SemiBold().FontSize(14);
                col.Item().Text($"Référence : {paiement.ReferenceTransaction}");
                col.Item().Text($"Montant : {paiement.Montant:N0} FCFA").FontSize(14).Bold();
                col.Item().Text($"Moyen : {paiement.ModePaiement}");
                col.Item().Text($"Date : {paiement.DateTraitement:dd/MM/yyyy HH:mm}");
            });

            // Filigrane
            column.Item().AlignCenter().PaddingTop(50).Text("PAYÉ")
                .FontSize(80).Bold().FontColor(Colors.Green.Lighten3);
        });
    }
}