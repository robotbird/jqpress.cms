/*
* 通用jquery ajax 评论 
* author   yepeng  20120327
*/
var PressComment;
if (!PressComment) {

    String.prototype.trim = function(){  
       return this.replace(/(^\s*)|(\s*$)/g, "");  
    } 
    PressComment = {
 
     //检查评论
     checkComment:function (){
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
            if(author==""){ r=" 名称";}
            if(email==""){r+=" 邮箱"; }
            if(content==""){r+=" 内容";}
            if (enableverifycode == 1) {if(verifycode==""){ r+=" 验证码"; } }
            
            if(r.length>0){ alert('请输入:' + r + ''); return false; } 
            return true;
        },//end
        
        //评论方法
        doComment:function () {
            this.checkComment();
           
            var Content = $.trim($("#commentcontent").val());
            Content = Content.replace(/(\n){3,}/ig, '\n\n');
            var strComment = $("#Comment_new").html();
            var parentCommentId = $("#ReplyToCommentId").val();
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
                //insertComment.ContentID = ContentID;
                insertComment.Content = Content;
                insertComment.strComment = "";
                insertComment.parentCommentId = 0;
                insertComment.title = "";
                $.ajax({
                    url: '/common/ajax/comment.ashx?act=save',
                    data: $('#commentform').serialize(),
                    type: 'post',
                    //dataType: 'text',
                    //contentType: 'application/json; charset=utf8',
                    success: function (data) {
                        PressComment.comment_callback(data);
                    },
                    error: function (data) {
                        alert("出现异常：" + data);
                    }
                });
            }
            else {
                var updateComment = {};
                updateComment.ContentID = 0;
               // updateComment.CommentID = $("#Comment_Edit_ID").val();
               updateComment.CommentID = 0;
               updateComment.Content = Content;
                $.ajax({
                    url: '/common/ajax/comment.ashx?act=update',
                    data: $('#commentform').serialize(),
                    type: 'post',
                    dataType: 'text',
                    contentType: 'application/json; charset=utf8',
                    success: function (data) {
                        this.comment_edit_callback(data);
                        $("#Comment_Edit_ID").val("");
                        $("#btnComment").val("提交评论");
                    },
                    error: function (data) {
                        alert("出现异常："+data);
                    }
                });
            }
        },//---
        
        //回复评论
        replyAuthor:function(author,commentid){
         $('#commentcontent').html("@"+author+"\r\n");
         $("#ReplyToCommentId").val(commentid);
        },//---
        //加载评论
        loadComment:function () {
            $.ajax({
                url: '/common/ajax/comment.ashx?act=load',
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
        },//---
        
       //-- Ctrl+Enter键快速提交
       ctlent2: function (eventobject) {
            if (eventobject.ctrlKey && eventobject.keyCode == 13) {
                this.doComment();
            }
        },//--
      
      //评论后回调函数
      comment_callback: function (response) {
            $(".comment_list").append(response.replace(/\n/g, '<br/>'));

            if ($("#com_count").length > 0) {
                var com_count = parseInt($.trim($("#com_count").text()), 10);
                $("#com_count").text(com_count + 1);
            }
            $("#ReplyToCommentId").html('0');
            this.comment_btn_reset();
        },//---
       
       //修改评论
       comment_edit_callback: function (response) {
            $("#Comment_new").html(response);
            $("#txtContent").val("");
            this.comment_btn_reset();
        },//---
        
       //重置提交按钮
       comment_btn_reset:function () {
            $("#btnComment").removeAttr("disabled");
            $("#btnComment").val("提交评论");
        }//---



    }//--

} //-





