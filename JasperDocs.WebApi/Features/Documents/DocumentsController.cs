using JasperDocs.WebApi.Core;
using Microsoft.AspNetCore.Mvc;

namespace JasperDocs.WebApi.Features.Documents;

[ApiController]
[Route("[controller]")]
public class DocumentsController : ControllerBase
{
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