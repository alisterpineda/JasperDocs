using JasperDocs.WebApi.Core;
using JasperDocs.WebApi.Core.Exceptions;
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

        document.Title = request.Title;
        document.Description = request.Description;
        document.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(ct);
    }
}
