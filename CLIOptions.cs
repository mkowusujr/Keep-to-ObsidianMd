namespace KeepMd;
using CommandLine;

/// <summary>
/// The accepted options for the CLI
/// </summary>
public class Options {
    [Value(0, Required = true, HelpText = "Path to keep zip file")]
    public string? SrcFilePath { get; set; }

    [Option('d', HelpText = "Destination Filepath")]
    public string? DestFilePath { get; set; }

    [Option('t', Default = (bool)true, HelpText = "Whether or not to include google keep tags")]
    public bool? TagsOption { get; set; }

    [Option('g', Default = (bool)false, HelpText = "Whether or not to group the files by tags into folders")]
    public bool GroupByTagsOption { get; set; }
}