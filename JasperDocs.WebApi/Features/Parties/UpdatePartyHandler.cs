using JasperDocs.WebApi.Core;
using JasperDocs.WebApi.Core.Exceptions;
using JasperDocs.WebApi.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace JasperDocs.WebApi.Features.Parties;

public class UpdatePartyHandler : IRequestHandler<UpdateParty>
{
    private readonly ApplicationDbContext _context;

    public UpdatePartyHandler(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task HandleAsync(
        UpdateParty request,
        CancellationToken ct = default)
    {
        // Validate name is not empty
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            throw new ValidationException("Name cannot be empty");
        }

        var party = await _context.Parties
            .FirstOrDefaultAsync(p => p.Id == request.PartyId, ct);

        if (party == null)
        {
            throw new NotFoundException("Party", request.PartyId);
        }

        party.Name = request.Name.Trim();
        party.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(ct);
    }
}
