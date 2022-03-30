;$(document).ready(function () {
    $("#comment").submit(function (event) {
        event.preventDefault();
        var comment = $("#comment");
        var submit = comment.find(":submit");
        submit.attr("disabled", true).text("正在提交，请稍后...");
        $.ajax({
            url: "/User/Comment",
            type: "POST",
            cache: false,
            data: comment.serialize(),
            error: function (error) {
                alert("评论提交失败")
                submit.attr("disabled", false).text("发表评论");
            },
            success: function (msg) {
                console.log(msg)
                if (msg.code === 20000) {
                    var id = comment.find("[name='ArticleId']").val()
                    $.ajax({
                        url: "/Comment?id=" + id,
                        type: "GET",
                        cache: false,
                        success: function (html) {
                            $("#commentDiv").html(html)
                        },
                        error: function (error) {
                            window.location.reload()
                        }
                    })
                }else {
                    alert("评论失败:" + msg.message);
                    submit.attr("disabled", false).text("发表评论");
                }
            }
        })
    })
})