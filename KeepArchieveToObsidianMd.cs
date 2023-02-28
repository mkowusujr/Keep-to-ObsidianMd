using AngleSharp;
using AngleSharp.Html.Dom;
using System.Drawing;
using CommandLine;

namespace KeepMd;

public class NoteConvert
{
    public static async Task KeepArchieveToObsidianMd(
        string inputFilePath,
        string fileName,
        Options options
    )
    {
        #region ReadHtml

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
                            $@"C:\Users\mokay\Source\Repos\Keep-to-ObsidianMd\output\{options.DestFilePath}\{fileName}-img{counter}.jpg"
                        );
                        outPutLines.Add($"![[{fileName}-img{counter}.jpg]]");
                        counter++;
                    }
                }
            }
        }

        #endregion

        #region Create file
        string outputFilePath = $@"{options.DestFilePath}\{fileName}.md";
        if (!Directory.Exists(options.DestFilePath))
            Directory.CreateDirectory(options.DestFilePath ?? @$".\KeepArchive");
        File.WriteAllLines(outputFilePath, outPutLines);
        #endregion
    }
}
