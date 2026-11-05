using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UniversitySystem.Infrastructure.Export;
using UniversitySystem.Infrastructure.Export.Dtos;

namespace UniversitySystem.Web.Controllers;

[AllowAnonymous]
public class ExportTestController : Controller
{
    private readonly ICsvGenerator _csvGenerator;
    private readonly IPdfGenerator _pdfGenerator;

    public ExportTestController(ICsvGenerator csvGenerator, IPdfGenerator pdfGenerator)
    {
        _csvGenerator = csvGenerator;
        _pdfGenerator = pdfGenerator;
    }

    // ... (DownloadTest et DownloadPdf existants)

    // NOUVELLE ACTION : PDF FINANCES
    public IActionResult DownloadFinancesPdf()
    {
        var transactions = new List<TransactionExportDto>
        {
            new() { Reference = "PAY-001", Matricule = "2026-FDS-00001", NomPrenom = "Dupont Jean", Faculte = "FDS", TypePaiement = "Scolarité", MoyenPaiement = "Mobile Money", Montant = 50000, DatePaiement = DateTime.UtcNow },
            new() { Reference = "PAY-002", Matricule = "2026-FDS-00002", NomPrenom = "José Maria", Faculte = "FDS", TypePaiement = "Scolarité", MoyenPaiement = "Banque", Montant = 75000, DatePaiement = DateTime.UtcNow.AddDays(-1) }
        };

        var bytes = _pdfGenerator.GenerateFinancesPdf(transactions, "Admin Test");
        return File(bytes, "application/pdf", "test_finances.pdf");
    }
}