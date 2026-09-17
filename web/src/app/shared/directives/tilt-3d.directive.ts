import { Directive, ElementRef, HostListener, input } from '@angular/core';

const REDUCED_MOTION_QUERY = '(prefers-reduced-motion: reduce)';
const COARSE_POINTER_QUERY = '(pointer: coarse)';

@Directive({
  selector: '[appTilt3d]',
  standalone: true,
  host: {
    class: 'tilt-3d'
  }
})
export class Tilt3dDirective {
  intensity = input<number>(10, { alias: 'appTilt3d' });

  constructor(private host: ElementRef<HTMLElement>) {}

  @HostListener('mousemove', ['$event'])
  onMouseMove(event: MouseEvent): void {
    if (window.matchMedia(REDUCED_MOTION_QUERY).matches || window.matchMedia(COARSE_POINTER_QUERY).matches) return;

    const element = this.host.nativeElement;
    const rect = element.getBoundingClientRect();
    const x = (event.clientX - rect.left) / rect.width - 0.5;
    const y = (event.clientY - rect.top) / rect.height - 0.5;
    const amount = this.intensity();
    element.style.transform = `perspective(1000px) rotateX(${y * -amount}deg) rotateY(${x * amount}deg) translateY(-6px)`;
  }

  @HostListener('mouseleave')
  onMouseLeave(): void {
    this.host.nativeElement.style.transform = 'perspective(1000px) rotateX(0deg) rotateY(0deg) translateY(0)';
  }
}
