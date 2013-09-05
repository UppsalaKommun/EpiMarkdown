using System;
using System.Collections.Generic;
using EPiServer.Core;
using EPiServer.Framework.DataAnnotations;
using EPiServer.PlugIn;
using UppsalaKommun.EpiMarkdown.EditorDescriptors;

namespace UppsalaKommun.EpiMarkdown
{
    [EditorHint(EditorNames.MarkdownEditor)]
    [PropertyDefinitionTypePlugIn(Description = "A Markdown replacement for TinyMCE", DisplayName = "Uppsala.Markdown")]
    public class PropertyMarkdown : PropertyLongString//, IReferenceMap
    {
        public override Type PropertyValueType
        {
            get
            {
                return typeof(string);
            }
        }

        public override object SaveData(PropertyDataCollection properties)
        {
            //var test = ReferencedPermanentLinkIds;


            return LongString;
        }

        public override object Value
        {
            get
            {
                var value = base.Value as string;
                return value;
            }
            set
            {
                base.Value = value;
            }
        }

        public override IPropertyControl CreatePropertyControl()
        {
            //No support for legacy edit mode
            return null;
        }

        public IList<Guid> ReferencedPermanentLinkIds
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public void RemapPermanentLinkReferences(IDictionary<Guid, Guid> idMap)
        {
            throw new NotImplementedException();
        }
    }
}