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
