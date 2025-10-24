using JasperDocs.WebApi.Core;
using JasperDocs.WebApi.Entities;
using JasperDocs.WebApi.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace JasperDocs.WebApi.Features.Documents;

public class CreateDocumentVersionHandler : IRequestHandler<CreateDocumentVersion>
{
    private readonly ApplicationDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IOptionsMonitor<StorageOptions> _storageOptions;

    public CreateDocumentVersionHandler(
        ApplicationDbContext context,
        IHttpContextAccessor httpContextAccessor,
        IOptionsMonitor<StorageOptions> storageOptions)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
        _storageOptions = storageOptions;
    }

    public async Task HandleAsync(CreateDocumentVersion request, CancellationToken ct = default)
    {
        // Verify document exists
        var documentExists = await _context.Documents
            .AnyAsync(d => d.Id == request.DocumentId, ct);

        if (!documentExists)
        {
            throw new InvalidOperationException($"Document with ID {request.DocumentId} not found.");
        }

        // Validate storage configuration
        var dataDirectoryPath = _storageOptions.CurrentValue.DataDirectoryPath;
        if (string.IsNullOrWhiteSpace(dataDirectoryPath))
        {
            throw new InvalidOperationException("DataDirectoryPath is not configured.");
        }
        if (!Path.IsPathFullyQualified(dataDirectoryPath))
        {
            throw new InvalidOperationException($"DataDirectoryPath must be an absolute path. Current value: {dataDirectoryPath}");
        }

        var strategy = _context.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(async () =>
        {
            await using var transaction = await _context.Database.BeginTransactionAsync(ct);
            try
            {
                // Get the next version number
                var maxVersion = await _context.DocumentVersions
                    .Where(v => v.DocumentId == request.DocumentId)
                    .MaxAsync(v => (int?)v.VersionNumber, ct) ?? 0;

                var newVersion = new DocumentVersion
                {
                    DocumentId = request.DocumentId,
                    VersionNumber = maxVersion + 1,
                    Description = request.Description,
                    CreatedByUserId = _httpContextAccessor.GetUserId(),
                    StoragePath = string.Empty // Temporary, will update after we have the ID
                };

                _context.DocumentVersions.Add(newVersion);
                await _context.SaveChangesAsync(ct);

                // Now we have the ID, construct the storage path
                var originalFileName = request.File.FileName;
                var relativePath = Path.Combine("documents", request.DocumentId.ToString(), newVersion.Id.ToString(), originalFileName);
                var fullPath = Path.Combine(dataDirectoryPath, relativePath);

                // Create directory if it doesn't exist
                var directory = Path.GetDirectoryName(fullPath);
                if (directory != null && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                // Save the file - if this fails, transaction will rollback
                await using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await request.File.CopyToAsync(stream, ct);
                }

                // Update the storage path
                newVersion.StoragePath = relativePath;
                await _context.SaveChangesAsync(ct);

                // Commit transaction only if everything succeeded
                await transaction.CommitAsync(ct);
            }
            catch
            {
                // Transaction will auto-rollback on dispose if not committed
                throw;
            }
        });
    }
}
