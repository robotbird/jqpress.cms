String.prototype.trim = function(){  
return this.replace(/(^\s*)|(\s*$)/g, "");  
}  

function checkComment(){
    var author=$("#commentauthor").val().trim();
    var email=$("#commentemail").val().trim();
    var siteurl=$("#commentsiteurl").val().trim();
    var content=$("#commentcontent").val().trim();
    
    var enableverifycode = $("#enableverifycode").val();
    var verifycode='';
    if (enableverifycode == 1) {
        verifycode=$("#commentverifycode").val().trim();
    }
   
    var r='';
    if(author==""){
        r=" 名称";
    }
    if(email==""){
        r+=" 邮箱";
    }
    if(content==""){
        r+=" 内容";
    }
    if (enableverifycode == 1) {
        if(verifycode==""){
            r+=" 验证码";
        }
    }
 
    
    if(r.length>0){
        $("#commentmessage").html('<div>请输入:' + r + '</div>');
        return false;
    } 
    return true;
}

function doComment() {
    var Content = $.trim($("#txtContent").val());
    Content = Content.replace(/(\n){3,}/ig, '\n\n');
    var strComment = $("#Comment_new").html();
    var parentCommentId = $("#ReplyToCommentId").html();
    var title = $("#news_title").html();

    if (Content == "") {
        alert("评论不能为空！");
        return;
    }
    else if (Content.length < 3) {
        alert('回复的字数太少了，至少3个字吧！');
        return;
    }
    $("#btnComment").val("提交中...").attr("disabled", "disabled");
    $("#Comment_new").html("评论提交中...").css("color", "red").css("margin-left", "20px");
    if ($("#Comment_Edit_ID").val() == null || $("#Comment_Edit_ID").val() == "") {
        var insertComment = {};
        insertComment.ContentID = ContentID;
        insertComment.Content = Content;
        insertComment.strComment = strComment;
        insertComment.parentCommentId = parentCommentId;
        insertComment.title = title;
        $.ajax({
            url: '/common/ajax/comment.ashx',
            data: JSON.stringify(insertComment),
            type: 'post',
            dataType: 'text',
            contentType: 'application/json; charset=utf8',
            success: function (data) {
                comment_callback(data);
            },
            error: function (data) {
                alert("出现异常：" + data);
            }
        });
    }
    else {
        var updateComment = {};
        updateComment.ContentID = ContentID;
        updateComment.CommentID = $("#Comment_Edit_ID").val();
        updateComment.Content = Content;
        $.ajax({
            url: '/mvcajax/news/UpdateComment',
            data: JSON.stringify(updateComment),
            type: 'post',
            dataType: 'text',
            contentType: 'application/json; charset=utf8',
            success: function (data) {
                comment_edit_callback(data);
                $("#Comment_Edit_ID").val("");
                $("#btnComment").val("提交评论");
            },
            error: function (data) {
                alert("出现异常："+data);
            }
        });
    }
}

function GetNewsComment() {
    $.ajax({
        url: '/mvcajax/news/GetComments',
        data: '{"nid":' + ContentID + '}',
        type: 'post',
        dataType: 'text',
        contentType: 'application/json; charset=utf8',
        beforeSend: function(){
            $("#comment_main_list").html("努力加载评论中...");
        },
        success: function (data) {
            $(".user_comment").remove();
            $("#Comment_new").empty();
            $("#comment_main_list").html(data);
        },
        error: function () {
            $("#comment_main_list").html("不好意思！评论加载失败！");
        }
    });
}

function loadCommentForm() {
    $.ajax({
        url: '/mvcajax/news/LoadCommentForm',
        data: '{}',
        type: 'post',
        dataType: 'text',
        contentType: 'application/json; charset=utf8',
        success: function (data) {
            $("#comment_form_block").html(data);
        },
        error: function (data) {
            $("#comment_form_block").html("出现异常：" + data);
        }
    }); 
}

function GetAdminLink() {
    $.ajax({
        url: '/mvcajax/news/AdminLink',
        data: '{"ContentID":' + ContentID + '}',
        type: 'post',
        dataType: 'text',
        contentType: 'application/json; charset=utf8',
        success: function (data) {
            $("#side_right_search").after(data);
        }
    });
}

function QueryString(fieldName) {
    var urlString = document.location.search;
    if (urlString != null) {
        var typeQu = fieldName + "=";
        var urlEnd = urlString.indexOf(typeQu);
        if (urlEnd != -1) {
            var paramsUrl = urlString.substring(urlEnd + typeQu.length);
            var isEnd = paramsUrl.indexOf('&');
            if (isEnd != -1) {
                return paramsUrl.substring(0, isEnd);
            }
            else {
                return paramsUrl;
            }
        }
        else {
            return null;
        }
    }
    else {
        return null;
    }
}

