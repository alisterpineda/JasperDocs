using JasperDocs.WebApi.Core;
using JasperDocs.WebApi.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace JasperDocs.WebApi.Features.Documents;

public class ListDocumentsHandler : IRequestHandler<ListDocuments, PaginatedResponse<DocumentListItemDto>>
{
    private readonly ApplicationDbContext _context;

    public ListDocumentsHandler(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PaginatedResponse<DocumentListItemDto>> HandleAsync(
        ListDocuments request,
        CancellationToken ct = default)
    {
        // Ensure valid pagination parameters
        var pageNumber = request.PageNumber < 1 ? 1 : request.PageNumber;
        var pageSize = request.PageSize < 1 ? 25 : request.PageSize;
        pageSize = Math.Min(pageSize, 100); // Cap at 100 items per page

        // Get total count
        var totalCount = await _context.Documents.CountAsync(ct);

        // Query documents with pagination
        var documents = await _context.Documents
            .OrderByDescending(d => d.UpdatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(d => new DocumentListItemDto
            {
                Id = d.Id,
                Title = d.Title,
                Description = d.Description,
                CreatedAt = d.CreatedAt
            })
            .ToListAsync(ct);

        return new PaginatedResponse<DocumentListItemDto>
        {
            Data = documents,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }
}
