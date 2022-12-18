using System;
using AngleSharp;
using AngleSharp.Html.Dom;
using System.Drawing;

public class NoteConvert
{
    public static async Task KeepArchieveToObsidianMd(string inputFilePath, string fileName, string outputDirectory)
    {
        #region ReadHtml
        // string filePath = @"C:\Users\mokay\Source\Repos\Keep-to-ObsidianMd\12 29 2015.html";
        // string filePath = @"C:\Users\mokay\Source\Repos\Keep-to-ObsidianMd\To Do List(1).html";
        // string filePath =
        //     @"C:\Users\mokay\Source\Repos\Keep-to-ObsidianMd\Sir Francis Drake (English, 1540-1595).html";

        string keepHtml = string.Join("", System.IO.File.ReadAllLines(inputFilePath));
        #endregion

        #region Setup angle sharp parser
        var config = Configuration.Default;
        using var context = BrowsingContext.New(config);
        using var doc = await context.OpenAsync(req => req.Content(keepHtml));
        #endregion

        #region Turn to Md
        List<string> outPutLines = new();
        #endregion

        // #region  Create fileName
        // string fileName = doc?.Title?.Replace(":", "_") ?? "Untitled";
        // Console.WriteLine(fileName);
        // #endregion

        #region Set title
        if (doc?.Title != null)
        {
            outPutLines.Add($"# {doc.Title}");
        }
        #endregion

        AngleSharp.Dom.IElement? noteContent = doc?.Body?.Children.First()
            .Children.First(element => element.ClassList.Contains("content"));

        #region Handle br elements
        noteContent?.QuerySelectorAll("br").TransformElements(br => br.InnerHtml = "\n");
        #endregion

        #region Convert note content to valid markdown
        string content = noteContent?.TextContent ?? string.Empty;
        #endregion

        #region Convert checkboxes
        content = content.Replace("☐", "\n- [ ] ").Replace("☑", "\n- [x] ").Trim();
        #endregion

        outPutLines.Add(content);

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
                            $@"C:\Users\mokay\Source\Repos\Keep-to-ObsidianMd\output\{outputDirectory}\{fileName}-img{counter}.jpg"
                        );
                        outPutLines.Add($"![[{fileName}-img{counter}.jpg]]");
                        counter++;
                    }
                }
            }
        }

        #endregion

        #region Create file
        string outputFilePath =
            $@"C:\Users\mokay\Source\Repos\Keep-to-ObsidianMd\output\{outputDirectory}\{fileName}.md";
        File.WriteAllLines(outputFilePath, outPutLines);
        // File.SetCreationTime(outputFilePath, new DateTime(""));
        #endregion
    }
}
