using JasperDocs.WebApi.Core;
using JasperDocs.WebApi.Core.Exceptions;
using JasperDocs.WebApi.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace JasperDocs.WebApi.Features.Documents;

public class RemovePartyFromDocumentHandler : IRequestHandler<RemovePartyFromDocument>
{
    private readonly ApplicationDbContext _context;

    public RemovePartyFromDocumentHandler(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task HandleAsync(RemovePartyFromDocument request, CancellationToken ct = default)
    {
        // Verify document exists
        var documentExists = await _context.Documents
            .AnyAsync(d => d.Id == request.DocumentId, ct);

        if (!documentExists)
        {
            throw new NotFoundException("Document", request.DocumentId);
        }

        // Find and remove the association
        var documentParty = await _context.DocumentParties
            .FirstOrDefaultAsync(dp => dp.DocumentId == request.DocumentId && dp.PartyId == request.PartyId, ct);

        if (documentParty == null)
        {
            // Silently succeed if association doesn't exist (idempotent operation)
            return;
        }

        _context.DocumentParties.Remove(documentParty);
        await _context.SaveChangesAsync(ct);
    }
}
