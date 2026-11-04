using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UniversitySystem.Web.Authorization; // <-- INDISPENSABLE

namespace UniversitySystem.Web.Areas.Scolarite.Controllers;

[Area("Scolarite")]
[Authorize(Roles = "scolarite,superadmin")]
[HasPermission("ETUDIANTS_LISTER")]
public class StudentsController : Controller
{
    public IActionResult Index() => View();
}