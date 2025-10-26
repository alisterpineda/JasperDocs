namespace JasperDocs.WebApi.Core.Exceptions;

/// <summary>
/// Base class for domain exceptions that can be mapped to HTTP responses.
/// </summary>
public abstract class DomainException : Exception
{
    protected DomainException(string message) : base(message)
    {
    }

    protected DomainException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
