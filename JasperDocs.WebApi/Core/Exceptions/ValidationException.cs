namespace JasperDocs.WebApi.Core.Exceptions;

/// <summary>
/// Exception thrown when validation fails.
/// Maps to HTTP 400 Bad Request.
/// </summary>
public class ValidationException : DomainException
{
    public ValidationException(string message) : base(message)
    {
    }
}
