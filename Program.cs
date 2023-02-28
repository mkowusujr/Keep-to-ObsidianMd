using CommandLine;

namespace KeepMd;

static class Program
{
	static void Main(string[] args)
	{
		var parser = new Parser(with => with.EnableDashDash = true);
		var result = Parser.Default.ParseArguments<Options>(args);

		result.WithParsed(async options =>
		{
			//     Console.WriteLine($"SrcFilepath = {options.SrcFilePath}");
			//     Console.WriteLine($"Dest = {options.DestFilePath}");
			//     Console.WriteLine($"Output folder name = {options.OutputFileName}");
			//     Console.WriteLine($"Tags option = {options.TagsOption}");
			//     Console.WriteLine($"Flat option = {options.FlatOption}");
			//     Console.WriteLine($"Flat option = {options.FlatOption}");
			//     Console.WriteLine($"Group by tags option = {options.GroupByTagsOption}");
			await CLIHandler.HandleCLIOptions(options);
		});
	}
}
