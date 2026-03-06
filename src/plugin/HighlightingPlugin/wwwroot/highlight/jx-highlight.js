(function () {
    let hasRun = false;

    function getOptions() {
        const options = window.jxHighlightOptions || {};
        return {
            enableInlineCode: options.enableInlineCode !== false,
            enableCopyButton: options.enableCopyButton !== false,
            enableLineNumbers: options.enableLineNumbers !== false,
            showLanguageLabel: options.showLanguageLabel !== false
        };
    }

    function markInlineCode(enableInlineCode) {
        if (!enableInlineCode) return;

        document.querySelectorAll("code:not(pre code)").forEach(function (codeElement) {
            codeElement.classList.add("jx-inline-hl");
            if (!codeElement.classList.contains("hljs")) {
                codeElement.classList.add("hljs");
            }
        });
    }

    function resolveLanguage(codeElement) {
        if (!codeElement) return "";

        if (codeElement.dataset && codeElement.dataset.language) return codeElement.dataset.language;
        if (codeElement.result && codeElement.result.language) return codeElement.result.language;

        const classList = Array.prototype.slice.call(codeElement.classList || []);
        for (let i = 0; i < classList.length; i++) {
            const cls = classList[i];
            if (cls.indexOf("language-") === 0) return cls.substring("language-".length);
            if (cls.indexOf("lang-") === 0) return cls.substring("lang-".length);
        }

        return "";
    }

    function formatLanguage(language) {
        if (!language) return "";
        return language
            .split(/[-_+]/g)
            .filter(Boolean)
            .map(function (item) {
                if (item.length <= 2) return item.toUpperCase();
                return item.charAt(0).toUpperCase() + item.slice(1);
            })
            .join(" ");
    }

    async function copyText(text) {
        if (!text) return false;

        if (navigator.clipboard && window.isSecureContext) {
            await navigator.clipboard.writeText(text);
            return true;
        }

        const textarea = document.createElement("textarea");
        textarea.value = text;
        textarea.setAttribute("readonly", "readonly");
        textarea.style.position = "fixed";
        textarea.style.opacity = "0";
        textarea.style.pointerEvents = "none";
        document.body.appendChild(textarea);
        textarea.select();
        textarea.setSelectionRange(0, textarea.value.length);
        const success = document.execCommand("copy");
        document.body.removeChild(textarea);
        return success;
    }

    function setCopyButtonState(button, success) {
        if (!button) return;

        const defaultText = button.getAttribute("data-default-text") || "复制";
        button.textContent = success ? "已复制" : "复制失败";
        button.classList.toggle("is-success", !!success);
        button.classList.toggle("is-error", !success);

        window.setTimeout(function () {
            button.textContent = defaultText;
            button.classList.remove("is-success");
            button.classList.remove("is-error");
        }, 1600);
    }

    function clamp(value, min, max) {
        return Math.min(max, Math.max(min, value));
    }

    function parseCssColor(value) {
        if (!value) return null;
        const text = String(value).trim().toLowerCase();
        if (!text || text === "transparent") return { r: 0, g: 0, b: 0, a: 0 };

        if (text.charAt(0) === "#") {
            const hex = text.substring(1);
            if (hex.length === 3 || hex.length === 4) {
                const r = parseInt(hex.charAt(0) + hex.charAt(0), 16);
                const g = parseInt(hex.charAt(1) + hex.charAt(1), 16);
                const b = parseInt(hex.charAt(2) + hex.charAt(2), 16);
                const a = hex.length === 4 ? parseInt(hex.charAt(3) + hex.charAt(3), 16) / 255 : 1;
                if ([r, g, b, a].some(function (x) { return Number.isNaN(x); })) return null;
                return { r: r, g: g, b: b, a: a };
            }

            if (hex.length === 6 || hex.length === 8) {
                const r = parseInt(hex.substring(0, 2), 16);
                const g = parseInt(hex.substring(2, 4), 16);
                const b = parseInt(hex.substring(4, 6), 16);
                const a = hex.length === 8 ? parseInt(hex.substring(6, 8), 16) / 255 : 1;
                if ([r, g, b, a].some(function (x) { return Number.isNaN(x); })) return null;
                return { r: r, g: g, b: b, a: a };
            }

            return null;
        }

        const rgbMatch = text.match(/^rgba?\(([^)]+)\)$/i);
        if (!rgbMatch || !rgbMatch[1]) return null;

        const parts = rgbMatch[1].split(",").map(function (item) { return item.trim(); });
        if (parts.length < 3) return null;

        const r = parseFloat(parts[0]);
        const g = parseFloat(parts[1]);
        const b = parseFloat(parts[2]);
        const a = parts.length >= 4 ? parseFloat(parts[3]) : 1;
        if ([r, g, b, a].some(function (x) { return Number.isNaN(x); })) return null;

        return { r: r, g: g, b: b, a: a };
    }

    function toCssRgba(color, alpha) {
        if (!color) return "";
        const r = clamp(Math.round(color.r), 0, 255);
        const g = clamp(Math.round(color.g), 0, 255);
        const b = clamp(Math.round(color.b), 0, 255);
        const sourceAlpha = typeof color.a === "number" ? color.a : 1;
        const a = clamp(typeof alpha === "number" ? alpha : sourceAlpha, 0, 1);
        return "rgba(" + r + ", " + g + ", " + b + ", " + a + ")";
    }

    function pickFirstOpaqueColor(candidates) {
        for (let i = 0; i < candidates.length; i++) {
            const color = parseCssColor(candidates[i]);
            if (color && color.a > 0.001) return color;
        }
        return null;
    }

    function applyThemePalette(container, preElement, codeElement) {
        if (!container || !preElement || !codeElement || !window.getComputedStyle) return;

        const codeStyle = window.getComputedStyle(codeElement);
        const preStyle = window.getComputedStyle(preElement);
        const containerStyle = window.getComputedStyle(container);
        const bodyStyle = window.getComputedStyle(document.body);

        const backgroundColor = pickFirstOpaqueColor([
            codeStyle.backgroundColor,
            preStyle.backgroundColor,
            containerStyle.backgroundColor,
            bodyStyle.backgroundColor
        ]) || { r: 255, g: 255, b: 255, a: 1 };

        const foregroundColor = pickFirstOpaqueColor([
            codeStyle.color,
            preStyle.color,
            containerStyle.color,
            bodyStyle.color
        ]) || { r: 15, g: 23, b: 42, a: 1 };

        container.style.setProperty("--jx-code-bg", toCssRgba(backgroundColor, backgroundColor.a));
        container.style.setProperty("--jx-code-fg", toCssRgba(foregroundColor, 1));
        container.style.setProperty("--jx-code-border", toCssRgba(foregroundColor, 0.24));
        container.style.setProperty("--jx-toolbar-bg", toCssRgba(backgroundColor, backgroundColor.a));
        container.style.setProperty("--jx-toolbar-fg", toCssRgba(foregroundColor, 0.96));
        container.style.setProperty("--jx-code-lang-color", toCssRgba(foregroundColor, 0.8));
        container.style.setProperty("--jx-line-bg", toCssRgba(foregroundColor, 0.06));
        container.style.setProperty("--jx-line-fg", toCssRgba(foregroundColor, 0.62));
        container.style.setProperty("--jx-copy-border", toCssRgba(foregroundColor, 0.28));
        container.style.setProperty("--jx-copy-border-hover", toCssRgba(foregroundColor, 0.45));
        container.style.setProperty("--jx-copy-hover-bg", toCssRgba(foregroundColor, 0.12));
        container.style.setProperty("--jx-copy-active-bg", toCssRgba(foregroundColor, 0.18));
    }

    function createToolbar(codeElement, options) {
        if (!options.enableCopyButton && !options.showLanguageLabel) return null;

        const toolbar = document.createElement("div");
        toolbar.className = "jx-code-toolbar";

        const left = document.createElement("div");
        left.className = "jx-code-toolbar-left";
        toolbar.appendChild(left);

        if (options.showLanguageLabel) {
            const language = resolveLanguage(codeElement);
            const languageLabel = document.createElement("span");
            languageLabel.className = "jx-code-lang";
            languageLabel.textContent = language ? formatLanguage(language) : "Code";
            left.appendChild(languageLabel);
        }

        if (options.enableCopyButton) {
            const copyButton = document.createElement("button");
            copyButton.type = "button";
            copyButton.className = "jx-copy-btn";
            copyButton.textContent = "复制";
            copyButton.setAttribute("data-default-text", "复制");
            copyButton.addEventListener("click", async function () {
                try {
                    const success = await copyText(codeElement.textContent || "");
                    setCopyButtonState(copyButton, success);
                } catch (e) {
                    setCopyButtonState(copyButton, false);
                }
            });
            toolbar.appendChild(copyButton);
        }

        return toolbar;
    }

    function ensureContainer(preElement) {
        const parent = preElement.parentElement;
        if (parent && parent.classList.contains("jx-code-block")) return parent;

        const container = document.createElement("div");
        container.className = "jx-code-block";
        preElement.parentNode.insertBefore(container, preElement);
        container.appendChild(preElement);
        return container;
    }

    function applyLineNumbers(preElement, codeElement, enableLineNumbers) {
        const oldGutter = preElement.querySelector(".jx-line-numbers");
        if (oldGutter) oldGutter.remove();

        preElement.classList.remove("jx-has-line-numbers");
        if (!enableLineNumbers) return;

        const normalized = (codeElement.textContent || "").replace(/\r\n/g, "\n");
        const lines = normalized.split("\n");
        if (lines.length > 1 && lines[lines.length - 1] === "") lines.pop();
        const lineCount = Math.max(1, lines.length);

        const gutter = document.createElement("div");
        gutter.className = "jx-line-numbers";

        const fragment = document.createDocumentFragment();
        for (let i = 1; i <= lineCount; i++) {
            const line = document.createElement("span");
            line.textContent = String(i);
            fragment.appendChild(line);
        }

        gutter.appendChild(fragment);
        preElement.classList.add("jx-has-line-numbers");
        preElement.appendChild(gutter);
    }

    function enhanceCodeBlocks(options) {
        const codeBlocks = document.querySelectorAll("pre code");
        codeBlocks.forEach(function (codeElement) {
            if (!codeElement || !codeElement.parentElement) return;

            const preElement = codeElement.parentElement;
            if (preElement.dataset.jxEnhanced === "1") return;

            const container = ensureContainer(preElement);
            applyThemePalette(container, preElement, codeElement);
            const toolbar = createToolbar(codeElement, options);
            if (toolbar) container.insertBefore(toolbar, preElement);

            applyLineNumbers(preElement, codeElement, options.enableLineNumbers);
            preElement.dataset.jxEnhanced = "1";
        });
    }

    function runHighlight() {
        if (hasRun || !window.hljs) return;
        hasRun = true;

        const options = getOptions();
        const cssSelector = options.enableInlineCode ? "pre code, code" : "pre code";

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

        markInlineCode(options.enableInlineCode);
        enhanceCodeBlocks(options);
    }

    window.jxHighlightAll = runHighlight;

    if (document.readyState === "loading") {
        document.addEventListener("DOMContentLoaded", runHighlight, { once: true });
    } else {
        runHighlight();
    }

    window.addEventListener("load", runHighlight, { once: true });
})();
