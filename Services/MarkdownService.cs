using System.Linq;

namespace UppsalaKommun.EpiMarkdown.Services
{
    public interface IMarkdownService
    {
        string Transform(string markdown);
    }

    public class MarkdownService : IMarkdownService
    {
        private readonly ITransformerFactory _transformerFactory;

        public MarkdownService(ITransformerFactory transformerFactory)
        {
            _transformerFactory = transformerFactory;
        }

        public string Transform(string markdown)
        {
            return _transformerFactory.GetTransformers().Aggregate(markdown, (current, transformer) => transformer.Transform(current));
        }
    }
}