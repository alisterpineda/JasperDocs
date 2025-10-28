namespace JasperDocs.WebApi.Entities;

public class DocumentParty
{
    public Guid DocumentId { get; set; }
    public Guid PartyId { get; set; }
    public DateTime CreatedAt { get; init; }

    #region Navigation Properties

    public Document Document { get; init; } = null!;
    public Party Party { get; init; } = null!;

    #endregion
}
