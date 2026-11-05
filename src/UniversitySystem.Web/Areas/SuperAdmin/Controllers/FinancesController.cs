using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UniversitySystem.Web.Authorization;

namespace UniversitySystem.Web.Areas.SuperAdmin.Controllers;

[Area("SuperAdmin")]
[Authorize(Roles = "superadmin")]
[HasPermission("FINANCES_LIRE")]
public class FinancesController : Controller
{
    public IActionResult Index() => View();
}