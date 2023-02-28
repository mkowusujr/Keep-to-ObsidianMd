using System.IO.Compression;

namespace KeepMd;

static class CLIHandler
{
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
            // Console.WriteLine(filepath);
            var filename = filepath.Split("\\").Last().Replace(".html", string.Empty);
            await NoteConvert.KeepArchieveToObsidianMd(filepath, filename, options);
        }
    }

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

    private static string GetTemporaryDirectory()
    {
        string tempDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        Directory.CreateDirectory(tempDirectory);
        return tempDirectory;
    }
}
