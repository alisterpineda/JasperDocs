using JasperDocs.WebApi.Core;
using JasperDocs.WebApi.Core.Exceptions;
using JasperDocs.WebApi.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace JasperDocs.WebApi.Features.Documents;

public class ListDocumentPartiesHandler : IRequestHandler<ListDocumentParties, IReadOnlyList<DocumentPartyDto>>
{
    private readonly ApplicationDbContext _context;

    public ListDocumentPartiesHandler(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<DocumentPartyDto>> HandleAsync(
        ListDocumentParties request,
        CancellationToken ct = default)
    {
        // Verify document exists
        var documentExists = await _context.Documents
            .AnyAsync(d => d.Id == request.DocumentId, ct);

        if (!documentExists)
        {
            throw new NotFoundException("Document", request.DocumentId);
        }

        // Get all parties associated with the document
        var parties = await _context.DocumentParties
            .Where(dp => dp.DocumentId == request.DocumentId)
            .Select(dp => new DocumentPartyDto
            {
                Id = dp.Party.Id,
                Name = dp.Party.Name
            })
            .OrderBy(p => p.Name)
            .ToListAsync(ct);

        return parties;
    }
}
