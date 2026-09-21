(function () {
  "use strict";

  const DEFAULTS = {
    speed: 0.42,
    lineOpacity: 0.5,
    lineWidth: 1.1,
    horizonPosition: 0.58,
    perspective: 0.92,
    horizontalLineCount: 18,
    verticalLineCount: 11,
  };

  const clamp = (value, min, max) => Math.min(max, Math.max(min, value));
  const roundToDevicePixel = (value, dpr) => Math.round(value * dpr) / dpr;

  function createInfinitePerspectiveGridRenderer(canvas, initialOptions) {
    const ctx = canvas.getContext("2d");
    if (!ctx) {
      throw new Error("No se pudo inicializar el contexto 2D del canvas.");
    }

    let options = Object.assign({}, DEFAULTS, initialOptions || {});
    let width = 0;
    let height = 0;
    let dpr = 1;
    let animationFrame = 0;
    let lastTimestamp = 0;
    let travel = 0;
    let isDestroyed = false;

    const motionQuery = window.matchMedia("(prefers-reduced-motion: reduce)");

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

    const drawVerticalLines = (vanishX, horizonY, bottomSpread) => {
      const totalLines = Math.max(2, Math.floor(options.verticalLineCount));
      const centerIndex = (totalLines - 1) / 2;

      for (let index = 0; index < totalLines; index += 1) {
        const normalized = centerIndex === 0 ? 0 : (index - centerIndex) / centerIndex;
        const eased = Math.sign(normalized) * Math.pow(Math.abs(normalized), 0.92);
        const xBottom = vanishX + eased * bottomSpread * 0.5;
        const alpha = options.lineOpacity * (0.16 + Math.abs(normalized) * 0.34);

        ctx.strokeStyle = `rgba(151, 199, 255, ${alpha.toFixed(3)})`;
        ctx.lineWidth = options.lineWidth;
        ctx.lineCap = "round";
        ctx.beginPath();
        ctx.moveTo(roundToDevicePixel(vanishX, dpr), roundToDevicePixel(horizonY, dpr));
        ctx.lineTo(roundToDevicePixel(xBottom, dpr), roundToDevicePixel(height + 4, dpr));
        ctx.stroke();
      }
    };

    const drawHorizontalLines = (vanishX, horizonY, bottomSpread) => {
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
        ctx.lineWidth = options.lineWidth;
        ctx.lineCap = "round";
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

    const drawHorizon = (horizonY) => {
      const glow = ctx.createLinearGradient(0, horizonY - 60, 0, horizonY + 40);
      glow.addColorStop(0, "rgba(58, 92, 152, 0)");
      glow.addColorStop(0.55, `rgba(101, 153, 255, ${(options.lineOpacity * 0.22).toFixed(3)})`);
      glow.addColorStop(1, "rgba(58, 92, 152, 0)");

      ctx.strokeStyle = glow;
      ctx.lineWidth = options.lineWidth;
      ctx.lineCap = "round";
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

      drawHorizon(horizonY);
      drawVerticalLines(vanishX, horizonY, bottomSpread);
      drawHorizontalLines(vanishX, horizonY, bottomSpread);
    };

    const loop = (timestamp) => {
      if (isDestroyed) return;

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

      travel += options.speed * deltaSeconds * 0.12;
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

    window.addEventListener("resize", resize);
    document.addEventListener("visibilitychange", handleVisibilityChange);
    if (motionQuery.addEventListener) {
      motionQuery.addEventListener("change", handleMotionChange);
    } else if (motionQuery.addListener) {
      motionQuery.addListener(handleMotionChange);
    }

    return {
      update(next) {
        options = Object.assign({}, options, next || {});
        renderFrame();

        if (!document.hidden && !motionQuery.matches && !animationFrame) {
          animationFrame = window.requestAnimationFrame(loop);
        }
      },
      destroy() {
        isDestroyed = true;
        stop();
        window.removeEventListener("resize", resize);
        document.removeEventListener("visibilitychange", handleVisibilityChange);
        if (motionQuery.removeEventListener) {
          motionQuery.removeEventListener("change", handleMotionChange);
        } else if (motionQuery.removeListener) {
          motionQuery.removeListener(handleMotionChange);
        }
      },
    };
  }

  function initGrid() {
    const gridControllers = [];
    const topCanvas = document.getElementById("gridCanvas");
    const bottomCanvas = document.getElementById("gridCanvasBottom");

    if (topCanvas) {
      gridControllers.push(createInfinitePerspectiveGridRenderer(topCanvas, {
        speed: 0.18,
        lineOpacity: 0.5,
        lineWidth: 1.1,
        horizonPosition: 0.58,
        perspective: 0.95,
        horizontalLineCount: 22,
        verticalLineCount: 13,
      }));
    }

    if (bottomCanvas) {
      gridControllers.push(createInfinitePerspectiveGridRenderer(bottomCanvas, {
        speed: 0.1,
        lineOpacity: 0.42,
        lineWidth: 1.0,
        horizonPosition: 0.62,
        perspective: 1.0,
        horizontalLineCount: 16,
        verticalLineCount: 11,
      }));
    }

    return gridControllers;
  }

  function initAnimations() {
    if (!window.gsap) return;

    gsap.registerPlugin(window.ScrollTrigger);

    const reduceMotion = window.matchMedia("(prefers-reduced-motion: reduce)").matches;

    gsap.defaults({
      ease: "power3.out",
      duration: 0.8,
    });

    const mm = gsap.matchMedia();

    mm.add({ reduceMotion: "(prefers-reduced-motion: reduce)" }, (context) => {
      const { reduceMotion: rm } = context.conditions;

      if (rm) {
        gsap.set(".reveal, .reveal-line, .reveal-body, .reveal-actions, .reveal-meta, .section-reveal, .section-reveal-title, .section-reveal-lead, .feature-reveal, .detail-reveal, .detail-reveal-title, .detail-reveal-lead, .detail-reveal-list, .detail-reveal-visual, .cta-reveal, .cta-reveal-title, .cta-reveal-lead, .cta-reveal-form, .cta-reveal-note", {
          clearProps: "opacity,transform",
        });
        return;
      }

      const heroTl = gsap.timeline({ defaults: { ease: "power3.out" } });

      heroTl.from(".reveal", {
        y: 14,
        opacity: 0,
        duration: 0.7,
      }, 0.1);

      heroTl.from(".reveal-line .line-part", {
        y: 52,
        opacity: 0,
        duration: 0.95,
        ease: "expo.out",
        stagger: { each: 0.08, from: "start" },
      }, 0.15);

      heroTl.from(".reveal-body", {
        y: 24,
        opacity: 0,
        duration: 0.75,
      }, 0.45);

      heroTl.from(".reveal-actions > *", {
        y: 18,
        opacity: 0,
        duration: 0.7,
        stagger: 0.1,
      }, 0.6);

      heroTl.from(".reveal-meta > *", {
        y: 14,
        opacity: 0,
        duration: 0.6,
        stagger: 0.06,
      }, 0.8);

      document.querySelectorAll(".meta-num").forEach((el) => {
        const target = parseFloat(el.dataset.count || "0");
        const isFloat = target % 1 !== 0;
        gsap.fromTo(
          el,
          { textContent: 0 },
          {
            textContent: target,
            duration: 1.6,
            ease: "power2.out",
            delay: 0.9,
            snap: isFloat ? 0.01 : 1,
            modifiers: {
              textContent: (value) => {
                const num = parseFloat(value);
                if (isFloat) return num.toFixed(2);
                return Math.round(num).toString();
              },
            },
          },
        );
      });

      gsap.to(".scroll-hint", {
        y: 6,
        opacity: 0.6,
        duration: 1.8,
        ease: "sine.inOut",
        yoyo: true,
        repeat: -1,
      });

      const createSectionReveal = (scope) => {
        const eyebrow = scope.querySelector(":scope > .section-reveal, :scope > .eyebrow");
        const title = scope.querySelector(":scope > .section-reveal-title, :scope > .section-title");
        const lead = scope.querySelector(":scope > .section-reveal-lead, :scope > .section-lead");

        const items = [eyebrow, title, lead].filter(Boolean);
        if (!items.length) return;

        gsap.from(items, {
          scrollTrigger: {
            trigger: scope,
            start: "top 82%",
            toggleActions: "play none none reverse",
          },
          y: 28,
          opacity: 0,
          duration: 0.8,
          stagger: 0.12,
          ease: "power3.out",
        });
      };

      document.querySelectorAll(".features-head").forEach(createSectionReveal);

      document.querySelectorAll(".feature-reveal").forEach((el, i) => {
        gsap.from(el, {
          scrollTrigger: {
            trigger: el,
            start: "top 85%",
            toggleActions: "play none none reverse",
          },
          y: 42,
          opacity: 0,
          duration: 0.85,
          delay: i * 0.08,
          ease: "back.out(1.6)",
        });
      });

      gsap.utils.toArray(".detail-reveal, .detail-reveal-title, .detail-reveal-lead").forEach((el, i, targets) => {
        if (i > 0 && targets[i - 1] === el) return;
      });

      const dc = document.querySelector(".detail-copy");
      if (dc) {
        const dEyebrow = dc.querySelector(".detail-reveal");
        const dTitle = dc.querySelector(".detail-reveal-title");
        const dLead = dc.querySelector(".detail-reveal-lead");
        const dList = dc.querySelector(".detail-reveal-list");
        const dVisual = document.querySelector(".detail-reveal-visual");

        const items = [dEyebrow, dTitle, dLead, dList].filter(Boolean);

        gsap.from(items, {
          scrollTrigger: {
            trigger: document.querySelector(".detail"),
            start: "top 78%",
            toggleActions: "play none none reverse",
          },
          y: 30,
          opacity: 0,
          duration: 0.85,
          stagger: 0.12,
          ease: "power3.out",
        });

        if (dList) {
          gsap.from(dList.querySelectorAll("li"), {
            scrollTrigger: {
              trigger: dList,
              start: "top 88%",
              toggleActions: "play none none reverse",
            },
            x: -14,
            opacity: 0,
            duration: 0.5,
            stagger: 0.08,
            ease: "power2.out",
            delay: 0.3,
          });
        }

        if (dVisual) {
          gsap.from(dVisual, {
            scrollTrigger: {
              trigger: document.querySelector(".detail"),
              start: "top 78%",
              toggleActions: "play none none reverse",
            },
            x: 40,
            opacity: 0,
            scale: 0.97,
            duration: 1,
            ease: "expo.out",
          });
        }
      }

      const ctaItems = [
        ".cta-reveal",
        ".cta-reveal-title",
        ".cta-reveal-lead",
        ".cta-reveal-form",
        ".cta-reveal-note",
      ];
      ctaItems.forEach((sel, i) => {
        const el = document.querySelector(sel);
        if (!el) return;
        gsap.from(el, {
          scrollTrigger: {
            trigger: el,
            start: "top 88%",
            toggleActions: "play none none reverse",
          },
          y: 24,
          opacity: 0,
          duration: 0.8,
          delay: i * 0.1,
          ease: "power3.out",
        });
      });

      document.querySelectorAll(".btn-primary").forEach((btn) => {
        btn.addEventListener("mouseenter", () => {
          gsap.to(btn.querySelectorAll("svg"), {
            x: 4,
            duration: 0.3,
            ease: "power2.out",
          });
        });
        btn.addEventListener("mouseleave", () => {
          gsap.to(btn.querySelectorAll("svg"), {
            x: 0,
            duration: 0.3,
            ease: "power2.out",
          });
        });
      });

      document.querySelectorAll(".feature").forEach((card) => {
        const ico = card.querySelector(".feature-ico");
        if (!ico) return;
        card.addEventListener("mouseenter", () => {
          gsap.to(ico, {
            y: -4,
            scale: 1.08,
            rotation: -6,
            duration: 0.4,
            ease: "back.out(2)",
          });
          gsap.to(card, {
            borderColor: "rgba(109, 177, 255, 0.3)",
            backgroundColor: "rgba(20, 28, 48, 0.6)",
            duration: 0.35,
          });
        });
        card.addEventListener("mouseleave", () => {
          gsap.to(ico, {
            y: 0,
            scale: 1,
            rotation: 0,
            duration: 0.35,
            ease: "power2.out",
          });
          gsap.to(card, {
            borderColor: "rgba(120, 160, 220, 0.12)",
            backgroundColor: "rgba(20, 28, 48, 0)",
            duration: 0.35,
            clearProps: "backgroundColor",
          });
        });
      });

      const navEl = document.querySelector(".nav");
      if (navEl) {
        ScrollTrigger.create({
          start: "top -40",
          onUpdate: (self) => {
            if (self.scroll() > 20) {
              navEl.classList.add("scrolled");
            } else {
              navEl.classList.remove("scrolled");
            }
          },
        });

        gsap.from(navEl, {
          y: -24,
          opacity: 0,
          duration: 0.8,
          ease: "power3.out",
          delay: 0.2,
        });
      }

      const heroContent = document.querySelector(".hero-content");
      if (heroContent && window.matchMedia("(min-width: 900px)").matches) {
        const xTo = gsap.quickTo(heroContent, "x", { duration: 0.9, ease: "power2.out" });
        const yTo = gsap.quickTo(heroContent, "y", { duration: 0.9, ease: "power2.out" });

        window.addEventListener("mousemove", (e) => {
          const x = (e.clientX / window.innerWidth - 0.5) * 12;
          const y = (e.clientY / window.innerHeight - 0.5) * 8;
          xTo(x);
          yTo(y);
        });
      }
    });
  }

  document.addEventListener("DOMContentLoaded", () => {
    try {
      initGrid();
    } catch (e) {
      console.warn("Grid render error:", e);
    }
    initAnimations();
  });
})();
