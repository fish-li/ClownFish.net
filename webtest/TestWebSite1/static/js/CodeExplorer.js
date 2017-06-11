
$(window).resize(function(){
	var height = $(window).height() - $("#topBanner").height() -2;
	$("#divExplorerContainer").height(height).width( $(window).width() );
	$("#divExplorer").layout("resize");
}); 

$(function(){
	if( window.SyntaxHighlighter )  {
		SyntaxHighlighter.defaults['toolbar'] = false;
		SyntaxHighlighter.defaults['auto-links'] = false;
	}
		
	$("#divBottomBlank").remove();
	$("#contentBody").css("padding", "0px");
	$("#topBanner").css("border-bottom", "0px");
	$(window).resize();
	
	var root = GetUrlParams()["root"];
	if( typeof(root) != "string" ) root = "";
	
	$('#fileFolderTree').tree({
		url: '/api/ns/FileTree/GetWebSiteFileNodes.aspx?root=' + root,
		onClick:function(node){
			if( node.attributes.FilePath == "###" )
				return;

			$.ajax({
				url: "/api/ns/FileTree/GetFileText.aspx",  type: "GET", dataType: "text",
				data: {path: node.attributes.FilePath },
				success: function(responseText) {
					var pre = "";
					switch(node.attributes.FileType){
						case ".cs":
						case ".ashx":
						case ".asax":
							pre = "<pre class='brush: c#;' name='code'>"; break;
						case ".css":
							pre = "<pre class='brush: css;' name='code'>"; break;
						case ".js":
							pre = "<pre class='brush: js;' name='code'>"; break;
						case ".sql":
							pre = "<pre class='brush: sql;' name='code'>"; break;
						case ".aspx":
						case ".ascx":
						case ".master":
						case ".config":
						case ".skin":
						case ".htm":
						case ".html":
							pre = "<pre class='brush: xml;' name='code'>"; break;
						default:
							pre = "<pre>";
					}
						
					var html = pre + responseText.replace(/</g, "&lt;") + "</pre>";
					$("#divFileBody").html(html).scrollTop(0).scrollLeft(0);
					SyntaxHighlighter.highlight();
					$("#divFileBody").panel({title: node.text});
				}
			});
		}
	});
});



function GetUrlParams() {
	var url = location.search; //获取url中"?"符后的字串
	var urlParams = new Object();
	if(url.indexOf("?") != -1) {
		var str = url.substr(1);
		strs = str.split("&");
		for(var i = 0; i < strs.length; i ++)
			//urlParams[strs[i].split("=")[0]] = decodeURIComponent(strs[i].split("=")[1]);
			urlParams[strs[i].split("=")[0]] = strs[i].split("=")[1];
	}
	return urlParams;
}
