(() => {
    /*===============================================*/
    /*======== Scripts för _ConversationList ========*/
    /*===============================================*/

    //Laddar in en konversation som en partial.
    async function loadThread(url, push) {
        const threadView = document.getElementById("threadView");
        if (!threadView) return;

        const res = await fetch(url, {
            headers: { "X-Requested-With": "fetch" }
        });

        if (!res.ok) {
            window.location.href = url;
            return;
        }

        const html = await res.text();
        threadView.innerHTML = html;

        const mod = await import("/js/messageThread.js");
        mod.initThreadBehaviors(threadView);

        if (push) history.pushState({}, "", url);

    }



    function scrollActiveIntoView(listRoot) {
        const active = listRoot.querySelector(".conversation-item.is-active");
        if (!active) return;

        // Only scroll if it’s outside the visible area of the list
        const container = listRoot; // or listRoot.querySelector(".your-scroll-container") if different
        const cRect = container.getBoundingClientRect();
        const aRect = active.getBoundingClientRect();

        const above = aRect.top < cRect.top;
        const below = aRect.bottom > cRect.bottom;

        if (above || below) {
            active.scrollIntoView({ block: "nearest", inline: "nearest", behavior: "smooth" });
        }
    }


    //Initierar listan med existerande konversationer
    function initConversationList() {
        const listRoot = document.querySelector(".thread-list");
        const threadView = document.getElementById("threadView");
        if (!listRoot || !threadView) return;

        function applyReadUi(item) {
            const dot = item.querySelector(".notification-dot");
            if (!dot) return;

            const unreadHere = parseInt(dot.textContent, 10) || 0;
            if (!unreadHere) return;

            dot.remove();

            const totalDot = document.getElementById("totalUnread");
            if (!totalDot) return;

            const total = parseInt(totalDot.textContent, 10) || 0;
            const next = Math.max(0, total - unreadHere);

            if (next <= 0) {
                totalDot.remove();
            } else if (next >= 100) {
                totalDot.textContent = '99+';
            }else {
                totalDot.textContent = next;
            }
        }


        function setActive(item) {
            listRoot.querySelectorAll(".conversation-item.is-active")
                .forEach(el => el.classList.remove("is-active"))

            item.classList.add("is-active");
            scrollActiveIntoView(listRoot);

            applyReadUi(item);
        }

        function updateActiveFromUrl() {
            const current = window.location.pathname + window.location.search;

            document
                .querySelectorAll(".conversation-item")
                .forEach(item => {
                    const url = item.dataset.url;
                    if (!url) return;

                    item.classList.toggle("is-active", url === current);
                });
            scrollActiveIntoView(listRoot);
        }

        listRoot.addEventListener("click", (e) => {
            if (e.target.closest("a, button, input")) return;
            if (e.target.closest(".is-active")) return;

            const item = e.target.closest(".conversation-item");
            if (!item) return;


            const url = item.dataset.url;
            if (!url) return;

            if (window.matchMedia("(max-width: 768px)").matches) {
                window.location.href = url;
                return;
            }

            e.preventDefault();
            setActive(item);
            loadThread(url, true);
        });

        window.addEventListener("popstate", () => {
            loadThread(window.location.href, false);
            updateActiveFromUrl();
        });

        // initial page render
        updateActiveFromUrl();
    }

    // Boot (script should be loaded with defer)
    initConversationList();

})();
