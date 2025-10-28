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

    [HttpGet("{id:guid}")]
    [ProducesResponseType<GetDocumentResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public Task<GetDocumentResponse> GetDocumentAsync(
        [FromServices] IRequestHandler<GetDocument, GetDocumentResponse> requestHandler,
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
    public Task UpdateDocumentAsync(
        [FromServices] IRequestHandler<UpdateDocument> requestHandler,
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
    public async Task<IActionResult> GetDocumentVersionFileAsync(
        [FromServices] IRequestHandler<DownloadDocumentVersion, FileDownloadInfo> requestHandler,
        [FromRoute] Guid versionId,
        CancellationToken ct = default)
    {
        var request = new DownloadDocumentVersion { VersionId = versionId };
        var fileInfo = await requestHandler.HandleAsync(request, ct);

        // Convert domain model to HTTP file result
        return PhysicalFile(
            fileInfo.FilePath,
            fileInfo.MimeType,
            fileInfo.FileName,
            fileInfo.EnableRangeProcessing);
    }

    [HttpGet("{id:guid}/parties")]
    [ProducesResponseType<IReadOnlyList<DocumentPartyDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public Task<IReadOnlyList<DocumentPartyDto>> ListDocumentPartiesAsync(
        [FromServices] IRequestHandler<ListDocumentParties, IReadOnlyList<DocumentPartyDto>> requestHandler,
        [FromRoute] Guid id,
        CancellationToken ct = default)
    {
        var request = new ListDocumentParties { DocumentId = id };
        return requestHandler.HandleAsync(request, ct);
    }

    [HttpPost("{id:guid}/parties")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public Task AddPartyToDocumentAsync(
        [FromServices] IRequestHandler<AddPartyToDocument> requestHandler,
        [FromRoute] Guid id,
        [FromBody] AddPartyToDocumentRequest request,
        CancellationToken ct = default)
    {
        var addRequest = new AddPartyToDocument
        {
            DocumentId = id,
            PartyId = request.PartyId
        };
        return requestHandler.HandleAsync(addRequest, ct);
    }

    [HttpDelete("{id:guid}/parties/{partyId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public Task RemovePartyFromDocumentAsync(
        [FromServices] IRequestHandler<RemovePartyFromDocument> requestHandler,
        [FromRoute] Guid id,
        [FromRoute] Guid partyId,
        CancellationToken ct = default)
    {
        var removeRequest = new RemovePartyFromDocument
        {
            DocumentId = id,
            PartyId = partyId
        };
        return requestHandler.HandleAsync(removeRequest, ct);
    }
}