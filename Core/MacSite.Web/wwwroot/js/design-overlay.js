// Dev-only pixel-fidelity overlay. Ghosts the original design mockup over the live page.
(function () {
    const img = document.getElementById('ovl-img');
    const show = document.getElementById('ovl-show');
    const op = document.getElementById('ovl-op');
    if (!img || !show || !op) return;

    function applyVisibility() {
        img.style.display = show.checked ? 'block' : 'none';
    }

    show.addEventListener('change', applyVisibility);
    op.addEventListener('input', () => { img.style.opacity = op.value; });

    const set = (src) => () => { img.src = src; show.checked = true; applyVisibility(); };
    document.getElementById('ovl-home')?.addEventListener('click', set('/_design/home.jpg'));
    document.getElementById('ovl-port')?.addEventListener('click', set('/_design/portfolio.jpg'));
    document.getElementById('ovl-mob')?.addEventListener('click', set('/_design/home-mobile.jpg'));

    // Keyboard: 'o' toggles the overlay quickly.
    document.addEventListener('keydown', (e) => {
        if (e.key === 'o' && !/input|textarea/i.test(e.target.tagName)) {
            show.checked = !show.checked;
            applyVisibility();
        }
    });

    applyVisibility();
})();
