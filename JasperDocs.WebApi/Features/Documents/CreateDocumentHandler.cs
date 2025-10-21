using JasperDocs.WebApi.Core;
using JasperDocs.WebApi.Entities;
using JasperDocs.WebApi.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace JasperDocs.WebApi.Features.Documents;

public class CreateDocumentHandler : IRequestHandler<CreateDocument>
{
    private readonly ApplicationDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CreateDocumentHandler(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
    }
    
    public async Task HandleAsync(CreateDocument request, CancellationToken ct = default)
    {
        var newDocument = new Document
        {
            Title = request.Title,
            Description = request.Description ?? string.Empty,
            CreatedByUserId = _httpContextAccessor.GetUserId()
        };
        _context.Documents.Add(newDocument);
        await _context.SaveChangesAsync(ct);
    }
}