function NewsLoad() {
    if (QueryString("listorder") != null) {
        document.getElementById("selectCommentOrder").options[QueryString("listorder")].selected = true;
    }
}

function ctlent2(eventobject) {
    if (eventobject.ctrlKey && eventobject.keyCode == 13) {

        doComment();
    }
}

function comment_callback(response) {
    $("#Comment_new").html(response.replace(/\n/g, '<br/>'));
    if (response.indexOf("<span style=\"color:red\">") < 0) {
        $("#txtContent").val("");
    }
    if ($("#com_count").length > 0) {
        var com_count = parseInt($.trim($("#com_count").text()), 10);
        $("#com_count").text(com_count + 1);
    }
    $("#ReplyToCommentId").html('0');
    comment_btn_reset();
}

function comment_edit_callback(response) {
    $("#Comment_new").html(response);
    $("#txtContent").val("");
    comment_btn_reset();
}

function comment_btn_reset() {
    $("#btnComment").removeAttr("disabled");
    $("#btnComment").val("提交评论");
}

function DiggNews(newsid) {
    $.ajax({
        url: '/mvcajax/news/DiggNews',
        data: '{"NewsID":'+newsid+'}',
        type: 'post',
        dataType: 'text',
        contentType: 'application/json; charset=utf8',
        success: function (data) {
            DiggNews_CallBack(data);
        },
        error: function (data) {
            alert("出现异常：" + data);
        }
    });
}

function DiggNews_CallBack(response) {
    if (response == "-1") {
        alert("您已经推荐过该文章！");
    }
    else if (response == "-2") {
        alert("您已经反对过该文章！");
    }
    else {
        var ret = response.split(",");
        $("#digg_num_" + ret[0]).html(ret[1]);
    }
}


function BuryNews(newsid) {
    $.ajax({
        url: '/mvcajax/news/BuryNews',
        data: '{"NewsID":' + newsid + '}',
        type: 'post',
        dataType: 'text',
        contentType: 'application/json; charset=utf8',
        success: function (data) {
            BuryNews_CallBack(data);
        },
        error: function (data) {
            alert("出现异常：" + data);
        }
    });
}

function BuryNews_CallBack(response) {
    if (response == "-1") {
        alert("您已经推荐过该文章！");
    }
    else if (response == "-2") {
        alert("您已经反对过该文章！");
    }
    else {
        var ret = response.split(",");
        $("#bury_num_" + ret[0]).html(ret[1]);
    }
}

function ReplyVoteInComment(id, action) {
    news_Comment.PageReplyVote(id, action, ReplyVote_callback);
}

function ReplyVote(id, action) {
    $.ajax({
        url: '/mvcajax/news/PageReplyVote',
        data: '{"CommentID":' + id + ',"Action":"' + action + '"}',
        type: 'post',
        dataType: 'text',
        contentType: 'application/json; charset=utf8',
        success: function (data) {
            ReplyVote_callback(data);
        },
        error: function (msg) {
            alert("出现异常：" + msg);
        }
    });
}

function ReplyVote_callback(response) {
    var ret = response.split(",");
    var id = ret[0];
    var action = ret[1];
    var vCount = ret[2];
    if (action == "agree") {
        if (vCount == 0) {
            $("#tips_" + id).html("<div class=WarningTips>你已经投过票了！</div>");
        } else {
            $("#agree_" + id).html(vCount);
            $("#tips_" + id).html("<div class=CompleteTips>谢谢你的参与！</div>");
        }

    } else if (action == "anti") {
        if (vCount == 0) {
            $("#tips_" + id).html("<div class=WarningTips>你已经投过票了！</div>");
        } else {
            $("#anti_" + id).html(vCount);
            $("#tips_" + id).html("<div class=CompleteTips>谢谢你的参与！</div>");
        }
    } else if (action == "report") {
        if (vCount == 0)
            $("#tips_" + id).html("<div class=WarningTips>感谢你的举报！</div>");
        else
            $("#tips_" + id).html("<div class=CompleteTips>感谢你的举报，我们会尽快处理！</div>");

    }
    setTimeout("HideMsg(" + id + ");", 3000);
}

function HideMsg(id) {
    $("#tips_" + id).html("");
}

