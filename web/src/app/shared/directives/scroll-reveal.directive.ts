import { Directive, ElementRef, OnDestroy, OnInit, input } from '@angular/core';

const REDUCED_MOTION_QUERY = '(prefers-reduced-motion: reduce)';

export type ScrollRevealVariant = 'up' | 'zoom' | 'left' | 'right' | 'flip';

@Directive({
  selector: '[appScrollReveal]',
  standalone: true,
  host: {
    class: 'scroll-reveal'
  }
})
export class ScrollRevealDirective implements OnInit, OnDestroy {
  delayMs = input<number>(0, { alias: 'appScrollReveal' });
  variant = input<ScrollRevealVariant>('up', { alias: 'appScrollRevealVariant' });
  private observer?: IntersectionObserver;

  constructor(private host: ElementRef<HTMLElement>) {}

  ngOnInit(): void {
    const element = this.host.nativeElement;
    element.classList.add(`scroll-reveal--${this.variant()}`);

    if (window.matchMedia(REDUCED_MOTION_QUERY).matches) {
      element.classList.add('scroll-reveal-visible');
      return;
    }

    element.style.transitionDelay = `${this.delayMs()}ms`;

    this.observer = new IntersectionObserver(
      (entries) => {
        for (const entry of entries) {
          if (entry.isIntersecting) {
            element.classList.add('scroll-reveal-visible');
            this.observer?.unobserve(element);
          }
        }
      },
      { threshold: 0.15, rootMargin: '0px 0px -60px 0px' }
    );
    this.observer.observe(element);
  }

  ngOnDestroy(): void {
    this.observer?.disconnect();
  }
}
