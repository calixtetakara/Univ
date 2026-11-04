namespace UniversitySystem.Web.Models;

/// <summary>
/// ViewModel pour l'Écran 0 (page d'accueil publique).
/// Contient les dates d'inscription chargées dynamiquement.
/// </summary>
public class HomeViewModel
{
    public DateTime? DateLicenceDebut { get; set; }
    public DateTime? DateLicenceFin { get; set; }
    public DateTime? DateMasterDebut { get; set; }
    public DateTime? DateMasterFin { get; set; }

    public bool LicenceOuverte => DateLicenceDebut.HasValue && DateLicenceFin.HasValue 
                                  && DateTime.UtcNow >= DateLicenceDebut && DateTime.UtcNow <= DateLicenceFin;

    public bool MasterOuverte => DateMasterDebut.HasValue && DateMasterFin.HasValue 
                                 && DateTime.UtcNow >= DateMasterDebut && DateTime.UtcNow <= DateMasterFin;
}