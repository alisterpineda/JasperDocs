using JasperDocs.WebApi.Core;
using JasperDocs.WebApi.Core.Exceptions;
using JasperDocs.WebApi.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace JasperDocs.WebApi.Features.Documents;

public class GetDocumentHandler : IRequestHandler<GetDocument, GetDocumentResponse>
{
    private readonly ApplicationDbContext _context;

    public GetDocumentHandler(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<GetDocumentResponse> HandleAsync(
        GetDocument request,
        CancellationToken ct = default)
    {
        // Fetch document with all versions and parties
        var document = await _context.Documents
            .Include(d => d.Versions)
            .Include(d => d.DocumentParties)
                .ThenInclude(dp => dp.Party)
            .FirstOrDefaultAsync(d => d.Id == request.DocumentId, ct);

        if (document == null)
        {
            throw new NotFoundException("Document", request.DocumentId);
        }

        // Select the appropriate version
        var selectedVersion = request.VersionNumber.HasValue
            ? document.Versions.FirstOrDefault(v => v.VersionNumber == request.VersionNumber.Value)
            : document.Versions.OrderByDescending(v => v.VersionNumber).FirstOrDefault();

        if (selectedVersion == null)
        {
            var versionInfo = request.VersionNumber.HasValue
                ? $"version {request.VersionNumber.Value}"
                : "any version";
            throw new NotFoundException($"Document {request.DocumentId} does not have {versionInfo}.");
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

        // Map all parties to DTOs
        var parties = document.DocumentParties
            .Select(dp => new DocumentPartyDto
            {
                Id = dp.Party.Id,
                Name = dp.Party.Name
            })
            .OrderBy(p => p.Name)
            .ToList();

        return new GetDocumentResponse
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
            AvailableVersions = availableVersions,
            Parties = parties
        };
    }
}
