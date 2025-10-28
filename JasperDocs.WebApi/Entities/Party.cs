namespace JasperDocs.WebApi.Entities;

public class Party
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; set; }
    public Guid? CreatedByUserId { get; init; }

    #region Navigation Properties

    public ApplicationUser? CreatedByUser { get; init; }
    public ICollection<DocumentParty> DocumentParties { get; set; } = new List<DocumentParty>();

    #endregion
}
