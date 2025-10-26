using JasperDocs.WebApi.Core;
using JasperDocs.WebApi.Core.Exceptions;
using JasperDocs.WebApi.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace JasperDocs.WebApi.Features.Documents;

public class DownloadDocumentVersionHandler : IRequestHandler<DownloadDocumentVersion, FileDownloadInfo>
{
    private readonly ApplicationDbContext _context;
    private readonly IOptionsMonitor<StorageOptions> _storageOptions;

    public DownloadDocumentVersionHandler(
        ApplicationDbContext context,
        IOptionsMonitor<StorageOptions> storageOptions)
    {
        _context = context;
        _storageOptions = storageOptions;
    }

    public async Task<FileDownloadInfo> HandleAsync(
        DownloadDocumentVersion request,
        CancellationToken ct = default)
    {
        // Fetch the document version
        var version = await _context.DocumentVersions
            .AsNoTracking()
            .FirstOrDefaultAsync(v => v.Id == request.VersionId, ct);

        if (version == null)
        {
            throw new NotFoundException("DocumentVersion", request.VersionId);
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

        // Build the full file path
        var fullPath = Path.Combine(dataDirectoryPath, version.StoragePath);

        // Normalize the path to prevent path traversal attacks
        var normalizedFullPath = Path.GetFullPath(fullPath);
        var normalizedDataPath = Path.GetFullPath(dataDirectoryPath);

        // Ensure the file is within the data directory
        if (!normalizedFullPath.StartsWith(normalizedDataPath, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Invalid file path detected.");
        }

        // Check if file exists
        if (!File.Exists(normalizedFullPath))
        {
            throw new NotFoundException($"File for DocumentVersion {request.VersionId} not found on disk.");
        }

        // Extract filename from the storage path
        var fileName = Path.GetFileName(version.StoragePath);

        return new FileDownloadInfo
        {
            FilePath = normalizedFullPath,
            MimeType = version.MimeType,
            FileName = fileName,
            EnableRangeProcessing = true
        };
    }
}
