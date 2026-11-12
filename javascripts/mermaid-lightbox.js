// Click-to-enlarge for Mermaid diagrams (see overrides/main.html).
(function () {
  function close(overlay) {
    overlay.remove();
    document.body.style.overflow = "";
  }

  function open(source) {
    var overlay = document.createElement("div");
    overlay.className = "mermaid-lightbox";
    overlay.setAttribute("role", "dialog");
    overlay.setAttribute("aria-modal", "true");
    overlay.title = "Bezárás (Esc)";

    // Copy the diagram together with Material's Mermaid theme styles.
    var shadow = overlay.attachShadow({ mode: "open" });
    shadow.innerHTML =
      source.shadowRoot.innerHTML +
      "<style>" +
      ":host{display:flex;align-items:center;justify-content:center}" +
      "svg{width:100%!important;height:100%!important;max-width:none!important}" +
      "</style>";

    overlay.addEventListener("click", function () { close(overlay); });
    document.addEventListener("keydown", function onKey(e) {
      if (e.key === "Escape") {
        close(overlay);
        document.removeEventListener("keydown", onKey);
      }
    });

    document.body.appendChild(overlay);
    document.body.style.overflow = "hidden";
  }

  document.addEventListener("click", function (e) {
    var host = e.target.closest && e.target.closest("div.mermaid");
    if (host && host.shadowRoot && host.shadowRoot.querySelector("svg")) {
      open(host);
    }
  });
})();
