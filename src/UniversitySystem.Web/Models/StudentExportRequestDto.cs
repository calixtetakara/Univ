public class StudentExportRequestDto
{
    public string? SearchNom { get; set; }
    public string? CodeFaculte { get; set; }
    public string? CodeFiliere { get; set; }
    public string Format { get; set; } = "csv"; // csv, xlsx, pdf
}