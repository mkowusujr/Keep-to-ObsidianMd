using AngleSharp;
using AngleSharp.Html.Dom;
using System.Drawing;
using CommandLine;

namespace KeepMd;

public static class Note
{
    public static async Task KeepHtmlToObsidianMd(
        string inputFilePath,
        string fileName,
        Options options
    )
    {
        // Read html
        string keepHtml = string.Join("", System.IO.File.ReadAllLines(inputFilePath));

        // Setup angle sharp parser
        #region Setup Angle Sharp
        var config = Configuration.Default;
        using var context = BrowsingContext.New(config);
        using var doc = await context.OpenAsync(req => req.Content(keepHtml));
        #endregion


        // Start convertion turn to md
        #region Convert Keep html to Obsidian md
        List<string> outPutLines = new();

        // Add title if avaialbe
        if (doc?.Title != null)
            outPutLines.Add($"# {doc.Title}");

        // Parse main body content
        AngleSharp.Dom.IElement? noteContent = doc?.Body?.Children.First()
            .Children.First(element => element.ClassList.Contains("content"));

        // Convert br elements into new lines
        noteContent?.QuerySelectorAll("br").TransformElements(br => br.InnerHtml = "\n");

        // if file content empty store empty string
        string content = noteContent?.TextContent ?? string.Empty;

        // Convert checkboxes
        content = content.Replace("☐", "\n- [ ] ").Replace("☑", "\n- [x] ").Trim();

        // Store convert content to be outputted later
        outPutLines.Add(content);
        #endregion


        // Handle notes with image attachments
        #region  Handle Images
        AngleSharp.Dom.IElement? noteAttachments = doc?.Body?.Children?.First()
            ?.Children.FirstOrDefault(element => element.ClassList.Contains("attachments"));

        if (noteAttachments != null)
        {
            List<byte[]>? rawImages = noteAttachments
                ?.QuerySelectorAll("img")
                ?.Cast<IHtmlImageElement>()
                ?.Select(imgElement => imgElement?.Source?.Split(",")[1])
                ?.Select(source => Convert.FromBase64String(source ?? string.Empty))
                ?.ToList();

            if (rawImages != null)
            {
                int counter = 1;
                foreach (byte[] rawImage in rawImages)
                {
                    Image image;
                    using (MemoryStream ms = new MemoryStream(rawImage))
                    {
                        image = Image.FromStream(ms);
                        image.Save(
                            $@"C:\Users\mokay\Source\Repos\Keep-to-ObsidianMd\output\{options.DestFilePath}\{fileName}-img{counter}.jpg"
                        );
                        outPutLines.Add($"![[{fileName}-img{counter}.jpg]]");
                        counter++;
                    }
                }
            }
        }
        #endregion


        // Output the converted file
        #region Create file
        string outputFilePath = $@"{options.DestFilePath}\{fileName}.md";
        if (!Directory.Exists(options.DestFilePath))
            Directory.CreateDirectory(options.DestFilePath ?? @$".\KeepArchive");
        File.WriteAllLines(outputFilePath, outPutLines);
        #endregion
    }
}
