namespace JasperDocs.WebApi.Core;

/// <summary>
/// Domain model representing information needed to download a file.
/// Controllers convert this to the appropriate HTTP file result.
/// </summary>
public class FileDownloadInfo
{
    /// <summary>
    /// The full physical path to the file on disk.
    /// </summary>
    public required string FilePath { get; init; }

    /// <summary>
    /// The MIME type of the file (e.g., "application/pdf").
    /// </summary>
    public required string MimeType { get; init; }

    /// <summary>
    /// The filename to use for the download (visible to the user).
    /// </summary>
    public required string FileName { get; init; }

    /// <summary>
    /// Whether to enable range processing for partial downloads (streaming).
    /// </summary>
    public bool EnableRangeProcessing { get; init; } = true;
}
