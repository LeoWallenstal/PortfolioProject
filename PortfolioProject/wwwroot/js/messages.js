(() => {
    /*===============================================*/
    /*======== Scripts för _ConversationThread ======*/
    /*===============================================*/

    //Ser till att man skrollar ner till dem senaste meddelandena när man kommer in på sidan.
    function initScroll(root) {
        const m = root.querySelector(".messages");
        if (!m) return;

        m.style.visibility = "hidden";
        const scroll = () => { m.scrollTop = m.scrollHeight; };

        scroll();
        requestAnimationFrame(() => {
            scroll();
            m.style.visibility = "";
        });
    }

    /* 
      Hanterar öppning av picker och val/input av emojis. 
    */
    function initEmoji(root) {
        const btn = root.querySelector(".emoji");
        const picker = root.querySelector(".emoji-picker");
        const textarea = root.querySelector(".text-box textarea");
        if (!btn || !picker || !textarea) return;

        //För att inte tappa fokus från textarea
        btn.addEventListener("mousedown", e => e.preventDefault());
        picker.addEventListener("mousedown", e => e.preventDefault());

        btn.addEventListener("click", (e) => {
            e.stopPropagation();
            picker.hidden = !picker.hidden;
        });

        picker.addEventListener("emoji-click", (e) => {
            const emoji = e.detail.unicode;
            const start = textarea.selectionStart;
            const end = textarea.selectionEnd;

            textarea.value =
                textarea.value.slice(0, start) +
                emoji +
                textarea.value.slice(end);

            const pos = start + emoji.length;
            textarea.setSelectionRange(pos, pos);
            textarea.focus();
            textarea.dispatchEvent(new Event("input", { bubbles: true }));
        });
    }

    //Hanterar stängning av emoji pickern
    function initGlobalEmojiClosers() {
        document.addEventListener("click", (e) => {
            const root = document.getElementById("threadView");
            const picker = root?.querySelector(".emoji-picker");
            if (!picker) return;

            if (!e.target.closest(".emoji-picker") && !e.target.closest(".input-box")) {
                picker.hidden = true;
            }
        });

        document.addEventListener("keydown", (e) => {
            if (e.key !== "Escape") return;
            const root = document.getElementById("threadView");
            const picker = root?.querySelector(".emoji-picker");
            if (picker) picker.hidden = true;
        });
    }

    /* !OBS! 
    Just nu räknas vissa emojis som flera chars, hanterar heller inte nådd maxgräns. 
    Se över hur det ska hanteras. 
    !OBS! */

    //Kontrollerar längden på meddelande
    function initCounter(root) {
        const textarea = root.querySelector(".text-box textarea");
        const counter = root.querySelector("#counter");
        if (!textarea || !counter) return;

        const update = () => { counter.textContent = `${textarea.value.length}/${textarea.maxLength}`; };
        textarea.addEventListener("input", update);
        update(); // init
    }

    //Sköter växandet av input rutan vid radbrytning.
    function initResize(root) {
        const textarea = root.querySelector(".text-box textarea");
        const wrapper = root.querySelector(".text-box");
        if (!textarea || !wrapper) return;

        const base = textarea.scrollHeight;

        function getMaxH() {
            const mh = getComputedStyle(textarea).maxHeight;
            return mh === "none" ? Infinity : parseFloat(mh);
        }

        function resize() {
            const maxH = getMaxH();
            textarea.style.height = "0px";
            const needed = Math.max(base, textarea.scrollHeight);
            const height = Math.min(needed, maxH);

            textarea.style.height = height + "px";
            wrapper.style.height = height + "px";
            textarea.style.overflowY = needed > maxH ? "auto" : "hidden";
        }

        textarea.addEventListener("input", resize);
        window.addEventListener("resize", resize);
        resize();
    }

    //Gör så att användare skicka meddelande med enter men låter shift+enter skapa ny rad.
    function initEnterToSend(root) {
        const textarea = root.querySelector(".text-box textarea");
        const sendBtn = root.querySelector(".send-btn");
        if (!textarea || !sendBtn) return;

        textarea.addEventListener("keydown", (e) => {
            if (e.key === "Enter" && !e.shiftKey) {
                e.preventDefault();
                sendBtn.click();
            }
        });
    }

    //Clearar errors
    function initClearError(root) {
        const inputBox = root.querySelector(".input-box");
        if (!inputBox) return;

        inputBox.addEventListener("input", () => {
            const err = root.querySelector("#send-error");
            if (err) err.remove();
        });
    }

    function initThreadBehaviors(root) {
        initScroll(root);
        initEmoji(root);
        initCounter(root);
        initResize(root);
        initEnterToSend(root);
        initClearError(root);
    }

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

        if (push) history.pushState({}, "", url);

        initThreadBehaviors(threadView);
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

        function setActive(item) {
            listRoot.querySelectorAll(".conversation-item.is-active")
                .forEach(el => el.classList.remove("is-active"))

            item.classList.add("is-active");
            scrollActiveIntoView(listRoot);
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
        initThreadBehaviors(threadView);
    }

    // Boot (script should be loaded with defer)
    initGlobalEmojiClosers();
    initConversationList();

})();
