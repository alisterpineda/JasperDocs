using JasperDocs.WebApi.Core;
using JasperDocs.WebApi.Core.Exceptions;
using JasperDocs.WebApi.Entities;
using JasperDocs.WebApi.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace JasperDocs.WebApi.Features.Documents;

public class UpdateDocumentHandler : IRequestHandler<UpdateDocument>
{
    private readonly ApplicationDbContext _context;

    public UpdateDocumentHandler(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task HandleAsync(
        UpdateDocument request,
        CancellationToken ct = default)
    {
        // Validate title is not empty
        if (string.IsNullOrWhiteSpace(request.Title))
        {
            throw new ValidationException("Title cannot be empty");
        }

        var document = await _context.Documents
            .FirstOrDefaultAsync(d => d.Id == request.DocumentId, ct);

        if (document == null)
        {
            throw new NotFoundException("Document", request.DocumentId);
        }

        // Validate all party IDs exist
        if (request.PartyIds.Count > 0)
        {
            var distinctPartyIds = request.PartyIds.Distinct().ToList();
            var existingPartyIds = await _context.Parties
                .Where(p => distinctPartyIds.Contains(p.Id))
                .Select(p => p.Id)
                .ToListAsync(ct);

            var invalidPartyIds = distinctPartyIds.Except(existingPartyIds).ToList();
            if (invalidPartyIds.Count > 0)
            {
                throw new ValidationException(
                    $"The following party IDs do not exist: {string.Join(", ", invalidPartyIds)}");
            }
        }

        // Use transaction for atomic updates
        var strategy = _context.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(async () =>
        {
            await using var transaction = await _context.Database.BeginTransactionAsync(ct);
            try
            {
                // Update document properties
                document.Title = request.Title;
                document.Description = request.Description;
                document.UpdatedAt = DateTime.UtcNow;

                // Remove all existing party associations
                var existingAssociations = await _context.DocumentParties
                    .Where(dp => dp.DocumentId == request.DocumentId)
                    .ToListAsync(ct);
                _context.DocumentParties.RemoveRange(existingAssociations);

                // Add new party associations
                var distinctPartyIds = request.PartyIds.Distinct().ToList();
                var newAssociations = distinctPartyIds.Select(partyId => new DocumentParty
                {
                    DocumentId = request.DocumentId,
                    PartyId = partyId
                }).ToList();
                _context.DocumentParties.AddRange(newAssociations);

                await _context.SaveChangesAsync(ct);
                await transaction.CommitAsync(ct);
            }
            catch
            {
                throw; // Auto-rollback on dispose
            }
        });
    }
}
