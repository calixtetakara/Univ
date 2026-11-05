using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UniversitySystem.Web.Authorization;

namespace UniversitySystem.Web.Areas.SuperAdmin.Controllers;

[Area("SuperAdmin")]
[Authorize(Roles = "superadmin")]
[HasPermission("KPI_LIRE")]
public class KpiController : Controller
{
    public IActionResult Index() => View();
}