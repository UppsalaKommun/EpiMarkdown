using System.Reflection;
using System.Web.Mvc;
using System.Web.Routing;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;

namespace UppsalaKommun.EpiMarkdown
{
    [InitializableModule]
    public class Bootstrap : IInitializableModule
    {
        public void Initialize(InitializationEngine context)
        {
            InitializeEmbeddedRoutes();
        }

        private static void InitializeEmbeddedRoutes()
        {
            var routes = RouteTable.Routes;
            var assembly = Assembly.GetExecutingAssembly();
            var assemblyName = assembly.GetName().Name;

            const string markdownEditorJsUrl = "ClientResources/Scripts/Editors/MarkdownEditor.js";
            routes.MapRoute(markdownEditorJsUrl, 
                            markdownEditorJsUrl,
                            new {controller = "Embedded", action = "ClientResources_Scripts_EditorsMarkdownEditor_js"}, 
                            null,
                            new[] {assemblyName});
        }

        public void Preload(string[] parameters)
        {
        }

        public void Uninitialize(InitializationEngine context)
        {
        }
    }
}
