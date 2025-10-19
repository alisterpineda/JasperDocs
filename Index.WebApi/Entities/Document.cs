namespace Index.WebApi.Entities;

public class Document
{
    public Guid Id { get; set; }
    public required string Title { get; set; }
    public required string Description { get; set; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; set; }
    public Guid? CreatedByUserId { get; init; }

    #region Navigation Properties

    public ApplicationUser? CreatedByUser { get; init; }

    #endregion
}