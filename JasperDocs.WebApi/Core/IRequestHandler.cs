namespace JasperDocs.WebApi.Core;

public interface IRequestHandler<TRequest, TResponse>
{
    Task<TResponse> HandleAsync(TRequest request, CancellationToken ct = default);
}

public interface IRequestHandler<TRequest>
{
    Task HandleAsync(TRequest request, CancellationToken ct = default);
}