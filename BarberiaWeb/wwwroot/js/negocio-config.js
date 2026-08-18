// Carga la configuración pública del negocio (GET /api/negocio) y aplica
// el theming (CSS custom properties) a la página actual.
// Pensado para ser incluido con <script src="/js/negocio-config.js"></script>
// antes del script principal de cada página HTML estática.

async function cargarConfigNegocio() {
    const res = await fetch(`${window.location.origin}/api/negocio`);
    const negocio = await res.json();
    aplicarTemaNegocio(negocio);
    return negocio;
}

function aplicarTemaNegocio(negocio) {
    const r = document.documentElement.style;
    r.setProperty('--primary', negocio.colorPrimario);
    r.setProperty('--accent', negocio.colorAcento);
    r.setProperty('--text', negocio.colorTexto);
    r.setProperty('--text-light', negocio.colorTextoClaro);
    r.setProperty('--bg', negocio.colorFondo);
    r.setProperty('--bg-light', negocio.colorFondoClaro);
}
