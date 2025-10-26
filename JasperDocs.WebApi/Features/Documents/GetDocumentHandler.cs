using JasperDocs.WebApi.Core;
using JasperDocs.WebApi.Infrastructure;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace JasperDocs.WebApi.Features.Documents;

public class GetDocumentHandler : IRequestHandler<GetDocument, Results<Ok<GetDocumentResponse>, NotFound>>
{
    private readonly ApplicationDbContext _context;

    public GetDocumentHandler(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Results<Ok<GetDocumentResponse>, NotFound>> HandleAsync(
        GetDocument request,
        CancellationToken ct = default)
    {
        // Fetch document with all versions
        var document = await _context.Documents
            .Include(d => d.Versions)
            .FirstOrDefaultAsync(d => d.Id == request.DocumentId, ct);

        if (document == null)
        {
            return TypedResults.NotFound();
        }

        // Select the appropriate version
        var selectedVersion = request.VersionNumber.HasValue
            ? document.Versions.FirstOrDefault(v => v.VersionNumber == request.VersionNumber.Value)
            : document.Versions.OrderByDescending(v => v.VersionNumber).FirstOrDefault();

        if (selectedVersion == null)
        {
            return TypedResults.NotFound();
        }

        // Map all versions to DTOs
        var availableVersions = document.Versions
            .OrderByDescending(v => v.VersionNumber)
            .Select(v => new DocumentVersionDto
            {
                Id = v.Id,
                VersionNumber = v.VersionNumber,
                Description = v.Description,
                MimeType = v.MimeType,
                CreatedAt = v.CreatedAt
            })
            .ToList();

        var response = new GetDocumentResponse
        {
            Id = document.Id,
            Title = document.Title,
            Description = document.Description,
            CreatedAt = document.CreatedAt,
            UpdatedAt = document.UpdatedAt,
            SelectedVersion = new DocumentVersionDto
            {
                Id = selectedVersion.Id,
                VersionNumber = selectedVersion.VersionNumber,
                Description = selectedVersion.Description,
                MimeType = selectedVersion.MimeType,
                CreatedAt = selectedVersion.CreatedAt
            },
            AvailableVersions = availableVersions
        };

        return TypedResults.Ok(response);
    }
}
