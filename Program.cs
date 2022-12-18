string targetDirectory =
    @"C:\Users\mokay\Downloads\Keep";

string[] fileEntries = Directory
    .GetFiles(targetDirectory)
    .Where(file => file.Contains(".html"))
    .ToArray();

Directory.CreateDirectory($@"C:\Users\mokay\Source\Repos\Keep-to-ObsidianMd\output\{"mrfriiday"}");
foreach (string filepath in fileEntries)
{
    Console.WriteLine(filepath);
    var filename = filepath.Split("\\").Last().Replace(".html", string.Empty);
    await NoteConvert.KeepArchieveToObsidianMd(filepath, filename, "mrfriiday");
}
