/* uk.editors.MarkdownEditor */

define([
    "dojo",
    "dojo/dom",
    "dojo/_base/connect",
    "dojo/_base/declare",
    "dojo/_base/lang",
    "dojo/_base/Deferred",
    "dojo/query",

    "dijit/_CssStateMixin",
    "dijit/_Widget",
    "dijit/_TemplatedMixin",
    "dijit/_WidgetsInTemplateMixin",
    "dijit/form/Textarea",

    // epi.shell
    "epi",
    "epi/dependency",
    "epi/shell/widget/dialog/Dialog",
    "epi/shell/dnd/Target",
    //"epi/cms/widget/_DndStateMixin",

    "epi/epi",
    "epi/shell/widget/_ValueRequiredMixin",
    //"epi/cms/widget/ContentSelectorDialog",
    "epi/cms/conversion/ContentLightUriConverter",
    "epi/cms/core/ContentReference",
    "epi/cms/widget/_Droppable",
    "epi/cms/widget/_HasChildDialogMixin",

    //resources
    "epi/i18n!epi/cms/nls/episerver.cms.widget.PageSelector",

    "dojo/domReady!"
],
function (
    dojo,
    dom,
    connect,
    declare,
    lang,
    Deferred,
    query,

    _CssStateMixin,
    _Widget,
    _TemplatedMixin,
    _WidgetsInTemplateMixin,
    Textarea,

    // epi.shell
    epi,
    dependency,
    Dialog,
    Target,
    //_DndStateMixin,

    epi,
    _ValueRequiredMixin,
    //ContentSelectorDialog,
    ContentLightUriConverter,
    ContentReference,
    _Droppable,
    _HasChildDialogMixin,

    //resources
    localization
) {

    return declare("uk.editors.MarkdownEditor", [_Widget, _TemplatedMixin, _WidgetsInTemplateMixin, _CssStateMixin, _HasChildDialogMixin, _ValueRequiredMixin], {

        templateString: "\
            <div class=\"dijitInline\">\
                <p><strong>##</strong>Rubrik 2<br/>\
                <strong>###</strong>Rubrik 3<br/>\
                <strong>^</strong>Informationsruta<strong>^</strong><br/>\
                <strong>%</strong>Varningsruta<strong>%</strong><br/>\
                <strong>$D</strong>Dokumentruta<strong>$D</strong><br/>\
                <strong>[</strong>L&auml;nk<strong>](</strong>http://www.sprakradet.se/<strong>)</strong></p>\
                <p><a href='http://insidan.uppsala.se/Markdown' target='_blank' style='text-decoration:underline;color:blue;'>Hj&auml;lp, hur skriver jag Markdown?</a></p>\
                <div data-dojo-attach-point=\"stateNode, tooltipNode, dropAreaNode\">\
                    <textarea data-dojo-attach-point=\"textArea\" data-dojo-type=\"dijit.form.Textarea\" style=\"width: 350px\"></textarea>\
                </div>\
            </div>",

        dropAreaNode: null,
        acceptDataTypes: null,
        dropTarget: null,
        intermediateChanges: false,
        value: null,
        markdownAsHtml: null,
        multiple: true,
        _markerPosition: null,
        _markedText: null,
        _converter: null,

        uninitialize: function () {
            this.inherited(arguments);
            this._dropTarget.destroy();
        },

        postCreate: function () {
            this.inherited(arguments);

            // Init textarea and bind event
            this.textArea.set("intermediateChanges", this.intermediateChanges);
            this.connect(this.textArea, "onChange", this._onTextAreaChanged);
            this.connect(this.textArea, "onBlur", this._onBlur);
            this.connect(this.textArea, "onFocus", this._onFocus);

            //Define drop target 
            this._dropTarget = Target(this.dropAreaNode, {
                accept: this.acceptDataTypes,
                createItemOnDrop: false,
                readOnly: this.readOnly
            });
            this.connect(this._dropTarget, "onDropData", "_onDropData");
        },

        postMixInProperties: function () {
            // summary:
            //		Initialize properties
            // tags:
            //    protected
            this.inherited(arguments);
            this._pageDataStore = this._pageDataStore || dependency.resolve("epi.storeregistry").get("epi.cms.content.light");
            this.acceptDataTypes = this.acceptDataTypes || ["epi.cms.pagereference"];
        },

        onChange: function (value) {
        },

        isValid: function () {
            // summary:
            //    Check if widget's value is valid.
            // tags:
            //    protected, override
            return true;
        },
        _onBlur: function () {
            //store marker position when focus is lost
            if (this._focusManager.curNode) {
                var selectionStartIndex = this._focusManager.curNode.selectionStart;
                var selectionEndIndex = this._focusManager.curNode.selectionEnd;

                this._markerPosition = selectionEndIndex;
                this._markedText = this.textArea.value.substring(selectionStartIndex, selectionEndIndex);
            }
        },
        _onFocus: function () {

        },
        onDrop: function (value) {
            this._onTextAreaChanged(value);
        },
        onDropping: function (dndData, source, nodes, copy) {
            //alert('OMG, you are dropping something...');
        },

        _onDropData: function (dndData, source, nodes, copy) {
            //summary:
            //    Handle drop data event.
            //
            // dndData:
            //    Dnd data extracted from the dragging items which have the same data type to the current target
            //
            // source:
            //    The dnd source.  
            //
            // nodes:
            //    The dragging nodes.
            //
            // copy:
            //    Denote that the drag is copy.
            //
            // tags:
            //    private
            var dropItem = dndData ? (dndData.length ? dndData[0] : dndData) : null;

            if (dropItem) {

                this.onDropping();

                dojo.when(dropItem.data, dojo.hitch(this, function (value) {

                    var currentText = this.textArea.value;
                    var versionAgnosticId = value;

                    var page = this._getLinkItemFromStore(versionAgnosticId);

                    if (page) {
                        var newValue = currentText;
                        var linkPattern;
                        if (this._markedText) {
                            //The editor marked text befor dnd
                            linkPattern = ["[", this._markedText, "](", page.url, ")"].join('');
                            newValue = currentText.replace(this._markedText, linkPattern);
                        } else {
                            //The editor just dnd's a page, add pagename as link text
                            linkPattern = ["[", page.text, "](", page.url, ")"].join('');
                            newValue = [currentText.slice(0, this._markerPosition), linkPattern, currentText.slice(this._markerPosition)].join('');
                        }

                        //var newValue = [currentText.slice(0, this._markerPosition), "{" + value + "}", currentText.slice(this._markerPosition)].join('');
                        this.set("value", newValue);
                        this.onDrop(newValue);
                    }
                }));
            }
        },

        openEpicDialog: function () {
            alert("Här skulle man kunna välja något att lägga in i editorn. Men nu går det inte.");
        },

        _getLinkItemFromStore: function (pageId) {
            var page = this._pageDataStore.get(pageId);
            if (page) {
                return { url: page.properties.pageLinkURL, text: page.name };
            }
            return null;
        },

        // Setter for value property
        _setValueAttr: function (value) {
            this._setValue(value, true);
        },

        _setReadOnlyAttr: function (value) {
            this._set("readOnly", value);
            this.textArea.set("readOnly", value);
        },

        // Setter for intermediateChanges
        _setIntermediateChangesAttr: function (value) {
            this.textArea.set("intermediateChanges", value);
            this._set("intermediateChanges", value);
        },

        // Event handler for textarea
        _onTextAreaChanged: function (value) {
            this._setValue(value, false);
        },

        _setValue: function (value, updateTextarea) {
            if (this._started && epi.areEqual(this.value, value)) {
                return;
            }

            // set value to this widget (and notify observers)
            this._set("value", value);

            // set value to textarea
            updateTextarea && this.textArea.set("value", value);

            if (this._started && this.validate()) {
                // Trigger change event
                this.onChange(value);
            }
        },
    });
});