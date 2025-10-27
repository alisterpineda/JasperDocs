namespace JasperDocs.WebApi.Entities;

public class DocumentVersion
{
    public Guid Id { get; set; }
    public Guid DocumentId { get; set; }
    public int VersionNumber { get; set; }
    public string? Description { get; set; }
    public required string StoragePath { get; set; }
    public required string MimeType { get; set; }
    public required string OriginalFileName { get; set; }
    public string? FileExtension { get; set; }
    public DateTime CreatedAt { get; init; }
    public Guid? CreatedByUserId { get; init; }

    #region Navigation Properties

    public Document Document { get; init; } = null!;
    public ApplicationUser? CreatedByUser { get; init; }

    #endregion
}
