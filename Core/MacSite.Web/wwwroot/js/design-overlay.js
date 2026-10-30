// Dev-only pixel-fidelity overlay. Ghosts the original design mockup over the live page.
(function () {
    const img = document.getElementById('ovl-img');
    const show = document.getElementById('ovl-show');
    const op = document.getElementById('ovl-op');
    if (!img || !show || !op) return;

    function pauseCarousels() {
        if (typeof jQuery === 'undefined' || typeof $.fn.slick === 'undefined') return;
        $('.home-banner .slider.slick-initialized').each(function () {
            $(this).slick('slickGoTo', 0, true);
            $(this).slick('slickPause');
        });
    }

    function resumeCarousels() {
        if (typeof jQuery === 'undefined' || typeof $.fn.slick === 'undefined') return;
        $('.home-banner .slider.slick-initialized').each(function () {
            $(this).slick('slickPlay');
        });
    }

    function applyVisibility() {
        const on = show.checked;
        document.documentElement.classList.toggle('ovl-active', on);
        img.style.display = on ? 'block' : 'none';
        if (on) pauseCarousels();
        else resumeCarousels();
    }

    show.addEventListener('change', applyVisibility);
    op.addEventListener('input', () => { img.style.opacity = op.value; });

    const set = (src) => () => {
        img.src = src;
        show.checked = true;
        applyVisibility();
    };

    document.getElementById('ovl-home')?.addEventListener('click', set('/_design/home.jpg'));
    document.getElementById('ovl-port')?.addEventListener('click', set('/_design/portfolio.jpg'));
    document.getElementById('ovl-mob')?.addEventListener('click', set('/_design/home-mobile.jpg'));

    document.addEventListener('keydown', (e) => {
        if (e.key === 'o' && !/input|textarea/i.test(e.target.tagName)) {
            show.checked = !show.checked;
            applyVisibility();
        }
    });

    if (typeof Blazor !== 'undefined' && typeof Blazor.addEventListener === 'function') {
        Blazor.addEventListener('enhancedload', () => {
            if (show.checked) pauseCarousels();
        });
    }

    applyVisibility();
})();
