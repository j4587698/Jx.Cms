(function () {
    let hasRun = false;

    function markInlineCode(enableInlineCode) {
        if (!enableInlineCode) return;

        document.querySelectorAll("code:not(pre code)").forEach(function (codeElement) {
            codeElement.classList.add("jx-inline-hl");
            if (!codeElement.classList.contains("hljs")) {
                codeElement.classList.add("hljs");
            }
        });
    }

    function runHighlight() {
        if (hasRun || !window.hljs) return;
        hasRun = true;

        const options = window.jxHighlightOptions || {};
        const enableInlineCode = options.enableInlineCode !== false;
        const cssSelector = enableInlineCode ? "pre code, code" : "pre code";

        try {
            hljs.configure({
                cssSelector: cssSelector,
                ignoreUnescapedHTML: true,
                throwUnescapedHTML: false
            });
            hljs.highlightAll();
        } catch (e) {
            console.warn("Code highlighting failed.", e);
        }

        markInlineCode(enableInlineCode);
    }

    window.jxHighlightAll = runHighlight;

    if (document.readyState === "loading") {
        document.addEventListener("DOMContentLoaded", runHighlight, { once: true });
    } else {
        runHighlight();
    }

    window.addEventListener("load", runHighlight, { once: true });
})();
