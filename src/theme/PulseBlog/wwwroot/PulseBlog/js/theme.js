(function () {
    const siteHeader = document.getElementById("siteHeader");
    const menuToggle = document.getElementById("menuToggle");
    const nav = document.getElementById("primaryNav");
    const searchWrap = document.getElementById("searchWrap");
    const searchToggle = document.getElementById("searchToggle");
    const siteSearch = document.getElementById("siteSearch");
    const siteSearchInput = document.getElementById("siteSearchInput");

    const toggleHeader = function () {
        if (!siteHeader) {
            return;
        }

        siteHeader.classList.toggle("is-scrolled", window.scrollY > 8);
    };

    const closeMenu = function () {
        if (!nav || !menuToggle) {
            return;
        }

        nav.classList.remove("is-open");
        menuToggle.setAttribute("aria-expanded", "false");
    };

    const initNavHierarchy = function () {
        if (!nav) {
            return;
        }

        const items = nav.querySelectorAll("li");
        Array.prototype.forEach.call(items, function (item) {
            if (!item || !item.children || item.children.length === 0) {
                return;
            }

            let hasDirectSubmenu = false;
            Array.prototype.forEach.call(item.children, function (child) {
                if (child && child.tagName === "UL") {
                    hasDirectSubmenu = true;
                }
            });

            if (hasDirectSubmenu) {
                item.classList.add("has-children");
            }
        });
    };

    const closeSearch = function () {
        if (!searchWrap || !searchToggle) {
            return;
        }

        searchWrap.classList.remove("is-open");
        searchToggle.setAttribute("aria-expanded", "false");
    };

    if (menuToggle && nav) {
        initNavHierarchy();

        menuToggle.addEventListener("click", function () {
            closeSearch();
            const opened = nav.classList.toggle("is-open");
            menuToggle.setAttribute("aria-expanded", opened ? "true" : "false");
        });

        window.addEventListener("resize", function () {
            if (window.innerWidth > 860) {
                closeMenu();
            }

            closeSearch();
        });
    }

    if (searchWrap && searchToggle && siteSearch) {
        searchToggle.addEventListener("click", function (event) {
            event.preventDefault();
            closeMenu();

            const opened = searchWrap.classList.toggle("is-open");
            searchToggle.setAttribute("aria-expanded", opened ? "true" : "false");
            if (opened && siteSearchInput) {
                siteSearchInput.focus();
                siteSearchInput.select();
            }
        });
    }

    document.addEventListener("click", function (event) {
        const target = event.target;

        if (nav && menuToggle && !nav.contains(target) && !menuToggle.contains(target)) {
            closeMenu();
        }

        if (searchWrap && !searchWrap.contains(target)) {
            closeSearch();
        }
    });

    document.addEventListener("keydown", function (event) {
        if (event.key === "Escape") {
            closeMenu();
            closeSearch();
        }
    });

    const progress = document.getElementById("topProgress");
    const updateProgress = function () {
        if (!progress) {
            return;
        }

        const scrollTop = document.documentElement.scrollTop || document.body.scrollTop;
        const height = document.documentElement.scrollHeight - document.documentElement.clientHeight;
        if (height <= 0) {
            progress.style.width = "0";
            return;
        }

        const rate = Math.min(100, Math.max(0, (scrollTop / height) * 100));
        progress.style.width = rate.toFixed(2) + "%";
    };

    let revealObserver = null;
    let revealIndex = 0;
    if ("IntersectionObserver" in window) {
        revealObserver = new IntersectionObserver(function (entries) {
            entries.forEach(function (entry) {
                if (entry.isIntersecting) {
                    entry.target.classList.add("is-visible");
                    revealObserver.unobserve(entry.target);
                }
            });
        }, { threshold: 0.12 });
    }

    const registerReveal = function (targets) {
        if (!targets || targets.length === 0) {
            return;
        }

        Array.prototype.forEach.call(targets, function (item) {
            if (!item || item.dataset.revealBound === "1") {
                return;
            }

            item.dataset.revealBound = "1";
            item.style.transitionDelay = Math.min(revealIndex * 38, 260) + "ms";
            revealIndex += 1;

            if (revealObserver) {
                revealObserver.observe(item);
            } else {
                item.classList.add("is-visible");
            }
        });
    };

    const getNextPageUrl = function (pager) {
        if (!pager) {
            return null;
        }

        const links = Array.prototype.slice.call(pager.querySelectorAll("a[href]"));
        if (links.length === 0) {
            return null;
        }

        const current = pager.querySelector(".current");
        let currentPage = NaN;
        if (current) {
            currentPage = parseInt((current.textContent || "").trim(), 10);
        }

        if (!isNaN(currentPage)) {
            const candidates = links
                .map(function (link) {
                    return {
                        link: link,
                        page: parseInt((link.textContent || "").trim(), 10)
                    };
                })
                .filter(function (item) {
                    return !isNaN(item.page) && item.page > currentPage;
                })
                .sort(function (a, b) {
                    return a.page - b.page;
                });

            if (candidates.length > 0) {
                return candidates[0].link.href;
            }
        }

        const explicitNext = pager.querySelector("a[rel='next'], .next a, a.next");
        if (explicitNext && explicitNext.href) {
            return explicitNext.href;
        }

        if (current && typeof current.compareDocumentPosition === "function" && typeof Node !== "undefined") {
            const nextByDom = links.find(function (link) {
                return (current.compareDocumentPosition(link) & Node.DOCUMENT_POSITION_FOLLOWING) !== 0;
            });
            if (nextByDom) {
                return nextByDom.href;
            }
        }

        return links[0].href;
    };

    const normalizeUrl = function (url) {
        try {
            return new URL(url, window.location.href).toString();
        } catch (_e) {
            return url;
        }
    };

    const initDynamicPager = function () {
        const modeRaw = document.body ? (document.body.getAttribute("data-list-pagination-mode") || "pager") : "pager";
        const mode = modeRaw.toLowerCase();
        if (mode === "pager") {
            return;
        }

        if (typeof window.fetch !== "function" || typeof window.DOMParser === "undefined") {
            return;
        }

        const storyFeed = document.querySelector(".story-feed");
        const pager = document.querySelector(".pager");
        if (!storyFeed || !pager) {
            return;
        }

        pager.classList.add("is-hidden");

        const controls = document.createElement("div");
        controls.className = "pager-dynamic-controls";

        const loadButton = document.createElement("button");
        loadButton.type = "button";
        loadButton.className = "pager-load-btn";
        loadButton.textContent = "加载下一页";
        controls.appendChild(loadButton);

        const hint = document.createElement("p");
        hint.className = "pager-load-hint muted";
        controls.appendChild(hint);

        const sentinel = document.createElement("div");
        sentinel.className = "pager-auto-sentinel";
        controls.appendChild(sentinel);

        pager.insertAdjacentElement("afterend", controls);

        let loading = false;
        let hasMore = true;
        let autoObserver = null;
        const loadedUrls = new Set([normalizeUrl(window.location.href)]);

        const updateHint = function () {
            if (!hasMore) {
                hint.textContent = "已经到底了";
                return;
            }

            if (mode === "autoload") {
                hint.textContent = "继续下拉可自动加载，也可以点击按钮手动加载。";
            } else {
                hint.textContent = "";
            }
        };

        const setIdle = function () {
            const nextUrl = getNextPageUrl(pager);
            if (!nextUrl) {
                hasMore = false;
                loadButton.disabled = true;
                loadButton.textContent = "没有更多内容";
                if (autoObserver) {
                    autoObserver.disconnect();
                }
            } else {
                hasMore = true;
                loadButton.disabled = false;
                loadButton.textContent = "加载下一页";
            }

            updateHint();
            return nextUrl;
        };

        const appendNextPage = function (html) {
            const parser = new DOMParser();
            const nextDoc = parser.parseFromString(html, "text/html");
            const nextFeed = nextDoc.querySelector(".story-feed");
            const nextPager = nextDoc.querySelector(".pager");
            if (!nextFeed || !nextPager) {
                throw new Error("invalid-next-page");
            }

            const cards = Array.prototype.slice.call(nextFeed.children);
            cards.forEach(function (card) {
                storyFeed.appendChild(card);
            });

            if (cards.length > 0) {
                const revealCards = cards.filter(function (card) {
                    return card.classList && card.classList.contains("js-reveal");
                });
                registerReveal(revealCards);
            }

            pager.innerHTML = nextPager.innerHTML;
            return cards.length > 0;
        };

        const loadNext = function (triggerType) {
            if (loading || !hasMore) {
                return;
            }

            const nextUrl = getNextPageUrl(pager);
            if (!nextUrl) {
                setIdle();
                return;
            }

            const normalizedNextUrl = normalizeUrl(nextUrl);
            if (loadedUrls.has(normalizedNextUrl)) {
                hasMore = false;
                setIdle();
                return;
            }

            loading = true;
            loadButton.disabled = true;
            loadButton.textContent = "加载中...";
            if (triggerType === "auto") {
                hint.textContent = "正在自动加载下一页...";
            }

            fetch(nextUrl, { credentials: "same-origin" })
                .then(function (resp) {
                    if (!resp.ok) {
                        throw new Error("http-" + resp.status);
                    }
                    return resp.text();
                })
                .then(function (html) {
                    loadedUrls.add(normalizedNextUrl);
                    const appended = appendNextPage(html);
                    if (!appended) {
                        hasMore = false;
                    }
                })
                .catch(function () {
                    hint.textContent = "加载失败，请点击按钮重试。";
                })
                .finally(function () {
                    loading = false;
                    setIdle();
                });
        };

        loadButton.addEventListener("click", function () {
            loadNext("click");
        });

        if (mode === "autoload" && "IntersectionObserver" in window) {
            autoObserver = new IntersectionObserver(function (entries) {
                entries.forEach(function (entry) {
                    if (entry.isIntersecting) {
                        loadNext("auto");
                    }
                });
            }, { rootMargin: "260px 0px" });

            autoObserver.observe(sentinel);
        } else {
            sentinel.style.display = "none";
        }

        setIdle();
    };

    registerReveal(document.querySelectorAll(".js-reveal"));
    initDynamicPager();

    window.addEventListener("scroll", function () {
        toggleHeader();
        updateProgress();
    }, { passive: true });

    toggleHeader();
    updateProgress();
})();