function HotReplyVote(id, action) {
    PageClass.PageReplyVote(id, action, HotReplyVote_callback);
}

function HotReplyVote_callback(response) {
    var ret = response.value.split(",");
    var id = ret[0];
    var action = ret[1];
    var vCount = ret[2];
    if (action == "agree") {
        if (vCount == 0) {
            $("#hot_tips_" + id).html("<div class=WarningTips>你已经投过票了！</div>");
        } else {
            $("#hot_agree_" + id).html(vCount);
            $("#hot_tips_" + id).html("<div class=CompleteTips>谢谢你的参与！</div>");
        }

    } else if (action == "anti") {
        if (vCount == 0) {
            $("#hot_tips_" + id).html("<div class=WarningTips>你已经投过票了！</div>");
        } else {
            $("#hot_anti_" + id).html(vCount);
            $("#hot_tips_" + id).html("<div class=CompleteTips>谢谢你的参与！</div>");
        }
    } else if (action == "report") {
        if (vCount == 0)
            $("#hot_tips_" + id).html("<div class=WarningTips>感谢你的举报！</div>");
        else
            $("#hot_tips_" + id).html("<div class=CompleteTips>感谢你的举报，我们会尽快处理！</div>");

    }
    setTimeout("HideMsg2(" + id + ");", 3000);
}

function HideMsg2(id) {
    $("#hot_tips_" + id).html("");
}




function DelComment(id) {
    var con = confirm('你确认删除该评论么？');
    if (con) {
        $.ajax({
            url: '/mvcajax/news/DelComment',
            data: '{"CodeID":"' + id + '","ContentID":' + ContentID + '}',
            type: 'post',
            dataType: 'text',
            contentType: 'application/json; charset=utf8',
            success: function (data) {
                $("#span_" + data).css("display", "none");
                if ($("#com_count").length > 0) {
                    var com_count = parseInt($.trim($("#com_count").text()), 10);
                    $("#com_count").text(com_count > 0 ? com_count - 1 : 0);
                }
            },
            error: function (msg) {
                alert("出现异常：" + msg);
            }
        });
    }
    return;
}

function SetAuthor(commentId) {
    $("#txtContent").removeClass('txtContent_bg');
    $("#ReplyToCommentId").html(commentId);
    var author = $("#comment_author_" + commentId).html();
    document.getElementById("txtContent").focus();
    document.getElementById("txtContent").value = "@" + author + "\n" + document.getElementById("txtContent").value;
    return false;
}

