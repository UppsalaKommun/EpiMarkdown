using EPiServer;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.Framework;
using EPiServer.ServiceLocation;
using EPiServer.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using UppsalaKommun.EpiMarkdown.Services;
using EPiServer.HtmlParsing;

namespace UppsalaKommun.EpiMarkdown
{
    [InitializableModule]
    public class MarkdownSoftLinkModule : IInitializableModule
    {
        private bool _eventsAttached;
        private ContentSoftLinkRepository _contentSoftLinkRepository;
        private IMarkdownService _markdownService;

        private IMarkdownService MarkdownService
        {
            get
            {
                return _markdownService ?? (_markdownService = ServiceLocator.Current.GetInstance<IMarkdownService>());
            }
        }

        private ContentSoftLinkRepository ContentSoftLinkRepository
        {
            get
            {
                return _contentSoftLinkRepository ?? (_contentSoftLinkRepository = ServiceLocator.Current.GetInstance<ContentSoftLinkRepository>());
            }
        }

        #region IInitializableModule members

        public void Initialize(EPiServer.Framework.Initialization.InitializationEngine context)
        {
            if (!_eventsAttached)
            {
                DataFactory.Instance.PublishedPage += Instance_PublishedPage;
                _eventsAttached = true;
            }
        }

        public void Uninitialize(EPiServer.Framework.Initialization.InitializationEngine context)
        {
            DataFactory.Instance.PublishedPage -= Instance_PublishedPage;
        }

        public void Preload(string[] parameters)
        {
            throw new NotImplementedException();
        }

        #endregion

        void Instance_PublishedPage(object sender, PageEventArgs e)
        {
            SaveSoftLinks(e.Content);
        }

        private static bool IsValidLinkAttribute(AttributeFragment attribute)
        {
            if (attribute == null || string.IsNullOrWhiteSpace(attribute.UnquotedValue) ||
                attribute.UnquotedValue.StartsWith("#"))
            {
                return false;
            }
            
            return Uri.IsWellFormedUriString(attribute.UnquotedValue, UriKind.RelativeOrAbsolute);
        }

        private void SaveSoftLinks(IContent content)
        {
            var contentData = content as IContentData;
            if (contentData == null)
                return;
            
            foreach (var propertyData in contentData.Property)
            {
                var data = propertyData as PropertyMarkdown;
                if (data != null)
                {
                    var markdownAsHtml = MarkdownService.Transform(data.Value as string);
                    var htmlReader = new HtmlStreamReader(markdownAsHtml);

                    var linkFragments = htmlReader.OfType<ElementFragment>().Where(f => f.NameEquals("a")).ToList();
                    if (linkFragments.Any())
                    {
                        var softLinks = new List<SoftLink>();

                        foreach (var elementFragment in linkFragments)
                        {
                            if (elementFragment.HasAttributes)
                            {
                                var attribute = elementFragment.Attributes["href"];
                                if (IsValidLinkAttribute(attribute) && PermanentLinkUtility.IsMappableUrl(new UrlBuilder(attribute.UnquotedValue)))
                                {
                                    softLinks.Add(new SoftLink
                                    {
                                        OwnerContentLink = content.ContentLink.CreateReferenceWithoutVersion(),
                                        OwnerLanguage = content is ILocalizable ? ((ILocalizable)content).Language : null,
                                        SoftLinkType = ReferenceType.PageLinkReference,
                                        Url = attribute.UnquotedValue
                                    });
                                }
                            }
                        }

                        ContentSoftLinkRepository.Save(content.ContentLink.CreateReferenceWithoutVersion(), content is ILocalizable ? ((ILocalizable)content).Language : null, softLinks);
                    }
                }
            }
        }
    }
}