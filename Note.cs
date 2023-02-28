using AngleSharp;
using AngleSharp.Html.Dom;
using System.Drawing;
using CommandLine;

namespace KeepMd;

/// <summary>
/// A class contianing convertion functions for different notes
/// </summary>
public static class Note
{
    /// <summary>
    /// Converts a google keep archive file to html
    /// </summary>
    /// <param name="inputFilePath">The current file being converted</param>
    /// <param name="fileName">The new file's name</param>
    /// <param name="options">The CLI options object</param>
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

        // Setup note body for easy querying
        var noteBody = doc?.Body?.Children?.First()?.Children;

        // Parse main body content
        AngleSharp.Dom.IElement? noteContent = noteBody?.First(
            element => element.ClassList.Contains("content")
        );

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
        AngleSharp.Dom.IElement? noteAttachments = noteBody?.FirstOrDefault(
            element => element.ClassList.Contains("attachments")
        );

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
                        if (!Directory.Exists(@$"{options.DestFilePath}\media"))
                            Directory.CreateDirectory(@$"{options.DestFilePath}\media");

                        image = Image.FromStream(ms);
                        image.Save($@"{options.DestFilePath}\media\{fileName}-img-{counter}.jpg");
                        outPutLines.Add($"\n![[{fileName}-img-{counter}.jpg]]");
                        counter++;
                    }
                }
            }
        }
        #endregion


        // Parse tags
        #region Handle tags
        var tags = noteBody
            ?.FirstOrDefault(element => element.ClassList.Contains("chips"))
            ?.Children?.FirstOrDefault(element => element.ClassList.Contains("chip"))
            ?.GetElementsByClassName("label-name")
            ?.Select(element => element.InnerHtml)
            ?.ToList();

        string listOfTags = string.Empty;
        tags?.ForEach(tag => listOfTags += ($"#{tag.Replace(" ", "-")} "));

        if (!listOfTags.Equals(string.Empty))
            outPutLines.Add($"\n{listOfTags.Trim()}");
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
