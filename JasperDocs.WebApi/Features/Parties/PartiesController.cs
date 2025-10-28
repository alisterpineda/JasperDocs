using JasperDocs.WebApi.Core;
using Microsoft.AspNetCore.Mvc;

namespace JasperDocs.WebApi.Features.Parties;

[ApiController]
[Route("[controller]")]
public class PartiesController : ControllerBase
{
    [HttpGet]
    [ProducesResponseType<PaginatedResponse<PartyListItemDto>>(StatusCodes.Status200OK)]
    public Task<PaginatedResponse<PartyListItemDto>> ListPartiesAsync(
        [FromServices] IRequestHandler<ListParties, PaginatedResponse<PartyListItemDto>> requestHandler,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 25,
        CancellationToken ct = default)
    {
        var request = new ListParties { PageNumber = pageNumber, PageSize = pageSize };
        return requestHandler.HandleAsync(request, ct);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType<GetPartyResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public Task<GetPartyResponse> GetPartyAsync(
        [FromServices] IRequestHandler<GetParty, GetPartyResponse> requestHandler,
        [FromRoute] Guid id,
        CancellationToken ct = default)
    {
        var request = new GetParty { PartyId = id };
        return requestHandler.HandleAsync(request, ct);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<string>(StatusCodes.Status400BadRequest)]
    public Task CreatePartyAsync(
        [FromServices] IRequestHandler<CreateParty> requestHandler,
        [FromBody] CreateParty request,
        CancellationToken ct = default)
    {
        return requestHandler.HandleAsync(request, ct);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType<string>(StatusCodes.Status400BadRequest)]
    public Task UpdatePartyAsync(
        [FromServices] IRequestHandler<UpdateParty> requestHandler,
        [FromRoute] Guid id,
        [FromBody] UpdatePartyRequest request,
        CancellationToken ct = default)
    {
        var updateRequest = new UpdateParty
        {
            PartyId = id,
            Name = request.Name
        };
        return requestHandler.HandleAsync(updateRequest, ct);
    }
}
