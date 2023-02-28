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
            try
            {
                await CLIHandler.HandleCLIOptions(options);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        });
    }
}
