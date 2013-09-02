using System.Collections.Generic;

namespace UppsalaKommun.EpiMarkdown.Services
{
    public interface ITransformerFactory
    {
        IEnumerable<ITransformer> GetTransformers();
    }
}