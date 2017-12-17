
document.writeln("<div class=\"float-contact\" id=\"float-contact\" style=\"position: fixed;z-index:1000; right: 1px; display: block;\">");
document.writeln("<a title=\"点击收缩\" href=\"javascript:void(0);\" onclick=\"show()\" class=\"qq-close\" id=\"float-contact-qq-close\">点击收缩</a>");
document.writeln("<div class=\"container2\">");
document.writeln("<div class=\"qq\">");
document.writeln("<div class=\"qqtitle\">在线客服</div>");
document.writeln("<ul class=\"qq-btn\">");
document.writeln("<li><a title=\"业务咨询\" target=\"_blank\" href=\"http://wpa.qq.com/msgrd?v=3&uin=1152160023&site=qq&menu=yes\">业务咨询</a></li>");
document.writeln("<li><a title=\"技术支持\" target=\"_blank\" href=\"http://wpa.qq.com/msgrd?v=3&uin=330296409&site=qq&menu=yes\">技术支持</a></li>");

document.writeln("</ul>");
document.writeln("</div>");
document.writeln("<div class=\"qqtel\">");
document.writeln("<div class=\"qqtitle\">咨询热线</div>");
document.writeln("<div class=\"qqcontent\">010-60701980</div>");
document.writeln("</div>");
document.writeln("</div>");
document.writeln("<a target=\"_blank\" href=\"http://www.bjjxysbz.com/\" class=\"myqqlink\">俊兴包装</a>");
document.writeln("</div>");
document.writeln("<div class=\"float-contact-mini\" id=\"float-contact-mini\" style=\"display: none; position: fixed; right: 1px;\">");
document.writeln("<a href=\"javascript:void(0);\" onclick=\"show()\" id=\"float-contact-mini\">联系我们</a>");
document.writeln("</div>");
function show() {
	var floatContact = document.getElementById('float-contact');
	var floatContactMini = document.getElementById('float-contact-mini');
	if(floatContact.style.display=="none") {
		floatContact.style.display="block";
		floatContactMini.style.display="none";
	}else {
		floatContact.style.display="none";
		floatContactMini.style.display="block";
	}
}