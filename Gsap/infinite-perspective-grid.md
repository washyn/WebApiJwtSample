# InfinitePerspectiveGrid

Este archivo reúne el código necesario para reutilizar la animación de grilla en perspectiva en otro proyecto React/TypeScript.

## 1. Componente `InfinitePerspectiveGrid.tsx`

```tsx
import { useEffect, useRef } from 'react';
import {
  createInfinitePerspectiveGridRenderer,
  type InfinitePerspectiveGridOptions,
} from './infinitePerspectiveGridRenderer';

export interface InfinitePerspectiveGridProps
  extends Partial<InfinitePerspectiveGridOptions> {
  className?: string;
}

export function InfinitePerspectiveGrid({
  className,
  speed = 0.42,
  lineOpacity = 0.5,
  lineWidth = 1.1,
  horizonPosition = 0.58,
  perspective = 0.92,
  horizontalLineCount = 18,
  verticalLineCount = 11,
}: InfinitePerspectiveGridProps) {
  const canvasRef = useRef<HTMLCanvasElement | null>(null);
  const controllerRef = useRef<ReturnType<typeof createInfinitePerspectiveGridRenderer> | null>(
    null,
  );

  useEffect(() => {
    if (!canvasRef.current) {
      return;
    }

    controllerRef.current = createInfinitePerspectiveGridRenderer(canvasRef.current, {
      speed,
      lineOpacity,
      lineWidth,
      horizonPosition,
      perspective,
      horizontalLineCount,
      verticalLineCount,
    });

    return () => controllerRef.current?.destroy();
  }, []);

  useEffect(() => {
    if (!controllerRef.current) {
      return;
    }

    controllerRef.current.update({
      speed,
      lineOpacity,
      lineWidth,
      horizonPosition,
      perspective,
      horizontalLineCount,
      verticalLineCount,
    });
  }, [
    speed,
    lineOpacity,
    lineWidth,
    horizonPosition,
    perspective,
    horizontalLineCount,
    verticalLineCount,
  ]);

  return (
    <canvas
      ref={canvasRef}
      className={['infinite-grid-canvas', className].filter(Boolean).join(' ')}
      aria-hidden="true"
    />
  );
}

export default InfinitePerspectiveGrid;
```

## 2. Renderer `infinitePerspectiveGridRenderer.ts`

