using System;
using System.IO;
using System.IO.Compression;

namespace KeepMd;

static class CLIHandler {
    public static void HandleCLIOptions(Options options) {
        string targetPath = ExtractKeepZipArchieve(@$".\{options.SrcFilePath}");
        string[] fileEntries = Directory
                                .GetFiles(targetPath)
                                .Where(file => file.Contains(".html"))
                                .ToArray();

        options.DestFilePath = options.DestFilePath == null ?  
                                    @$".\" 
                                    : 
                                    @$".\{options.DestFilePath}";

        Console.WriteLine(targetPath);
        Console.WriteLine(options.DestFilePath);
    }

    private static string ExtractKeepZipArchieve(string zipPath){
        string extractPath = GetTemporaryDirectory();
        
        using (var file = File.OpenRead(zipPath)) {
            using (var keepZipArchive = new ZipArchive(file, ZipArchiveMode.Read)) {
                keepZipArchive.ExtractToDirectory(extractPath);
            }
        }

        return Path.Combine(extractPath, "Takeout");
    }

    private static string GetTemporaryDirectory() {
        string tempDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        Directory.CreateDirectory(tempDirectory);
        return tempDirectory;
    }
}