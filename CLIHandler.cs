using System.IO.Compression;

namespace KeepMd;

/// <summary>
/// Wrapper class to run cli convertion functions
/// </summary>
static class CLIHandler
{
    /// <summary>
    /// Run the converter function with the input stored in the cli options
    /// </summary>
    /// <param name="options">The CLI options object</param>
    public static async Task HandleCLIOptions(Options options)
    {
        string targetPath = ExtractKeepZipArchieve(@$".\{options.SrcFilePath}");

        options.DestFilePath =
            options.DestFilePath == null ? @$".\KeepArchive" : @$".\{options.DestFilePath}";

        string[] fileEntries = Directory
            .GetFiles(targetPath)
            .Where(file => file.Contains(".html"))
            .ToArray();

        foreach (string filepath in fileEntries)
        {
            var filename = filepath.Split("\\").Last().Replace(".html", string.Empty);
            await Note.KeepHtmlToObsidianMd(filepath, filename, options);
        }
    }

    /// <summary>
    /// Extracts a google keep zip file to a temporary directory.
    /// </summary>
    /// <param name="zipPath">The path to the zip file.</param>
    /// <returns>
    /// The file path to the extracted keep archive's subfolder
    /// containing html files.
    /// </returns>
    private static string ExtractKeepZipArchieve(string zipPath)
    {
        string extractPath = GetTemporaryDirectory();

        using (var file = File.OpenRead(zipPath))
        {
            using (var keepZipArchive = new ZipArchive(file, ZipArchiveMode.Read))
            {
                keepZipArchive.ExtractToDirectory(extractPath);
            }
        }

        return Path.Combine(extractPath, @"Takeout\Keep");
    }

    /// <summary>
    /// Creates a temporary directory.
    /// </summary>
    /// <returns>The filepath to the temp directory.</returns>
    private static string GetTemporaryDirectory()
    {
        string tempDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        Directory.CreateDirectory(tempDirectory);
        return tempDirectory;
    }
}
