using JasperDocs.WebApi.Core;
using JasperDocs.WebApi.Infrastructure;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace JasperDocs.WebApi.Features.Documents;

public class UpdateDocumentHandler : IRequestHandler<UpdateDocument, Results<Ok, NotFound, BadRequest<string>>>
{
    private readonly ApplicationDbContext _context;

    public UpdateDocumentHandler(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Results<Ok, NotFound, BadRequest<string>>> HandleAsync(
        UpdateDocument request,
        CancellationToken ct = default)
    {
        // Validate title is not empty
        if (string.IsNullOrWhiteSpace(request.Title))
        {
            return TypedResults.BadRequest("Title cannot be empty");
        }

        var document = await _context.Documents
            .FirstOrDefaultAsync(d => d.Id == request.DocumentId, ct);

        if (document == null)
        {
            return TypedResults.NotFound();
        }

        document.Title = request.Title;
        document.Description = request.Description;
        document.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(ct);

        return TypedResults.Ok();
    }
}
