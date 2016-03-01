

$(function(){
	$("a.btnSetStyle").click(function(){
		var style = $(this).attr("ps");
		
		// 其实写cookie也可以直接使用JS去写的。
		$.ajax({
			url: "/ajax/ns/Style/SetStyle.aspx",
			data: {style: style},
			success: function(){
				window.location = window.location;
			}
		});
		return false;
	});
});

