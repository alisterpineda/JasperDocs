namespace JasperDocs.WebApi.Core;

/// <summary>
/// Helper class for extracting file extensions, including compound extensions like .tar.gz
/// </summary>
public static class FileExtensionHelper
{
    /// <summary>
    /// Common compound file extensions that should be treated as a single extension
    /// </summary>
    private static readonly string[] CompoundExtensions =
    [
        ".tar.gz",
        ".tar.bz2",
        ".tar.xz",
        ".tar.zst"
    ];

    /// <summary>
    /// Extracts the file extension from a filename, including compound extensions.
    /// Returns the extension with the leading dot (e.g., ".pdf", ".tar.gz"), or null if no extension.
    /// </summary>
    /// <param name="filename">The filename to extract the extension from</param>
    /// <returns>The file extension with leading dot, or null if no extension exists</returns>
    public static string? GetFileExtension(string filename)
    {
        if (string.IsNullOrWhiteSpace(filename))
            return null;

        // Check for compound extensions first
        foreach (var compoundExt in CompoundExtensions)
        {
            if (filename.EndsWith(compoundExt, StringComparison.OrdinalIgnoreCase))
                return compoundExt;
        }

        // Fall back to standard extension extraction
        var extension = Path.GetExtension(filename);
        return string.IsNullOrEmpty(extension) ? null : extension;
    }

    /// <summary>
    /// Strips the file extension from a filename, including compound extensions.
    /// </summary>
    /// <param name="filename">The filename to strip the extension from</param>
    /// <returns>The filename without the extension</returns>
    public static string StripExtension(string filename)
    {
        if (string.IsNullOrWhiteSpace(filename))
            return filename;

        var extension = GetFileExtension(filename);

        if (extension == null)
            return filename;

        // Remove the extension from the end
        return filename[..^extension.Length];
    }
}
