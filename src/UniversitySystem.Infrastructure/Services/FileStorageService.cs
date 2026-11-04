using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using MimeDetective; // Nécessite le package Mime-Detective
using MimeDetective.Definitions;
using MimeDetective.Engine;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace UniversitySystem.Infrastructure.Services;

/// <summary>
/// Contrat pour le stockage sécurisé des fichiers.
/// </summary>
public interface IFileStorageService
{
    Task<string> SaveUploadAsync(IFormFile file, string subDirectory);
}

/// <summary>
/// Implémentation du service de stockage.
/// Applique les règles de sécurité RG-07 (taille, MIME réel, anti-traversal).
/// </summary>
public class FileStorageService : IFileStorageService
{
    private readonly string _rootPath;
    private static readonly IContentInspector _inspector;

    /// <summary>
    /// Initialisation statique de l'inspecteur MIME.
    /// (On utilise des définitions par défaut pour détecter le type de fichier).
    /// </summary>
    static FileStorageService()
    {
        _inspector = new ContentInspectorBuilder()
        {
            Definitions = DefaultDefinitions.All()
        }.Build();
    }

    /// <summary>
    /// Constructeur du service.
    /// </summary>
    /// <param name="config">Configuration de l'application pour lire Storage:RootPath.</param>
    public FileStorageService(IConfiguration config)
    {
        // On lit le chemin défini dans appsettings.json, sinon on utilise C:/univ-storage par défaut
        _rootPath = config["Storage:RootPath"] ?? "C:/univ-storage";
    }

    /// <inheritdoc />
    public async Task<string> SaveUploadAsync(IFormFile file, string subDirectory)
    {
        // 1. Vérification de la présence du fichier
        if (file == null || file.Length == 0)
            throw new ValidationException("Le fichier est vide.");

        // 2. Vérification de la taille (RG-07 : max 2 Mo)
        if (file.Length > 2 * 1024 * 1024)
            throw new ValidationException("Fichier trop volumineux (max 2 Mo).");

        // 3. Détection du vrai type MIME
        using var stream = file.OpenReadStream();
        
        // On inspecte le flux du fichier avec Mime-Detective
        var inspectionResults = _inspector.Inspect(stream);
        
        // On récupère le premier type MIME trouvé
        var mimeType = inspectionResults.ByMimeType().FirstOrDefault()?.MimeType;

        // 4. On n'autorise que le PDF, le JPEG et le PNG
        if (mimeType == null || (mimeType != "application/pdf" && 
            mimeType != "image/jpeg" && 
            mimeType != "image/png"))
        {
            throw new ValidationException("Format non autorisé. Seuls PDF, JPG et PNG sont acceptés.");
        }

        // 5. On détermine l'extension en fonction du MIME détecté
        var extension = mimeType switch
        {
            "application/pdf" => ".pdf",
            "image/jpeg" => ".jpg",
            "image/png" => ".png",
            _ => ".bin"
        };
        
        // 6. On génère un nom aléatoire (GUID) pour éviter les collisions et le path traversal
        var fileName = $"{Guid.NewGuid():N}{extension}";
        
        // 7. Création du dossier physique s'il n'existe pas
        var folderPath = Path.Combine(_rootPath, subDirectory);
        if (!Directory.Exists(folderPath))
            Directory.CreateDirectory(folderPath);

        // 8. Construction du chemin complet
        var filePath = Path.Combine(folderPath, fileName);

        // 9. Écriture du fichier sur le disque
        using var fileStream = new FileStream(filePath, FileMode.Create);
        stream.Position = 0; // On remet le curseur au début
        await stream.CopyToAsync(fileStream);

        // 10. On retourne le chemin relatif pour la base de données (avec des slashs)
        return Path.Combine(subDirectory, fileName).Replace("\\", "/");
    }
}