using JasperDocs.WebApi.Core;
using JasperDocs.WebApi.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace JasperDocs.WebApi.Features.Parties;

public class ListPartiesHandler : IRequestHandler<ListParties, PaginatedResponse<PartyListItemDto>>
{
    private readonly ApplicationDbContext _context;

    public ListPartiesHandler(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PaginatedResponse<PartyListItemDto>> HandleAsync(
        ListParties request,
        CancellationToken ct = default)
    {
        // Ensure valid pagination parameters
        var pageNumber = request.PageNumber < 1 ? 1 : request.PageNumber;
        var pageSize = request.PageSize < 1 ? 25 : request.PageSize;
        pageSize = Math.Min(pageSize, 100); // Cap at 100 items per page

        // Get total count
        var totalCount = await _context.Parties.CountAsync(ct);

        // Query parties with pagination
        var parties = await _context.Parties
            .OrderBy(p => p.Name)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(p => new PartyListItemDto
            {
                Id = p.Id,
                Name = p.Name,
                CreatedAt = p.CreatedAt
            })
            .ToListAsync(ct);

        return new PaginatedResponse<PartyListItemDto>
        {
            Data = parties,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }
}
