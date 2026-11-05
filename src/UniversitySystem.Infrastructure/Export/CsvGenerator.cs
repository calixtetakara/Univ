using System.Globalization;
using System.Text;
using CsvHelper;
using UniversitySystem.Infrastructure.Export.Dtos;

namespace UniversitySystem.Infrastructure.Export;

/// <summary>
/// Contrat du générateur de fichiers CSV (M7-01).
/// </summary>
public interface ICsvGenerator
{
    /// <summary>Génère un CSV pour la liste des étudiants.</summary>
    byte[] GenerateStudentsCsv(IReadOnlyList<EtudiantExportDto> students);

    /// <summary>Génère un CSV pour les transactions financières.</summary>
    byte[] GenerateFinancesCsv(IReadOnlyList<TransactionExportDto> transactions);
}

/// <summary>
/// Générateur de fichiers CSV.
/// Utilise un encodage UTF-8 BOM pour Excel FR (accents corrects).
/// </summary>
public class CsvGenerator : ICsvGenerator
{
    /// <inheritdoc />
    public byte[] GenerateStudentsCsv(IReadOnlyList<EtudiantExportDto> students)
    {
        using var memoryStream = new MemoryStream();

        // UTF-8 avec BOM (obligatoire pour Excel FR)
        using var writer = new StreamWriter(memoryStream, new UTF8Encoding(true));
        using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);

        // En-têtes
        csv.WriteField("Matricule");
        csv.WriteField("Nom");
        csv.WriteField("Prenom");
        csv.WriteField("Faculte");
        csv.WriteField("Filiere");
        csv.WriteField("Niveau");
        csv.WriteField("StatutDossier");
        csv.WriteField("StatutPaiement");
        csv.NextRecord();

        // Lignes
        foreach (var s in students)
        {
            csv.WriteField(s.Matricule);
            csv.WriteField(s.Nom);
            csv.WriteField(s.Prenom);
            csv.WriteField(s.Faculte);
            csv.WriteField(s.Filiere);
            csv.WriteField(s.Niveau);
            csv.WriteField(s.StatutDossier);
            csv.WriteField(s.StatutPaiement);
            csv.NextRecord();
        }

        writer.Flush();
        return memoryStream.ToArray();
    }

    /// <inheritdoc />
    public byte[] GenerateFinancesCsv(IReadOnlyList<TransactionExportDto> transactions)
    {
        using var memoryStream = new MemoryStream();
        using var writer = new StreamWriter(memoryStream, new UTF8Encoding(true));
        using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);

        // En-têtes
        csv.WriteField("Reference");
        csv.WriteField("Matricule");
        csv.WriteField("NomPrenom");
        csv.WriteField("Faculte");
        csv.WriteField("TypePaiement");
        csv.WriteField("MoyenPaiement");
        csv.WriteField("Montant");
        csv.WriteField("DatePaiement");
        csv.NextRecord();

        // Lignes
        foreach (var t in transactions)
        {
            csv.WriteField(t.Reference);
            csv.WriteField(t.Matricule);
            csv.WriteField(t.NomPrenom);
            csv.WriteField(t.Faculte);
            csv.WriteField(t.TypePaiement);
            csv.WriteField(t.MoyenPaiement);
            csv.WriteField(t.Montant);
            csv.WriteField(t.DatePaiement.ToString("dd/MM/yyyy HH:mm"));
            csv.NextRecord();
        }

        writer.Flush();
        return memoryStream.ToArray();
    }
}