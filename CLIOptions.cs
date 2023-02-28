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
}