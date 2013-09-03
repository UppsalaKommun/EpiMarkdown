EpiMarkdown
===========

This is an attempt to implement a [Markdown](http://en.wikipedia.org/wiki/Markdown) alternative to the default [TinyMCE](http://www.tinymce.com) editor in [EPiServer](http://www.episerver.com/).

## Why Markdown in EPiServer!?

slubbo, rock on here...

## It should...

The project is in an experimental stage. But if it turns out to be as useful as we hope, it should include:

- Support episerver soft links
- Insert and DnD images, documents and page links
- Live preview in edit mode

<img src="https://raw.github.com/UppsalaKommun/EpiMarkdown/master/EPiMarkdown.png" alt="Screenshot" />
[ScreenShot](https://raw.github.com/UppsalaKommun/EpiMarkdown/master/EPiMarkdown.png)

## Setup

1. Add reference to UppsalaKommun.EpiMarkdown
2. Implement UppsalaKommun.EpiMarkdown.Services.ITransformer
3. Implement UppsalaKommun.EpiMarkdown.Transformers.ITransformerFactory
4. Configure IoC container:

        container.For<IMarkdownService>().Use<MarkdownService>();
        container.For<ITransformerFactory>().Use<TransformerFactory>();
        
5. Add ~/Views/Shared/DisplayTemplates/Markdown.cshtml
6. Add a markdown propery to a page definition:

        [UIHint(UppsalaKommun.EpiMarkdown.EditorDescriptors.EditorNames.MarkdownEditor, PresentationLayer.Edit)]
        [BackingType(typeof(PropertyMarkdown))]
        public virtual string MarkdownContent { get; set; }

7. Render property in view:

        @Html.PropertyFor(x => x.CurrentPage.MarkdownContent, new { tag = "Markdown"})

### Implement an ITransformer

Write your own Markdown to HTML engine. Or fetch one from nuget. Such as [MarkdownSharp](http://www.nuget.org/packages/MarkdownSharp/) or [MarkdownDeep](http://www.nuget.org/packages/MarkdownDeep.NET/).

        Install-Package MarkdownDeep.NET

Implement ITransformer:

    public class MarkdownDeepTransformer : ITransformer
    {
        public string Transform(string markdown)
        {
            var response = markdown;

            var md = new MarkdownDeep.Markdown
            {
                SafeMode = false,
                ExtraMode = true,
                AutoHeadingIDs = true,
                MarkdownInHtml = true,
                NewWindowForExternalLinks = true
            };

            response = md.Transform(response);

            return response;
        }
    }

### Implement an ITransformerFactory

    public class TransformerFactory : ITransformerFactory
    {
        public IEnumerable<ITransformer> GetTransformers()
        {
            return new List<ITransformer>
                {
                    new MarkdownDeepTransformer(),
                    new DocumentBoxTransformer(),
                    new InformationBoxTransformer(),
                    new WarningBoxTransformer()
                };
        }
    }

### Add Markdown.cshtml

        @using UppsalaKommun.EpiMarkdown.Services
        @model string
        @{
                var markdownEngine = EPiServer.ServiceLocation.ServiceLocator.Current.GetInstance<IMarkdownService>();
                var response = markdownEngine.Transform(Model);
        }
        @Html.Raw(response)