```ts
export interface InfinitePerspectiveGridOptions {
  speed: number;
  lineOpacity: number;
  lineWidth: number;
  horizonPosition: number;
  perspective: number;
  horizontalLineCount: number;
  verticalLineCount: number;
}

export interface InfinitePerspectiveGridController {
  update: (next: Partial<InfinitePerspectiveGridOptions>) => void;
  destroy: () => void;
}

const DEFAULTS: InfinitePerspectiveGridOptions = {
  speed: 0.42,
  lineOpacity: 0.5,
  lineWidth: 1.1,
  horizonPosition: 0.58,
  perspective: 0.92,
  horizontalLineCount: 18,
  verticalLineCount: 11,
};

const clamp = (value: number, min: number, max: number) =>
  Math.min(max, Math.max(min, value));

const roundToDevicePixel = (value: number, dpr: number) =>
  Math.round(value * dpr) / dpr;

export function createInfinitePerspectiveGridRenderer(
  canvas: HTMLCanvasElement,
  initialOptions: Partial<InfinitePerspectiveGridOptions> = {},
): InfinitePerspectiveGridController {
  const ctx = canvas.getContext('2d');

  if (!ctx) {
    throw new Error('No se pudo inicializar el contexto 2D del canvas.');
  }

  let options: InfinitePerspectiveGridOptions = {
    ...DEFAULTS,
    ...initialOptions,
  };

  let width = 0;
  let height = 0;
  let dpr = 1;
  let animationFrame = 0;
  let lastTimestamp = 0;
  let travel = 0;
  let isDestroyed = false;

  const motionQuery = window.matchMedia('(prefers-reduced-motion: reduce)');

  const resize = () => {
    const bounds = canvas.getBoundingClientRect();
    dpr = Math.max(1, window.devicePixelRatio || 1);
    width = Math.max(1, bounds.width);
    height = Math.max(1, bounds.height);

    canvas.width = Math.round(width * dpr);
    canvas.height = Math.round(height * dpr);
    ctx.setTransform(dpr, 0, 0, dpr, 0, 0);
    renderFrame();
  };

  const drawVerticalLines = (
    vanishX: number,
    horizonY: number,
    bottomSpread: number,
  ) => {
    const totalLines = Math.max(2, Math.floor(options.verticalLineCount));
    const centerIndex = (totalLines - 1) / 2;

    for (let index = 0; index < totalLines; index += 1) {
      const normalized = centerIndex === 0 ? 0 : (index - centerIndex) / centerIndex;
      const eased = Math.sign(normalized) * Math.pow(Math.abs(normalized), 0.92);
      const xBottom = vanishX + eased * bottomSpread * 0.5;
      const alpha = options.lineOpacity * (0.16 + Math.abs(normalized) * 0.34);

      ctx.strokeStyle = `rgba(151, 199, 255, ${alpha.toFixed(3)})`;
      ctx.beginPath();
      ctx.moveTo(roundToDevicePixel(vanishX, dpr), roundToDevicePixel(horizonY, dpr));
      ctx.lineTo(roundToDevicePixel(xBottom, dpr), roundToDevicePixel(height + 4, dpr));
      ctx.stroke();
    }
  };

  const drawHorizontalLines = (
    vanishX: number,
    horizonY: number,
    bottomSpread: number,
  ) => {
    const count = Math.max(6, Math.floor(options.horizontalLineCount));
    const visibleDepth = Math.max(1, height - horizonY);
    const motionOffset = travel % 1;

    for (let index = 0; index < count; index += 1) {
      const progress = ((index / count) - motionOffset + 1) % 1;
      const perspectiveProgress = Math.pow(progress, 2.18);
      const y = horizonY + perspectiveProgress * visibleDepth;

      if (y <= horizonY + 1 || y > height + 2) {
        continue;
      }

      const depthFactor = clamp((y - horizonY) / Math.max(1, height - horizonY), 0, 1);
      const halfWidth = depthFactor * bottomSpread * 0.5;
      const alpha = options.lineOpacity * (0.12 + depthFactor * 0.88);

      ctx.strokeStyle = `rgba(118, 178, 255, ${alpha.toFixed(3)})`;
      ctx.beginPath();
      ctx.moveTo(
        roundToDevicePixel(vanishX - halfWidth, dpr),
        roundToDevicePixel(y, dpr),
      );
      ctx.lineTo(
        roundToDevicePixel(vanishX + halfWidth, dpr),
        roundToDevicePixel(y, dpr),
      );
      ctx.stroke();
    }
  };

  const drawHorizon = (horizonY: number) => {
    const glow = ctx.createLinearGradient(0, horizonY - 60, 0, horizonY + 40);
    glow.addColorStop(0, 'rgba(58, 92, 152, 0)');
    glow.addColorStop(0.55, `rgba(101, 153, 255, ${(options.lineOpacity * 0.22).toFixed(3)})`);
    glow.addColorStop(1, 'rgba(58, 92, 152, 0)');

    ctx.strokeStyle = glow;
    ctx.beginPath();
    ctx.moveTo(0, roundToDevicePixel(horizonY, dpr));
    ctx.lineTo(width, roundToDevicePixel(horizonY, dpr));
    ctx.stroke();
  };

  const renderFrame = () => {
    ctx.clearRect(0, 0, width, height);

    const horizonY = height * clamp(options.horizonPosition, 0.5, 0.65);
    const vanishX = width * 0.5;
    const bottomSpread = width * clamp(options.perspective, 0.55, 1.35);

    ctx.lineWidth = options.lineWidth;
    ctx.lineCap = 'round';

    drawHorizon(horizonY);
    drawVerticalLines(vanishX, horizonY, bottomSpread);
    drawHorizontalLines(vanishX, horizonY, bottomSpread);
  };

  const loop = (timestamp: number) => {
    if (isDestroyed) {
      return;
    }

    if (document.hidden || motionQuery.matches) {
      lastTimestamp = timestamp;
      renderFrame();
      return;
    }

    if (!lastTimestamp) {
      lastTimestamp = timestamp;
    }

    const deltaSeconds = Math.min((timestamp - lastTimestamp) / 1000, 0.05);
    lastTimestamp = timestamp;

    travel += options.speed * deltaSeconds * 0.32;
    renderFrame();
    animationFrame = window.requestAnimationFrame(loop);
  };

  const stop = () => {
    if (animationFrame) {
      window.cancelAnimationFrame(animationFrame);
      animationFrame = 0;
    }
  };

  const start = () => {
    stop();
    lastTimestamp = 0;
    renderFrame();

    if (!document.hidden && !motionQuery.matches) {
      animationFrame = window.requestAnimationFrame(loop);
    }
  };

  const handleVisibilityChange = () => {
    if (document.hidden) {
      stop();
      renderFrame();
      return;
    }

    start();
  };

  const handleMotionChange = () => {
    if (motionQuery.matches) {
      stop();
      renderFrame();
      return;
    }

    start();
  };

  resize();
  start();

  window.addEventListener('resize', resize);
  document.addEventListener('visibilitychange', handleVisibilityChange);
  motionQuery.addEventListener('change', handleMotionChange);

  return {
    update(next) {
      options = {
        ...options,
        ...next,
      };
      renderFrame();

      if (!document.hidden && !motionQuery.matches && !animationFrame) {
        animationFrame = window.requestAnimationFrame(loop);
      }
    },
    destroy() {
      isDestroyed = true;
      stop();
      window.removeEventListener('resize', resize);
      document.removeEventListener('visibilitychange', handleVisibilityChange);
      motionQuery.removeEventListener('change', handleMotionChange);
    },
  };
}
```

