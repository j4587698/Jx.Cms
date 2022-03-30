function blogs_comment(id) {
	var i = id;
	
	var frm = $('#divCommentPost'),
		cancel = $("#cancel-reply");
	if ($("#inpRevID").val == 0){
		frm.before($("<div id='temp-frm' style='display:none'>")).addClass("reply-frm");
	}
	$("#inpRevID").val(i);
	$('#AjaxComment' + i).before(frm);

	cancel.show().click(function () {
		var temp = $('#temp-frm');
		$("#inpRevID").val(0);
		if (!temp.length || !frm.length) return;
		temp.before(frm);
		temp.remove();
		$(this).hide();
		frm.removeClass("reply-frm");
		return false;
	});
	try {
		$('#txaArticle').focus();
	} catch (e) { }
	console.log('out')
	return false;
}