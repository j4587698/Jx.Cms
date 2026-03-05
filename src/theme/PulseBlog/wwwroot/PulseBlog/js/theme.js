(function () {
    const siteHeader = document.getElementById("siteHeader");
    const menuToggle = document.getElementById("menuToggle");
    const nav = document.getElementById("primaryNav");

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

    if (menuToggle && nav) {
        menuToggle.addEventListener("click", function () {
            const opened = nav.classList.toggle("is-open");
            menuToggle.setAttribute("aria-expanded", opened ? "true" : "false");
        });

        document.addEventListener("click", function (event) {
            if (!nav.contains(event.target) && !menuToggle.contains(event.target)) {
                closeMenu();
            }
        });

        document.addEventListener("keydown", function (event) {
            if (event.key === "Escape") {
                closeMenu();
            }
        });

        window.addEventListener("resize", function () {
            if (window.innerWidth > 860) {
                closeMenu();
            }
        });
    }

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

    const revealTargets = document.querySelectorAll(".js-reveal");
    if (revealTargets.length > 0 && "IntersectionObserver" in window) {
        const observer = new IntersectionObserver(function (entries) {
            entries.forEach(function (entry) {
                if (entry.isIntersecting) {
                    entry.target.classList.add("is-visible");
                    observer.unobserve(entry.target);
                }
            });
        }, { threshold: 0.12 });

        revealTargets.forEach(function (item, index) {
            item.style.transitionDelay = Math.min(index * 38, 260) + "ms";
            observer.observe(item);
        });
    } else {
        revealTargets.forEach(function (item) {
            item.classList.add("is-visible");
        });
    }

    window.addEventListener("scroll", function () {
        toggleHeader();
        updateProgress();
    }, { passive: true });

    toggleHeader();
    updateProgress();
})();
