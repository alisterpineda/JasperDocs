using JasperDocs.WebApi.Core;
using Microsoft.AspNetCore.Http.HttpResults;
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

    [HttpGet("{id:guid}")]
    [ProducesResponseType<GetDocumentResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public Task<Results<Ok<GetDocumentResponse>, NotFound>> GetDocumentAsync(
        [FromServices] IRequestHandler<GetDocument, Results<Ok<GetDocumentResponse>, NotFound>> requestHandler,
        [FromRoute] Guid id,
        [FromQuery] int? versionNumber = null,
        CancellationToken ct = default)
    {
        var request = new GetDocument { DocumentId = id, VersionNumber = versionNumber };
        return requestHandler.HandleAsync(request, ct);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType<string>(StatusCodes.Status400BadRequest)]
    public Task<Results<Ok, NotFound, BadRequest<string>>> UpdateDocumentAsync(
        [FromServices] IRequestHandler<UpdateDocument, Results<Ok, NotFound, BadRequest<string>>> requestHandler,
        [FromRoute] Guid id,
        [FromBody] UpdateDocumentRequest request,
        CancellationToken ct = default)
    {
        var updateRequest = new UpdateDocument
        {
            DocumentId = id,
            Title = request.Title,
            Description = request.Description
        };
        return requestHandler.HandleAsync(updateRequest, ct);
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

    [HttpGet("versions/{versionId:guid}/file")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public Task<Results<PhysicalFileHttpResult, NotFound>> GetDocumentVersionFileAsync(
        [FromServices] IRequestHandler<DownloadDocumentVersion, Results<PhysicalFileHttpResult, NotFound>> requestHandler,
        [FromRoute] Guid versionId,
        CancellationToken ct = default)
    {
        var request = new DownloadDocumentVersion { VersionId = versionId };
        return requestHandler.HandleAsync(request, ct);
    }
}