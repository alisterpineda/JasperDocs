using JasperDocs.WebApi.Core;
using JasperDocs.WebApi.Core.Exceptions;
using JasperDocs.WebApi.Entities;
using JasperDocs.WebApi.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace JasperDocs.WebApi.Features.Documents;

public class AddPartyToDocumentHandler : IRequestHandler<AddPartyToDocument>
{
    private readonly ApplicationDbContext _context;

    public AddPartyToDocumentHandler(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task HandleAsync(AddPartyToDocument request, CancellationToken ct = default)
    {
        // Verify document exists
        var documentExists = await _context.Documents
            .AnyAsync(d => d.Id == request.DocumentId, ct);

        if (!documentExists)
        {
            throw new NotFoundException("Document", request.DocumentId);
        }

        // Verify party exists
        var partyExists = await _context.Parties
            .AnyAsync(p => p.Id == request.PartyId, ct);

        if (!partyExists)
        {
            throw new NotFoundException("Party", request.PartyId);
        }

        // Check if association already exists
        var associationExists = await _context.DocumentParties
            .AnyAsync(dp => dp.DocumentId == request.DocumentId && dp.PartyId == request.PartyId, ct);

        if (associationExists)
        {
            // Silently succeed if association already exists (idempotent operation)
            return;
        }

        // Create the association
        var documentParty = new DocumentParty
        {
            DocumentId = request.DocumentId,
            PartyId = request.PartyId
        };

        _context.DocumentParties.Add(documentParty);
        await _context.SaveChangesAsync(ct);
    }
}
