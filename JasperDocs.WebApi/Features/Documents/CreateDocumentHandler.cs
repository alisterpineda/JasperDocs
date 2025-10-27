using JasperDocs.WebApi.Core;
using JasperDocs.WebApi.Entities;
using JasperDocs.WebApi.Infrastructure;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace JasperDocs.WebApi.Features.Documents;

public class CreateDocumentHandler : IRequestHandler<CreateDocument>
{
    private readonly ApplicationDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IOptionsMonitor<StorageOptions> _storageOptions;

    public CreateDocumentHandler(
        ApplicationDbContext context,
        IHttpContextAccessor httpContextAccessor,
        IOptionsMonitor<StorageOptions> storageOptions)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
        _storageOptions = storageOptions;
    }

    public async Task HandleAsync(CreateDocument request, CancellationToken ct = default)
    {
        var userId = _httpContextAccessor.GetUserId();
        var originalFileName = request.File.FileName;

        // Extract file extension (including compound extensions like .tar.gz)
        var fileExtension = FileExtensionHelper.GetFileExtension(originalFileName);
        var titleWithoutExtension = FileExtensionHelper.StripExtension(originalFileName);

        // Determine MIME type from file extension
        var provider = new FileExtensionContentTypeProvider();
        if (!provider.TryGetContentType(originalFileName, out var mimeType))
        {
            mimeType = "application/octet-stream";
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
                var newDocument = new Document
                {
                    Title = titleWithoutExtension,
                    Description = null,
                    CreatedByUserId = userId
                };

                var initialVersion = new DocumentVersion
                {
                    Document = newDocument,
                    VersionNumber = 1,
                    Description = "Initial version",
                    CreatedByUserId = userId,
                    StoragePath = string.Empty, // Temporary, will update after we have the ID
                    MimeType = mimeType,
                    OriginalFileName = originalFileName,
                    FileExtension = fileExtension
                };

                _context.Documents.Add(newDocument);
                _context.DocumentVersions.Add(initialVersion);
                await _context.SaveChangesAsync(ct);

                // Now we have the IDs, construct the storage path using GUID + extension
                var fileName = initialVersion.Id.ToString() + (fileExtension ?? string.Empty);
                var relativePath = Path.Combine("documents", newDocument.Id.ToString(), fileName);
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
                initialVersion.StoragePath = relativePath;
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