namespace UniversitySystem.Infrastructure.Export.Dtos;

public class EtudiantExportDto
{
    public string Matricule { get; set; } = string.Empty;
    public string Nom { get; set; } = string.Empty;
    public string Prenom { get; set; } = string.Empty;
    public string Faculte { get; set; } = string.Empty;
    public string Filiere { get; set; } = string.Empty;
    public string Niveau { get; set; } = string.Empty;
    public string StatutDossier { get; set; } = string.Empty;
    public string StatutPaiement { get; set; } = string.Empty;
}

public class TransactionExportDto
{
    public string Reference { get; set; } = string.Empty;
    public string Matricule { get; set; } = string.Empty;
    public string NomPrenom { get; set; } = string.Empty;
    public string Faculte { get; set; } = string.Empty;
    public string TypePaiement { get; set; } = string.Empty;
    public string MoyenPaiement { get; set; } = string.Empty;
    public decimal Montant { get; set; }
    public DateTime DatePaiement { get; set; }
}