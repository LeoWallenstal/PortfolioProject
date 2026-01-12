//Hanterar dialog rutan som kräver att användaren bekräftar innan ett meddelande raderas.
function initDeleteDialog() {
    const overlay = document.getElementById("deleteDialogOverlay")
    if (!overlay) return;

    const dialog = overlay.querySelector(".delete-dialog")
    const confirmBtn = document.getElementById("deleteDialogConfirm");

    let pendingForm = null;
    let lastFocused = null;

    const openDialog = (form) => {
        pendingForm = form;
        lastFocused = document.activeElement;

        overlay.classList.remove("d-none");
        overlay.setAttribute("aria-hidden", "false");
        document.body.style.overflow = "hidden";

        dialog.focus();
    };

    const closeDialog = () => {
        pendingForm = null;

        overlay.classList.add("d-none");
        overlay.setAttribute("aria-hidden", "true");
        document.body.style.overflow = "";

        if (lastFocused) lastFocused.focus();
    };

    document.addEventListener("click", (e) => {
        const deleteBtn = e.target.closest("button.msg-delete");
        if (!deleteBtn) return;

        const form = deleteBtn.closest("form");
        if (!form) return;

        e.preventDefault();
        openDialog(form);
    });

    confirmBtn.addEventListener("click", () => {
        if (!pendingForm) return;
        pendingForm.submit();
    });

    overlay.addEventListener("click", (e) => {
        if (e.target === overlay) closeDialog();
        if (e.target.closest(".close")) closeDialog();
    });

    document.addEventListener("keydown", (e) => {
        if (e.key === "Escape" && !overlay.classList.contains("d-none")) {
            closeDialog();
        }
    });

}


//Metod för att swapa partial istället för att ladda om hela sidan.
//Dels för att inte hela sidan ska få en "blinkande" effekt,
//men också för att inte förlora felmeddelanden från DataAnnotations
async function postThread(form, push) {
    const threadView = document.getElementById("threadView");
    if (!threadView) return;

    const res = await fetch(form.action, {
        method: "POST",
        body: new FormData(form)
    });

    //Om det lyckas så ska sidan laddas om istället för att bara byta partial.
    if (res.status === 204) {
        window.location.href = form.action.replace(/\/send(\?|$)/, "?");
        return;
    }

    const html = await res.text();
    threadView.innerHTML = html;

    initThreadBehaviors(threadView);

    if (push) history.pushState({}, "", form.action);
}


//Ser till att man kommer ner till nyaste chattarna direkt.
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
function initGlobalEmojiClosers(root) {
    document.addEventListener("click", (e) => {
        const picker = root?.querySelector(".emoji-picker");
        if (!picker) return;

        if (!e.target.closest(".emoji-picker") && !e.target.closest(".input-box")) {
            picker.hidden = true;
        }
    });

    document.addEventListener("keydown", (e) => {
        if (e.key !== "Escape") return;
        const picker = root?.querySelector(".emoji-picker");
        if (picker) picker.hidden = true;
    });
}

/* !OBS! 
Just nu räknas vissa emojis som flera chars, se över hur det ska hanteras. 
!OBS! */

//Kontrollerar längden på meddelande
function initCounter(root) {
    const textarea = root.querySelector(".text-box textarea");
    const counter = root.querySelector("#counter");
    if (!textarea || !counter) return;

    const update = () => { counter.textContent = `${textarea.value.length}/${textarea.maxLength}`; };
    textarea.addEventListener("input", update);
    update();
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

//Variabel för att unvika att man attachar lyssnaren flera gånger, tex vid felaktig inmatning.
let sendListenerAttached = false;
//Fångar upp när man skickar ett meddelande för att fetcha med AJAX
function initSend(root) {
    if (sendListenerAttached) return;
    sendListenerAttached = true;

    root.addEventListener("submit", (e) => {
        const form = e.target;
        if (!(form instanceof HTMLFormElement)) return;

        if (!form.matches("#sendForm")) return;

        e.preventDefault();
        postThread(form, false);
    });
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

export function initThreadBehaviors(root) {
    if (!root) return;

    initDeleteDialog();
    initScroll(root);
    initEmoji(root);
    initGlobalEmojiClosers(root);
    initCounter(root);
    initResize(root);
    initSend(root);
    initEnterToSend(root);
    initClearError(root);
    
}


document.addEventListener("DOMContentLoaded", () => {
    initThreadBehaviors(document);
});
