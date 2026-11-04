using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using MimeDetective;
using MimeDetective.Definitions;
using MimeDetective.Engine;
using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace UniversitySystem.Infrastructure.Services;

public interface IFileStorageService
{
    Task<string> SaveUploadAsync(IFormFile file, string subDirectory);
    string GetPhysicalPath(string relativePath);
}

/// <summary>
/// Service de stockage physique des fichiers (RG-07).
/// Sécurise les uploads (taille, MIME, anti-path traversal).
/// </summary>
public class FileStorageService : IFileStorageService
{
    private readonly string _rootPath;
    private static readonly IContentInspector _inspector;

    static FileStorageService()
    {
        _inspector = new ContentInspectorBuilder()
        {
            Definitions = DefaultDefinitions.All()
        }.Build();
    }

    public FileStorageService(IConfiguration config)
    {
        // On stocke le chemin racine normalisé pour éviter les problèmes de séparateurs
        _rootPath = Path.GetFullPath(config["Storage:RootPath"] ?? "C:/univ-storage");
    }

    /// <summary>
    /// Sauvegarde un fichier uploadé de manière sécurisée (RG-07).
    /// </summary>
    public async Task<string> SaveUploadAsync(IFormFile file, string subDirectory)
    {
        if (file == null || file.Length == 0)
            throw new ValidationException("Le fichier est vide.");

        if (file.Length > 2 * 1024 * 1024)
            throw new ValidationException("Fichier trop volumineux (max 2 Mo).");

        using var stream = file.OpenReadStream();
        
        var inspectionResults = _inspector.Inspect(stream);
        var mimeType = inspectionResults.ByMimeType().FirstOrDefault()?.MimeType;

        if (mimeType == null || (mimeType != "application/pdf" && 
            mimeType != "image/jpeg" && 
            mimeType != "image/png"))
        {
            throw new ValidationException("Format non autorisé. Seuls PDF, JPG et PNG sont acceptés.");
        }

        var extension = mimeType switch
        {
            "application/pdf" => ".pdf",
            "image/jpeg" => ".jpg",
            "image/png" => ".png",
            _ => ".bin"
        };
        
        var fileName = $"{Guid.NewGuid():N}{extension}";
        
        // Normalisation du sous-dossier
        var normalizedSubDir = subDirectory.Replace("/", Path.DirectorySeparatorChar.ToString());
        var folderPath = Path.Combine(_rootPath, normalizedSubDir);
        
        if (!Directory.Exists(folderPath))
            Directory.CreateDirectory(folderPath);

        var filePath = Path.Combine(folderPath, fileName);
        using var fileStream = new FileStream(filePath, FileMode.Create);
        stream.Position = 0; 
        await stream.CopyToAsync(fileStream);

        // On retourne un chemin relatif avec des slashs (format standard pour l'URL)
        return Path.Combine(subDirectory, fileName).Replace("\\", "/");
    }

    /// <summary>
    /// Récupère le chemin physique absolu d'un fichier à partir de son chemin relatif (RG-11).
    /// Protège contre le path traversal.
    /// </summary>
    public string GetPhysicalPath(string relativePath)
    {
        if (string.IsNullOrEmpty(relativePath))
            throw new ArgumentException("Le chemin ne peut pas être vide.");

        // 1. Normalisation du chemin (on force les \ de Windows)
        var normalizedRelativePath = relativePath.Replace("/", Path.DirectorySeparatorChar.ToString());
        
        // 2. Combinaison et résolution du chemin absolu
        var fullPath = Path.GetFullPath(Path.Combine(_rootPath, normalizedRelativePath));
        
        // 3. Normalisation de la racine pour comparaison
        var normalizedRoot = _rootPath;
        if (!normalizedRoot.EndsWith(Path.DirectorySeparatorChar))
            normalizedRoot += Path.DirectorySeparatorChar;

        // 4. Vérification anti-path traversal (insensible à la casse)
        if (!fullPath.StartsWith(normalizedRoot, StringComparison.OrdinalIgnoreCase))
            throw new UnauthorizedAccessException("Tentative de path traversal détectée.");

        return fullPath;
    }
}