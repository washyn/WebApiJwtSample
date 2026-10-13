# Prompt: Integrar InfinitePerspectiveGrid + GSAP en una Landing Page

> Instrucciones listas para pasar a un agente/LLM y que implemente la grilla en perspectiva infinita animada con GSAP sobre una sección hero (o cualquier sección full-bleed) de una landing page existente.

---

## Contexto / Requerimiento

Quiero integrar una **grilla en perspectiva infinita animada** (estilo tron / wireframe retrofuturista) como fondo visual de una sección hero de una landing page, con animaciones de entrada GSAP para el copy encima, reveals vinculados al scroll para el resto de secciones y micro-interacciones.

Referencia de la grilla: `infinite-perspective-grid.md` (canvas 2D con punto de fuga en el horizonte, líneas horizontales que avanzan hacia el espectador, respeto de `prefers-reduced-motion`, pausa en `visibilitychange`, manejo de `devicePixelRatio`).

---

## Prompt completo para un agente

```
Actúa como desarrollador frontend con expertise en GSAP y composición visual premium. Tienes que integrar la animación de "grilla en perspectiva infinita (canvas 2D)" descrita en ./infinite-perspective-grid.md como el fondo visual dominante del hero de una landing page, y combinarla con animaciones GSAP para todo el ingreso y navegación de la página.

Requerimientos técnicos obligatorios:

1. GRILLE EN PERSPECTIVA INFINITA (canvas 2D sin librerías)
   - Porta el renderer `createInfinitePerspectiveGridRenderer` del markdown a vanilla JavaScript, respetando 100% su API y comportamiento:
     • opciones: speed, lineOpacity, lineWidth, horizonPosition, perspective, horizontalLineCount, verticalLineCount
     • controller devuelve { update(next), destroy() }
     • respeta (prefers-reduced-motion: reduce): pausa la animación y muestra solo el frame estático
     • pausa al document.hidden (visibilitychange)
     • ajusta canvas.width/height a devicePixelRatio y llama ctx.setTransform(dpr,0,0,dpr,0,0) para líneas nítidas
     • color líneas: rgba(151,199,255,alpha) verticales / rgba(118,178,255,alpha) horizontales
     • horizonte con un glow lineal (degradé transparente → rgba(101,153,255,*) → transparente) centrado en horizonY
   - Instancia la grilla en 2 puntos:
     a) <canvas id="gridCanvas"> dentro del <section class="hero">. Opciones sugeridas: speed: 0.18, horizonPosition: 0.58, perspective: 0.95, horizontalLineCount: 22, verticalLineCount: 13
     b) <canvas id="gridCanvasBottom" class="grid-canvas-bottom"> dentro de la sección CTA final, con CSS transform: scaleY(-1) para invertir la perspectiva. Opciones: speed: 0.10, lineOpacity: 0.42, horizonPosition: 0.62
   - CSS mínimo (importante para que no rompa):
     .hero { position: relative; overflow: hidden; isolation: isolate; min-height: 100svh; }
     .grid-canvas { position: absolute; inset: 0; width: 100%; height: 100%; pointer-events: none; z-index: 0; display: block; }
     .hero-fade { position: absolute; inset: 0; z-index: 1; pointer-events: none; background: linear-gradient(180deg, rgba(5,8,15,0.98) 0%, rgba(5,8,15,0.94) 18%, rgba(5,8,15,0.78) 36%, rgba(5,8,15,0.22) 62%, rgba(5,8,15,0.08) 78%, rgba(5,8,15,0.4) 100%); }
     .hero-content { position: relative; z-index: 2; }

2. ANIMACIONES GSAP (gsap.min.js + ScrollTrigger.min.js via CDN)
   - Carga: <script src="https://cdn.jsdelivr.net/npm/gsap@3.14.2/dist/gsap.min.js"></script> y ScrollTrigger equivalente.
   - gsap.defaults({ ease: "power3.out", duration: 1.2 }).
   - Usa gsap.matchMedia() para detectar "(prefers-reduced-motion: reduce)". Si coincide => limpia las props de animación (clearProps) en todos los targets y NO corre tweens/timelines.
   - ENTRADA DEL HERO (gsap.timeline, NO .play() automático fuera del matchMedia, o sí, pero respetando reduceMotion):
     • kicker/badge: y:14, opacity:0, duration:1.1s
     • h1 partido en 3 <span class="line-part">: from(y:52, opacity:0, duration:1.35s, ease:"expo.out", stagger:{each:0.14})
     • párrafo hero-body: y:24, opacity:0, duration:1.1s
     • botones CTA: y:18, opacity:0, stagger:0.18, duration:1.0s
     • meta strip (3 stats): stagger:0.12, duration:0.9s
     • contadores numéricos (ej. 99.99%, 120ms, 40+) con gsap textContent, snap a 0.01 (para float) o 1 (enteros), duration:2.6s, delay:1.4s
   - SCROLL REVEALS con ScrollTrigger (toggleActions: "play none none reverse"):
     • Encabezado de cada sección (eyebrow → h2 → p lead): y:28, opacity:0, stagger:0.2, duration:1.15s, start: "top 82%"
     • Tarjetas de features: y:42, opacity:0, duration:1.2s, delay i*0.14, ease:"back.out(1.4)", start:"top 85%"
     • Detail section: [eyebrow, title, lead, list] → y:30 stagger:0.2 duration:1.2; bullets del <ul> → x:-14 opacity:0 stagger 0.14; visual-block (código/imagen) → x:40 scale:0.97 opacity:0 duration:1.5s ease:"expo.out"
     • CTA section: eyebrow → title → lead → form → note: y:24 opacity:0 duration:1.1s delay i*0.18
   - MICRO-INTERACCIONES:
     • .btn-primary hover: gsap.to svg flecha → x:4, duration:0.55s ease:"power2.out"
     • .feature hover: ico → y:-4 scale:1.08 rotation:-6 duration:0.65s ease:"back.out(2)", card → borderColor resaltado 0.55s
     • ScrollTrigger al nav: tras scroll >20px agrega clase .scrolled (borde inferior + fondo blur sólido). Entrada del nav: from y:-24 opacity:0 duration:1.2s delay:0.3s
   - PARALLAX SUTIL (≥900px) con gsap.quickTo en mousemove sobre .hero-content: duration:1.6s, amplitud ±10px X / ±6px Y.

3. DIRECCIÓN DE ARTE / LANDING STRUCTURE
   - Estética: dark premium (#05080f de fondo), typografía: "Space Grotesk" 500/700 para titulares + "Inter" para cuerpo, una sola accent-color azul (#6db1ff / #4a7cff).
   - Estructura mínima de landing:
     1. Nav fijo con blur backdrop-filter y marca N (badge gradiente)
     2. Hero full-bleed 100svh: kicker + h1 "Escala sin límites." + párrafo + 2 botones (primary/ghost) + 3 KPIs + scroll-hint animado
     3. Features (grid 3 cols): 3 cards sin sombras agresivas, solo borde + glassmorphism suave
     4. Detail / Arquitectura (grid 2 cols): copy izq + bloque código/visual con glow difuso
     5. CTA final: mismo canvas de grilla invertido, formulario email + CTA
     6. Footer mínimo
   - No usar cards genéricas de SaaS, iconos decorativos de más ni nubes de logos. Cada sección tiene una sola idea.
   - Full-bleed hero: la sección corre edge-to-edge; el inner content va con max-width ~1160px padding lateral 32px.

4. ENTREGABLES
   - index.html: estructura + imports de fuentes Google + CDN de GSAP + ScrollTrigger
   - styles.css: todo el sistema visual
   - script.js: initGrid() que crea las 2 instancias del renderer + initAnimations() con todo lo de GSAP

5. CALIDAD
   - Responsive breakpoints en 960px y 600px.
   - En <960px la grid de features pasa a 1 columna, detail a 1 columna, se ocultan nav-links que no son CTA.
   - Ninguna animación de layout (width/top/left). Todo mediante transform/opacity.
   - will-change: transform solo en los elementos que lo necesitan (hero-content).

No uses librerías extra aparte de GSAP. Implementa todo en vanilla HTML/CSS/JS.
```

