using System;
using System.Collections.Generic;
using EPiServer.Shell.ObjectEditing;
using EPiServer.Shell.ObjectEditing.EditorDescriptors;

namespace UppsalaKommun.EpiMarkdown.EditorDescriptors
{
    [EditorDescriptorRegistration(TargetType = typeof(string), UIHint = EditorNames.MarkdownEditor)]
    public class MarkdownEditorDescriptor : EditorDescriptor
    {
        public override void ModifyMetadata(ExtendedMetadata metadata, IEnumerable<Attribute> attributes)
        {
            ClientEditingClass = "uk.editors.MarkdownEditor";
            base.ModifyMetadata(metadata, attributes);
        }
    }

    public static class EditorNames
    {
        public const string MarkdownEditor = "MarkdownEditor";
    }
}