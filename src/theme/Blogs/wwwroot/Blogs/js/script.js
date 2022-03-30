//点赞处理
function Blogs_prise(post_id,user_id){
	var $prise=$("#Blogs_prise_id-"+post_id);
	if($prise.hasClass('prised')){
		$prise.addClass('am-animation-scale-down');
		alert($Blogszanalert);
		return
		}
	if(post_id){
		$prise.addClass('am-animation-scale-down');
		var ajax_data={post_id:post_id,user_id:user_id};
		jQuery.post(
			ajaxurl+"Blogs_prise",
			ajax_data,
			function(result){
				if(result.status==200){
					var $count=$prise.find('span');
					$prise.addClass('prised').removeClass('am-animation-scale-down');
					$count.text(result.count);
					}else{
						alert($Blogszanalert);
						}
			},
			'json'
		)
	}
}
$(document).ready(function() {
	var s = document.location;
	$("#divNavBar-main a").each(function() {
		if (this.href == s.toString().split("#")[0]) {
			$(this).addClass("current-menu-item");
			return false;
		}
	});
});

$(document).ready(function(a){a(".collapseButton").click(function(){a(this).parent().parent().find(".xContent").slideToggle("fast")})})
$(document).ready(function(){
//图片自动链接
 $('.single-content img').each(function(i){
 if (! this.parentNode.href){
 $(this).wrap("<a href=\""+this.src+"\" data-fancybox=\"images\" class=\"cboxElement\" rel=\"example_group\"></a>");
 }
 });	
// 滚屏
$('.tools_top').click(function () {
    $('html,body').animate({
        scrollTop: '0px'
    }, 800);
});
$('.tools_comments').click(function () {
    $('html,body').animate({
        scrollTop: $('.commentszbp-area').offset().top
    }, 800);
});
		
// 搜索
$(".nav-search").click(function(){
	$("#main-search").slideToggle(500);
});
	
// 侧边栏文章切换
    $('#top_post').find('.top_post_item').hide();
	$('#top_post').find('.' + $('#top_post_filter').find('li').eq(0).attr('id')).show();
    $('#top_post_filter').on('mouseover', 'li', function(){
		$('#top_post_filter').find('li').removeClass('top_post_filter_active');
		$(this).addClass('top_post_filter_active');
		$('#top_post').find('a').hide();
		$('#top_post').find('.' + $(this).attr('id')).show();
    })
	
// 最新文章
	$(".clr").mouseover(function () {
        $(this).addClass('hov');
        }).mouseleave(function () {
            $(this).removeClass('hov');
    });
	
// 去边线
$(".message-widget li:last, .message-page li:last, .hot_commend li:last, .random-page li:last, .search-page li:last, .my-comment li:last").css("border","none");

// 表情
$('.smiley').click(function () {
	$('.smiley-box').animate({
		opacity: 'toggle',
		left: '50px'
	}, 1000).animate({
		left: '10px'
	}, 'fast');
	return false;
});

// 图片数量
var i = $('#gallery img').size();
$('.myimg').html(' ' + i + ' 张图片');

});
// 文字滚动
(function($){$.fn.textSlider=function(settings){settings=jQuery.extend({speed:"normal",line:2,timer:1000},settings);return this.each(function(){$.fn.textSlider.scllor($(this),settings)})};$.fn.textSlider.scllor=function($this,settings){var ul=$("ul:eq(0)",$this);var timerID;var li=ul.children();var _btnUp=$(".up:eq(0)",$this);var _btnDown=$(".down:eq(0)",$this);var liHight=$(li[0]).height();var upHeight=0-settings.line*liHight;var scrollUp=function(){_btnUp.unbind("click",scrollUp);ul.animate({marginTop:upHeight},settings.speed,function(){for(i=0;i<settings.line;i++){ul.find("li:first").appendTo(ul)}ul.css({marginTop:0});_btnUp.bind("click",scrollUp)})};var scrollDown=function(){_btnDown.unbind("click",scrollDown);ul.css({marginTop:upHeight});for(i=0;i<settings.line;i++){ul.find("li:last").prependTo(ul)}ul.animate({marginTop:0},settings.speed,function(){_btnDown.bind("click",scrollDown)})};var autoPlay=function(){timerID=window.setInterval(scrollUp,settings.timer)};var autoStop=function(){window.clearInterval(timerID)};ul.hover(autoStop,autoPlay).mouseout();_btnUp.css("cursor","pointer").click(scrollUp);_btnUp.hover(autoStop,autoPlay);_btnDown.css("cursor","pointer").click(scrollDown);_btnDown.hover(autoStop,autoPlay)}})(jQuery);

// 表情
function grin(a){var d;a=" "+a+" ";if(document.getElementById("txaArticle")&&document.getElementById("txaArticle").type=="textarea"){d=document.getElementById("txaArticle")}else{return false}if(document.selection){d.focus();sel=document.selection.createRange();sel.text=a;d.focus()}else{if(d.selectionStart||d.selectionStart=="0"){var c=d.selectionStart;var b=d.selectionEnd;var e=b;d.value=d.value.substring(0,c)+a+d.value.substring(b,d.value.length);e+=a.length;d.focus();d.selectionStart=e;d.selectionEnd=e}else{d.value+=a;d.focus()}}};

// 弹窗
(function(a){a.fn.extend({leanModal:function(d){var e={top:100,overlay:0.5,closeButton:null};var c=a("<div id='overlay'></div>");a("body").append(c);d=a.extend(e,d);return this.each(function(){var f=d;a(this).click(function(j){var i=a(this).attr("href");a("#overlay").click(function(){b(i)});a(f.closeButton).click(function(){b(i)});var h=a(i).outerHeight();var g=a(i).outerWidth();a("#overlay").css({"display":"block",opacity:0});a("#overlay").fadeTo(200,f.overlay);a(i).css({"display":"block","position":"fixed","opacity":0,"z-index":11000,"left":50+"%","margin-left":-(g/2)+"px","top":f.top+"px"});a(i).fadeTo(200,1);j.preventDefault()})});function b(f){a("#overlay").fadeOut(200);a(f).css({"display":"none"})}}})})(jQuery);