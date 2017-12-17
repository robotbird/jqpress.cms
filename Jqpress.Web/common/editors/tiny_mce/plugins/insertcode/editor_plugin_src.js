
(function() {
	// Load plugin specific language pack
	tinymce.PluginManager.requireLangPack('insertcode');

	tinymce.create('tinymce.plugins.InsertCode', {

		init : function(ed, url) {
			ed.addCommand('mceInsertCode', function() {
				ed.windowManager.open({
					file : url + '/dialog.htm?v=2',
					width : 480 + parseInt(ed.getLang('insertcode.delta_width', 0)),
					height : 320 + parseInt(ed.getLang('insertcode.delta_height', 0)),
					inline : 1
				}, {
					plugin_url : url, // Plugin absolute URL
					some_custom_arg : 'custom arg' // Custom argument
				});
			});

			// Register example button
			ed.addButton('insertcode', {
			    title: 'Insert Code',
				cmd : 'mceInsertCode',
				image : url + '/img/code2.gif'
			});

			// Add a node change handler, selects the button in the UI when a image is selected
			ed.onNodeChange.add(function(ed, cm, n) {
			    cm.setActive('insertcode', n.nodeName == 'IMG');
			});
		},

		createControl : function(n, cm) {
			return null;
		},

		getInfo : function() {
			return {
				longname : 'Insert Code plugin',
				author : 'Rtur.net',
				authorurl : 'http://rtur.net',
				infourl: 'http://rtur.net/blog/post/2009/11/26/Building-custom-plugin-for-TinyMCE.aspx',
				version : "1.0"
			};
		}
	});

	// Register plugin
	tinymce.PluginManager.add('insertcode', tinymce.plugins.InsertCode);
})();