---

## Resumen de snippets clave (para reutilizar rápido)

### a) Instanciar la grilla en el hero

```html
<section class="hero">
  <canvas id="gridCanvas" class="grid-canvas" aria-hidden="true"></canvas>
  <div class="hero-fade"></div>
  <div class="hero-content">
    <!-- kicker, h1, body, actions, meta -->
  </div>
</section>
```

```js
createInfinitePerspectiveGridRenderer(
  document.getElementById("gridCanvas"),
  { speed: 0.18, horizonPosition: 0.58, perspective: 0.95, horizontalLineCount: 22, verticalLineCount: 13 }
);
```

### b) Timeline hero con GSAP

```js
const heroTl = gsap.timeline({ defaults: { ease: "power3.out" } });
heroTl.from(".reveal", { y: 14, opacity: 0, duration: 1.1 }, 0.1);
heroTl.from(".reveal-line .line-part", { y: 52, opacity: 0, duration: 1.35, ease: "expo.out", stagger: { each: 0.14 } }, 0.15);
heroTl.from(".reveal-body", { y: 24, opacity: 0, duration: 1.1 }, 0.6);
heroTl.from(".reveal-actions > *", { y: 18, opacity: 0, duration: 1.0, stagger: 0.18 }, 0.9);
heroTl.from(".reveal-meta > *", { y: 14, opacity: 0, duration: 0.9, stagger: 0.12 }, 1.25);
```

### c) Reduce motion guard

```js
gsap.matchMedia().add(
  { reduceMotion: "(prefers-reduced-motion: reduce)" },
  (context) => {
    if (context.conditions.reduceMotion) {
      gsap.set(".reveal, .section-reveal, .feature-reveal /* + todos los targets */", { clearProps: "opacity,transform" });
      return;
    }
    // ... correr timelines y scrollTriggers
  }
);
```

### d) Scroll reveal de sección

```js
gssap.from([eyebrow, title, lead], {
  scrollTrigger: { trigger: scope, start: "top 82%", toggleActions: "play none none reverse" },
  y: 28, opacity: 0, duration: 1.15, stagger: 0.2, ease: "power3.out",
});
```

---

## Parámetros de ajuste rápido (knobs)

- **Velocidad de la grilla** → `speed`: entre `0.05` (muy calmado) y `0.5` (muy dinámico). `0.18` es el sweet spot cinematográfico.
- **Factor global de avance** → dentro del renderer: `travel += options.speed * deltaSeconds * 0.12` (bajar el 0.12 para +lento).
- **Duraciones GSAP** → `gsap.defaults.duration`: `1.2` = calmado; `0.8` = snappy.
- **Horizonte** → `horizonPosition`: `0.58` central, `0.65` horizonte bajo (más profundidad), `0.5` horizonte alto.
- **Perspectiva** → `perspective`: `<0.8` = encajonado, `1.1+` = muy abierto tipo tron.
- **Cantidad de líneas** → `horizontalLineCount` + `verticalLineCount` afectan densidad.
