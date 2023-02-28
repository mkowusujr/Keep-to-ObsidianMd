// string targetDirectory =
//     @"C:\Users\mokay\Downloads\Keep";

// string[] fileEntries = Directory
//     .GetFiles(targetDirectory)
//     .Where(file => file.Contains(".html"))
//     .ToArray();

// Directory.CreateDirectory($@"C:\Users\mokay\Source\Repos\Keep-to-ObsidianMd\output\{"mrfriiday"}");
// foreach (string filepath in fileEntries)
// {
//     Console.WriteLine(filepath);
//     var filename = filepath.Split("\\").Last().Replace(".html", string.Empty);
//     await NoteConvert.KeepArchieveToObsidianMd(filepath, filename, "mrfriiday");
// }
using CommandLine;

namespace KeepMd;

static class Program {
    static void Main (string[] args) {
        var parser = new Parser(with => with.EnableDashDash = true);
        var result = Parser.Default.ParseArguments<Options>(args);

        result.WithParsed(options =>
        {
        //     Console.WriteLine($"SrcFilepath = {options.SrcFilePath}");
        //     Console.WriteLine($"Dest = {options.DestFilePath}");
        //     Console.WriteLine($"Output folder name = {options.OutputFileName}");
        //     Console.WriteLine($"Tags option = {options.TagsOption}");
        //     Console.WriteLine($"Flat option = {options.FlatOption}");
        //     Console.WriteLine($"Flat option = {options.FlatOption}");
        //     Console.WriteLine($"Group by tags option = {options.GroupByTagsOption}");
            CLIHandler.HandleCLIOptions(options);
        });
        // run cli options
    }
}