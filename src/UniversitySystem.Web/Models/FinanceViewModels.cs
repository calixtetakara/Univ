using System;
using System.Collections.Generic;

namespace UniversitySystem.Web.Models;

public class FinanceIndexViewModel
{
    public IReadOnlyList<TransactionDto> Transactions { get; set; } = new List<TransactionDto>();
    
    // Totaux
    public decimal TotalGlobal { get; set; }
    public Dictionary<string, decimal> TotalParMoyen { get; set; } = new();
    public Dictionary<string, decimal> TotalParType { get; set; } = new();

    // Filtres
    public string? MoyenPaiement { get; set; }
    public string? TypePaiement { get; set; }
    public DateTime? DateDebut { get; set; }
    public DateTime? DateFin { get; set; }
    public string? CodeFaculte { get; set; }
    public string? CodeFiliere { get; set; }

    // Pagination
    public int Page { get; set; } = 1;
    public int TotalPages { get; set; }
    public int TotalCount { get; set; }
}

public class TransactionDto
{
    public int IdPaiement { get; set; }
    public string Reference { get; set; } = string.Empty;
    public string Matricule { get; set; } = string.Empty;
    public string NomComplet { get; set; } = string.Empty;
    public string Faculte { get; set; } = string.Empty;
    public string Filiere { get; set; } = string.Empty;
    public string TypePaiement { get; set; } = "Scolarité"; // Valeur par défaut
    public string MoyenPaiement { get; set; } = string.Empty;
    public decimal Montant { get; set; }
    public DateTime DatePaiement { get; set; }
}