## 3. Uso en tu proyecto

```tsx
import InfinitePerspectiveGrid from './InfinitePerspectiveGrid';

export function HeroSection() {
  return (
    <section className="hero-grid-zone">
      <InfinitePerspectiveGrid
        speed={0.42}
        lineOpacity={0.52}
        lineWidth={1.12}
        horizonPosition={0.58}
        perspective={0.94}
        horizontalLineCount={22}
        verticalLineCount={13}
      />

      <div className="hero-grid-fade" />

      <div className="hero-copy">
        <h1>Título del hero</h1>
        <p>Texto principal encima de la animación.</p>
      </div>
    </section>
  );
}
```

## 4. CSS mínimo

```css
.hero-grid-zone {
  position: relative;
  overflow: hidden;
  isolation: isolate;
}

.infinite-grid-canvas {
  position: absolute;
  inset: 0;
  width: 100%;
  height: 100%;
  display: block;
  pointer-events: none;
  z-index: 0;
}

.hero-grid-fade {
  position: absolute;
  inset: 0;
  pointer-events: none;
  z-index: 1;
  background:
    linear-gradient(
      180deg,
      rgba(6, 10, 20, 0.995) 0%,
      rgba(6, 10, 20, 0.98) 14%,
      rgba(6, 10, 20, 0.9) 28%,
      rgba(6, 10, 20, 0.36) 52%,
      rgba(6, 10, 20, 0.08) 72%,
      rgba(6, 10, 20, 0.16) 100%
    );
}

.hero-copy {
  position: relative;
  z-index: 2;
}
```

## 5. Notas

- No usa librerías externas.
- Respeta `prefers-reduced-motion`.
- Pausa la animación si la pestaña no está visible.
- Maneja `devicePixelRatio` para líneas nítidas.
- Si quieres usarlo en `Next.js`, pon `"use client";` al inicio del componente.
