namespace UppsalaKommun.EpiMarkdown.Services
{
    public interface ITransformer
    {
        string Transform(string markdown);
    }
}