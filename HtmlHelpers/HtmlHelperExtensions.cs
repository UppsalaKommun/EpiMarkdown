using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using EPiServer;
using EPiServer.Core;
using EPiServer.Core.Html.StringParsing;
using EPiServer.Globalization;
using EPiServer.HtmlParsing;
using EPiServer.ServiceLocation;
using EPiServer.Web;
using EPiServer.Web.Routing;

namespace UppsalaKommun.EpiMarkdown.HtmlHelpers
{
    public static class HtmlHelperExtensions
    {
        public static IHtmlString RawButWithMappedUrls(this HtmlHelper helper, string value)
        {
            var formattedString = value;

            var linkFragments = new HtmlStreamReader(value).OfType<ElementFragment>().Where(f => f.NameEquals("a")).ToList();
            if (linkFragments.Any())
            {
                var linkMapper = ServiceLocator.Current.GetInstance<IPermanentLinkMapper>();
                foreach (var linkFragment in linkFragments)
                {
                    if (linkFragment.HasAttributes)
                    {
                        var attribute = linkFragment.Attributes["href"];
                        if (PermanentLinkUtility.IsMappableUrl(new UrlBuilder(attribute.UnquotedValue)))
                        {
                            var url = new UrlFragment(attribute.UnquotedValue, linkMapper);
                            var mappedUrl = MapUrlFromRoute(helper.ViewContext.RequestContext, helper.RouteCollection, url.GetViewFormat());
                            if (!string.IsNullOrEmpty(mappedUrl))
                            {
                                formattedString = value.Replace(attribute.UnquotedValue, mappedUrl);
                            }
                        }
                    }
                }
            }
            return new HtmlString(formattedString);
        }

        private static string MapUrlFromRoute(RequestContext requestContext, RouteCollection routeCollection, string url)
        {
            var mappedUrl = new UrlBuilder(HttpUtility.HtmlDecode(url));
            var contentReference = PermanentLinkUtility.GetContentReference(mappedUrl);
            if (ContentReference.IsNullOrEmpty(contentReference))
            {
                return mappedUrl.ToString();
            }

            var language = mappedUrl.QueryCollection["epslanguage"] ?? (requestContext.GetLanguage() ?? ContentLanguage.PreferredCulture.Name);

            var setIdAsQueryParameter = false;
            bool result;
            if (!string.IsNullOrEmpty(requestContext.HttpContext.Request.QueryString["idkeep"]) &&
                !bool.TryParse(requestContext.HttpContext.Request.QueryString["idkeep"], out result))
            {
                setIdAsQueryParameter = result;
            }

            return routeCollection.GetVirtualPath(contentReference, language, setIdAsQueryParameter, false).GetUrl();
        }

    }
}
