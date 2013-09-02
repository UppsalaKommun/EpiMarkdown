EpiMarkdown
===========

This is an attempt to implement a markdown alternative to the default tinymce editor in episerver.

## It should...

- Support episerver soft links
- Insert and DoD images, documents and page links
- Live preview in edit mode

## Setup

1. Add reference to UppsalaKommun.EpiMarkdown
2. Implement UppsalaKommun.EpiMarkdown.Services.ITransformer
3. Implement UppsalaKommun.EpiMarkdown.Transformers.ITransformerFactory
4. Configure IoC container:
        container.For<IMarkdownService>().Use<MarkdownService>();
        container.For<ITransformerFactory>().Use<TransformerFactory>();
5. Add ~/Views/Shared/DisplayTemplates/Markdown.cshtml
6. Add a markdown propery to a page definition:
        [CultureSpecific]
        [Display(Name = "Innehåll (Markdown)", Order = 35)]
        [UIHint(UppsalaKommun.EpiMarkdown.EditorDescriptors.EditorNames.MarkdownEditor, PresentationLayer.Edit)]
        [BackingType(typeof(PropertyMarkdown))]
        public virtual string MarkdownContent { get; set; }
7. Render property in view: @Html.PropertyFor(x => x.CurrentPage.MarkdownContent, new { tag = "Markdown"})

### Implement an ITransformer

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
