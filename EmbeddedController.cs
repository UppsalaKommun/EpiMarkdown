using System.IO;
using System.Reflection;
using System.Web.Mvc;

namespace UppsalaKommun.EpiMarkdown
{
    public class EmbeddedController : Controller
    {
        private readonly Assembly _assembly;
        private readonly string _assemblyName;

        public EmbeddedController()
        {
            _assembly = Assembly.GetExecutingAssembly();
            _assemblyName = _assembly.GetName().Name;
        }

        public ActionResult ClientResources_Scripts_EditorsMarkdownEditor_js()
        {
            using (var stream = _assembly.GetManifestResourceStream(string.Format("{0}.ClientResources.Scripts.Editors.MarkdownEditor.js", _assemblyName)))
            using (var reader = new StreamReader(stream))
            {
                var result = reader.ReadToEnd();
                return Content(result);
            }
        }
    }
}