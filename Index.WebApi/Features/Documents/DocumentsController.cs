using Index.WebApi.Core;
using Microsoft.AspNetCore.Mvc;

namespace Index.WebApi.Features.Documents;

[ApiController]
[Route("[controller]")]
public class DocumentsController : ControllerBase
{
    [HttpPost()]
    public Task CreateDocumentAsync([FromServices] IRequestHandler<CreateDocument> requestHandler, [FromBody] CreateDocument request, CancellationToken ct = default)
    {
        return requestHandler.HandleAsync(request, ct);
    }
}