using JasperDocs.WebApi.Core;
using JasperDocs.WebApi.Core.Exceptions;
using JasperDocs.WebApi.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace JasperDocs.WebApi.Features.Parties;

public class GetPartyHandler : IRequestHandler<GetParty, GetPartyResponse>
{
    private readonly ApplicationDbContext _context;

    public GetPartyHandler(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<GetPartyResponse> HandleAsync(
        GetParty request,
        CancellationToken ct = default)
    {
        var party = await _context.Parties
            .FirstOrDefaultAsync(p => p.Id == request.PartyId, ct);

        if (party == null)
        {
            throw new NotFoundException("Party", request.PartyId);
        }

        return new GetPartyResponse
        {
            Id = party.Id,
            Name = party.Name,
            CreatedAt = party.CreatedAt,
            UpdatedAt = party.UpdatedAt
        };
    }
}
