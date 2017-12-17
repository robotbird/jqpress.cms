tinyMCEPopup.requireLangPack();

var InsertCodeDialog = {
    init: function() {
        var s = tinyMCEPopup.editor.selection.getContent({ format: 'text' });
        if (s.trim().length > 0) {
            document.forms[0].txtCode.value = s.trim();
        }
    },

    insert: function() {
        var s1 = '<pre class="brush: ';
        s1 += document.forms[0].ddLang.value;
        s1 += ';">' + Encoder.htmlEncode(document.forms[0].txtCode.value.trim()) + '</pre>';

        tinyMCEPopup.editor.execCommand('mceInsertContent', false, s1);
        tinyMCEPopup.close();
    }
};

String.prototype.trim = function() {
    return this.replace(/^\s*/, "").replace(/\s*$/, "");
}

tinyMCEPopup.onInit.add(InsertCodeDialog.init, InsertCodeDialog);