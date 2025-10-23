using JasperDocs.WebApi.Core;
using JasperDocs.WebApi.Entities;
using JasperDocs.WebApi.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace JasperDocs.WebApi.Features.Documents;

public class CreateDocumentVersionHandler : IRequestHandler<CreateDocumentVersion>
{
    private readonly ApplicationDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CreateDocumentVersionHandler(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task HandleAsync(CreateDocumentVersion request, CancellationToken ct = default)
    {
        // Verify document exists
        var documentExists = await _context.Documents
            .AnyAsync(d => d.Id == request.DocumentId, ct);

        if (!documentExists)
        {
            throw new InvalidOperationException($"Document with ID {request.DocumentId} not found.");
        }

        // Get the next version number
        var maxVersion = await _context.DocumentVersions
            .Where(v => v.DocumentId == request.DocumentId)
            .MaxAsync(v => (int?)v.VersionNumber, ct) ?? 0;

        var newVersion = new DocumentVersion
        {
            DocumentId = request.DocumentId,
            VersionNumber = maxVersion + 1,
            Description = request.Description,
            CreatedByUserId = _httpContextAccessor.GetUserId()
        };

        _context.DocumentVersions.Add(newVersion);
        await _context.SaveChangesAsync(ct);
    }
}
