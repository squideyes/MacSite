// Lightweight, dependency-free carousels. Works with static SSR (no Blazor circuit).
// Markup contract:
//   <root data-carousel data-cr-mode="slide|fade" data-cr-autoplay="true" data-cr-interval="6000">
//     <viewport>            (the overflow-hidden parent of the track; optional for fade)
//       <track data-cr-track> <cell> ... </track>
//     </viewport>
//     <dots data-cr-dots></dots>            (optional; buttons generated here)
//     <button data-cr-prev>  <button data-cr-next>   (optional arrows)
(function () {
    function init(root) {
        if (root.__crInit) return;
        root.__crInit = true;

        const track = root.querySelector('[data-cr-track]');
        if (!track) return;
        const cells = Array.from(track.children);
        if (cells.length === 0) return;

        const mode = root.dataset.crMode || 'slide';
        const autoplay = root.dataset.crAutoplay === 'true';
        const interval = Math.max(2500, parseInt(root.dataset.crInterval || '6000', 10));
        const viewport = track.parentElement;
        const dotsWrap = root.querySelector('[data-cr-dots]');

        let index = 0;
        let timer = null;

        let dots = [];
        if (dotsWrap) {
            dotsWrap.innerHTML = '';
            cells.forEach((_, i) => {
                const b = document.createElement('button');
                b.type = 'button';
                b.className = 'cr-dot';
                b.setAttribute('aria-label', 'Go to slide ' + (i + 1));
                b.addEventListener('click', () => { go(i); restart(); });
                dotsWrap.appendChild(b);
            });
            dots = Array.from(dotsWrap.children);
        }

        const gap = () => parseFloat(getComputedStyle(track).columnGap || getComputedStyle(track).gap || '0') || 0;
        const step = () => cells[0].getBoundingClientRect().width + gap();

        function maxIndex() {
            if (mode === 'fade') return cells.length - 1;
            const total = cells.length * step() - gap();
            const over = total - viewport.clientWidth;
            if (over <= 1) return 0;
            return Math.min(cells.length - 1, Math.ceil(over / step()));
        }

        function render() {
            if (mode === 'fade') {
                cells.forEach((c, i) => c.classList.toggle('is-active', i === index));
            } else {
                track.style.transform = 'translateX(' + (-index * step()) + 'px)';
            }
            dots.forEach((d, i) => d.classList.toggle('is-active', i === index));
        }

        function go(i) {
            const max = maxIndex();
            index = i < 0 ? max : (i > max ? 0 : i);
            render();
        }
        const next = () => go(index + 1);
        const prev = () => go(index - 1);

        root.querySelectorAll('[data-cr-prev]').forEach(b => b.addEventListener('click', () => { prev(); restart(); }));
        root.querySelectorAll('[data-cr-next]').forEach(b => b.addEventListener('click', () => { next(); restart(); }));

        if (mode === 'slide') {
            let startX = 0, base = 0, dragging = false;
            viewport.style.touchAction = 'pan-y';
            viewport.addEventListener('pointerdown', e => {
                dragging = true; startX = e.clientX; base = -index * step();
                track.style.transition = 'none';
                try { viewport.setPointerCapture(e.pointerId); } catch { }
            });
            viewport.addEventListener('pointermove', e => {
                if (!dragging) return;
                track.style.transform = 'translateX(' + (base + (e.clientX - startX)) + 'px)';
            });
            const end = e => {
                if (!dragging) return;
                dragging = false;
                track.style.transition = '';
                const dx = e.clientX - startX;
                if (Math.abs(dx) > 55) { dx < 0 ? next() : prev(); } else render();
                restart();
            };
            viewport.addEventListener('pointerup', end);
            viewport.addEventListener('pointercancel', end);
        }

        function start() { if (autoplay && cells.length > 1 && !timer) timer = setInterval(next, interval); }
        function stop() { if (timer) { clearInterval(timer); timer = null; } }
        function restart() { stop(); start(); }

        root.addEventListener('mouseenter', stop);
        root.addEventListener('mouseleave', start);
        let rt;
        window.addEventListener('resize', () => { clearTimeout(rt); rt = setTimeout(() => { go(Math.min(index, maxIndex())); }, 120); });

        render();
        start();
    }

    function initAll() { document.querySelectorAll('[data-carousel]').forEach(init); }

    if (document.readyState === 'loading') document.addEventListener('DOMContentLoaded', initAll);
    else initAll();
    document.addEventListener('enhancedload', initAll);
})();
