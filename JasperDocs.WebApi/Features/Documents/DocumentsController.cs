using JasperDocs.WebApi.Core;
using Microsoft.AspNetCore.Mvc;

namespace JasperDocs.WebApi.Features.Documents;

[ApiController]
[Route("[controller]")]
public class DocumentsController : ControllerBase
{
    [HttpGet]
    [ProducesResponseType<PaginatedResponse<DocumentListItemDto>>(StatusCodes.Status200OK)]
    public Task<PaginatedResponse<DocumentListItemDto>> ListDocumentsAsync(
        [FromServices] IRequestHandler<ListDocuments, PaginatedResponse<DocumentListItemDto>> requestHandler,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 25,
        CancellationToken ct = default)
    {
        var request = new ListDocuments { PageNumber = pageNumber, PageSize = pageSize };
        return requestHandler.HandleAsync(request, ct);
    }

    [HttpPost()]
    [Consumes("multipart/form-data")]
    public Task CreateDocumentAsync([FromServices] IRequestHandler<CreateDocument> requestHandler, [FromForm] CreateDocument request, CancellationToken ct = default)
    {
        return requestHandler.HandleAsync(request, ct);
    }

    [HttpPost("versions")]
    [Consumes("multipart/form-data")]
    public Task CreateDocumentVersionAsync([FromServices] IRequestHandler<CreateDocumentVersion> requestHandler, [FromForm] CreateDocumentVersion request, CancellationToken ct = default)
    {
        return requestHandler.HandleAsync(request, ct);
    }
}