(() => {
    function setSubmitState(submitButton, disabled, text) {
        if (!submitButton) return;
        submitButton.disabled = disabled;
        submitButton.textContent = text;
    }

    async function loadCommentList(form) {
        const articleIdInput = form.querySelector("[name='ArticleId']");
        const articleId = articleIdInput?.value;
        if (!articleId) {
            window.location.reload();
            return;
        }

        const response = await fetch(`/Comment?id=${encodeURIComponent(articleId)}`, {
            method: "GET",
            cache: "no-store"
        });
        if (!response.ok) throw new Error("load-comment-failed");

        const html = await response.text();
        const commentDiv = document.getElementById("commentDiv");
        if (!commentDiv) {
            window.location.reload();
            return;
        }

        commentDiv.innerHTML = html;
    }

    async function submitComment(event) {
        event.preventDefault();

        const form = event.currentTarget;
        const submitButton = form.querySelector("[type='submit']");
        setSubmitState(submitButton, true, "正在提交，请稍后...");

        try {
            const formData = new FormData(form);
            const payload = new URLSearchParams(formData);
            const response = await fetch("/User/Comment", {
                method: "POST",
                headers: {
                    "Content-Type": "application/x-www-form-urlencoded; charset=UTF-8",
                    "X-Requested-With": "XMLHttpRequest"
                },
                body: payload.toString()
            });

            if (!response.ok) throw new Error("submit-comment-failed");
            const msg = await response.json();

            if (msg?.code === 20000) {
                await loadCommentList(form);
                return;
            }

            alert(`评论失败: ${msg?.message ?? "未知错误"}`);
            setSubmitState(submitButton, false, "提交评论");
        } catch (error) {
            alert("评论提交失败");
            setSubmitState(submitButton, false, "提交评论");
        }
    }

    document.addEventListener("DOMContentLoaded", () => {
        const form = document.getElementById("comment");
        if (!form) return;
        form.addEventListener("submit", submitComment);
    });
})();
