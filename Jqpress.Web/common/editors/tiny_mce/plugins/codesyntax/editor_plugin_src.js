 * @author Jacobus Meintjes
 * @copyright Copyright © 2010, PhoenixCode, All rights reserved.
 
 (function()
 {	
    tinymce.create('tinymce.plugins.SyntaxHighlighter', {
        createControl: function(n, cm) {
            switch (n) {
                case 'codesyntax':
                    var mlb = cm.createListBox('codesyntax', {
                        title: 'Format Code',
                        width: '400',
                        onselect: function(v) {
                            var content = new String(tinyMCE.activeEditor.selection.getContent());
                            tinyMCE.activeEditor.selection.setContent('<pre name="code" class="c-sharp">' + content + '</pre>');
                        }
                    });

                    // Add some values to the list box
                    mlb.add('C#', 'csharp');
                    mlb.add('Html', 'xhtml');
                    mlb.add('Xml/Xsl', 'xml');
                    mlb.add('VB.Net', 'vb.net');
                    mlb.add('SQL', 'sql');
                    mlb.add('CSS', 'css');
                    mlb.add('JavaScript', 'javascript');

                    // Return the new listbox instance
                    return mlb;
            }
            return null;
        },
		
		init: function(){
			alert('Init');
		}
    });

    // Register plugin with a short name
    tinymce.PluginManager.add('syntaxhighlighter', tinymce.plugins.SyntaxHighlighterPlugin);
 })();	