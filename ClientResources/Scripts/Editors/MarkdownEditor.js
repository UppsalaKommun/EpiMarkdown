define([
    "dojo/_base/connect",
    "dojo/_base/declare",
    "dojo/_base/lang",

    "dijit/_CssStateMixin",
    "dijit/_Widget",
    "dijit/_TemplatedMixin",
    "dijit/_WidgetsInTemplateMixin",
    "dijit/form/Textarea",

    "epi/epi",
    "epi/shell/widget/_ValueRequiredMixin"
],
function (
    connect,
    declare,
    lang,

    _CssStateMixin,
    _Widget,
    _TemplatedMixin,
    _WidgetsInTemplateMixin,
    Textarea,

    epi,
    _ValueRequiredMixin
) {

    return declare("uk.editors.MarkdownEditor", [_Widget, _TemplatedMixin, _WidgetsInTemplateMixin, _CssStateMixin, _ValueRequiredMixin], {

        templateString: "\
            <div class=\"dijitInline\">\
                <div data-dojo-attach-point=\"stateNode, tooltipNode\">\
                    <div data-dojo-attach-point=\"textArea\" data-dojo-type=\"dijit.form.Textarea\" style=\"width: 350px\"></div>\
                </div>\
                <br />\
                <span>${helptext}</span>\
            </div>",

        helptext: "Markdown goes here!",

        intermediateChanges: false,

        value: null,
        
        markdownAsHtml: null,

        multiple: true,

        onChange: function (value) {
        },

        postCreate: function () {
            this.inherited(arguments);

            this.textArea.set("intermediateChanges", this.intermediateChanges);
            this.connect(this.textArea, "onChange", this._onTextAreaChanged);
        },

        isValid: function () {
            return true;
        },

        _setValueAttr: function (value) {
            this._setValue(value, true);
        },

        _setReadOnlyAttr: function (value) {
            this._set("readOnly", value);
            this.textArea.set("readOnly", value);
        },

        _setIntermediateChangesAttr: function (value) {
            this.textArea.set("intermediateChanges", value);
            this._set("intermediateChanges", value);
        },

        _onTextAreaChanged: function (value) {
            this._setValue(value, false);
        },

        _setValue: function (value, updateTextarea) {
            if (this._started && epi.areEqual(this.value, value)) {
                return;
            }

            this._set("value", value);

            updateTextarea && this.textArea.set("value", value);

            if (this._started && this.validate()) {
                this.onChange(value);
            }
        },
    });
});