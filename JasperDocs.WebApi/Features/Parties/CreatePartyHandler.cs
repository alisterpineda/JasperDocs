using JasperDocs.WebApi.Core;
using JasperDocs.WebApi.Core.Exceptions;
using JasperDocs.WebApi.Entities;
using JasperDocs.WebApi.Infrastructure;

namespace JasperDocs.WebApi.Features.Parties;

public class CreatePartyHandler : IRequestHandler<CreateParty>
{
    private readonly ApplicationDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CreatePartyHandler(
        ApplicationDbContext context,
        IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task HandleAsync(CreateParty request, CancellationToken ct = default)
    {
        // Validate name is not empty
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            throw new ValidationException("Name cannot be empty");
        }

        var userId = _httpContextAccessor.GetUserId();

        var party = new Party
        {
            Name = request.Name.Trim(),
            CreatedByUserId = userId
        };

        _context.Parties.Add(party);
        await _context.SaveChangesAsync(ct);
    }
}