function QuoteComment(commentId) {
    SetAuthor(commentId);
    var content = $("#comment_body_" + commentId).html();
    content = content.replace(/<br>|<br\/>/ig, '\n');
    content = content.replace(/<fieldset\sclass=\"comment_quote\"><legend>引用<\/legend>/ig, '[quote]');
    content = content.replace(/<\/fieldset>/ig,'[/quote]');
    content = content.replace(/<[^>]*>/g, '');
    content = jQuery.trim(content);
    if (content.length > 200) {
        content = content.substring(0, 200) + "...";
    }
    $("#txtContent").focus();
    var text = $("#txtContent").val();
    text = text.replace(/^\n/g, '');
    $("#txtContent").val(text + "[quote]" + content + "[/quote]" + "\n");
}

function SetCommentEdit(commentID) {
    $("#Comment_Edit_ID").val(commentID);
    $.ajax({
        url: '/mvcajax/news/GetComment',
        data: '{"CodeID":"' + commentID + '"}',
        type: 'post',
        dataType: 'text',
        contentType: 'application/json; charset=utf8',
        success: function (data) {
            $("#txtContent").val(data);
            $("#txtContent").focus();
            $("#btnComment").val("修改评论");
        },
        error: function (msg) {
            alert("出现异常：" + msg);
        }
    });
}

function recommend(ContentID) {
    $.ajax({
        url: '/service/NewsService.asmx/SetRecommend',
        data: '{contentId:"' + ContentID + '"}',
        type: 'post',
        dataType: 'json',
        contentType: 'application/json; charset=utf-8',
        cache: false,
        success: function (data) {
            if (data.d) {
                $("#opt_recommend").css("color", "red");
                $("#opt_recommend").html("推荐成功");
            }
            else {
                $("#opt_recommend").css("color", "red");
                $("#opt_recommend").html("推荐失败");
            }
        },
        error: function (xhr) {
            alert(xhr.responseText);
        }
    });
}

function cancel_recommend(ContentID) {
    $.ajax({
        url: '/service/NewsService.asmx/CancelRecommend',
        data: '{contentId:"' + ContentID + '"}',
        type: 'post',
        dataType: 'json',
        contentType: 'application/json; charset=utf-8',
        cache: false,
        success: function (data) {
            if (data.d) {
                $("#opt_recommend").css("color", "red");
                $("#opt_recommend").html("取消推荐成功");
            }
            else {
                $("#opt_recommend").css("color", "red");
                $("#opt_recommend").html("取消推荐失败");
            }
        },
        error: function (xhr) {
            alert(xhr.responseText);
        }
    });
}

function hide_news(ContentID) {
    $.ajax({
        url: '/mvcajax/news/HideNews',
        data: '{"ContentID":' + ContentID + '}',
        type: 'post',
        dataType: 'text',
        contentType: 'application/json; charset=utf8',
        success: function (data) {
            location.reload();
        },
        error: function (msg) {
            alert("出现异常：" + msg);
        }
    });
}

function publish_news(ContentID) {
    $.ajax({
        url: '/mvcajax/news/SetPublishStatus',
        data: '{"ContentID":' + ContentID + '}',
        type: 'post',
        dataType: 'text',
        contentType: 'application/json; charset=utf8',
        success: function (data) {
            location.reload();
        },
        error: function (msg) {
            alert("出现异常：" + msg);
        }
    });
}

function PutInWz() {
    var width = 460;
    var height = 353;
    var leftVal = (screen.width - width) / 2;
    var topVal = (screen.height - height) / 2;
    var d = document;
    var t = d.selection ? (d.selection.type != 'None' ? d.selection.createRange().text : '') : (d.getSelection ? d.getSelection() : '');
    if (t.length > 200) {
        t = t.substring(0, 200) + "...";
    }
    window.open('http://wz.cnblogs.com/create?t=' + encodeURIComponent(d.title) + '&u=' + encodeURIComponent(d.location.href) + '&c=' +
	     encodeURIComponent(t) + '&i=0', '_blank', 'width=' + width + ',height=' + height + ',toolbars=0,resizable=1,left=' + leftVal + ',top=' + topVal);
}

function go_link(url) {
    window.location = url;
}

function LogShareClick(clickName) {
    PageClass.LogShareClick(clickName);
}

function ToTsina() {
    var title = $("#news_title a").html();
    var url = "http://news.cnblogs.com/n/" + ContentID + "/";
    var t_news = "#IT新闻# " + title + "，" + url;
    $.ajax({
        url: '/service/NewsService.asmx/ToTsina',
        data: "{content:'" + t_news + "'}",
        type: 'post',
        dataType: 'json',
        contentType: 'application/json; charset=utf-8',
        cache: false,
        success: function (data) {
            if (data.d) {
                $("#opt_tsina").css("color", "red");
                $("#opt_tsina").html("发布成功");
            }
            else {
                $("#opt_tsina").css("color", "red");
                $("#opt_tsina").html("发布失败");
            }
        },
        error: function (xhr) {
            alert(xhr.responseText);
        }
    });
}



function audit(newsId, auditType) {
    ajaxRequest.url = "/service/NewsService.asmx/AuditById";
    ajaxRequest.data = '{newsId:' + newsId + ',auditType:' + auditType + '}';
    ajaxRequest.success = function (data) {
        $("#span_audit_" + auditType).css("color", "red");
        $("#span_audit_" + auditType).html($("#lnk_audit_" + auditType).html() + "成功");
    };
    $.ajax(ajaxRequest);
}

function GetRecommendJobList() {
    $.ajax({
        url: '/service/NewsService.asmx/GetRecommendJobList',
        data: '{}',
        type: 'post',
        dataType: 'json',
        contentType: 'application/json; charset=utf-8',
        cache: false,
        success: function (data) {
            if (data.d) {
                $("#job_recommend_list").html(data.d);
                $("#job_recommend").show();
            }
        }
    });
}

function log() {
    var id = ContentID;
    $.ajax({
        url: '/service/NewsService.asmx/UpdateViewCount',
        data: '{"id":' + id + '}',
        type: 'post',
        dataType: 'json',
        contentType: 'application/json; charset=utf-8',
        cache: false,
        success: function (data) {
        }
    });
}

function google_cse_load() {
    var cses = document.createElement('script');
    cses.type = 'text/javascript';
    cses.src = 'http://www.google.com.hk/coop/cse/brand?form=cse-search-box&amp;lang=zh-Hans';
    var node = document.getElementById('google_cse_holder');
    node.parentNode.insertBefore(cses, node);
}