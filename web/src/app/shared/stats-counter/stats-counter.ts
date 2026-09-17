import { Component, ElementRef, OnDestroy, AfterViewInit, input, signal } from '@angular/core';
import { NgTemplateOutlet } from '@angular/common';
import { RouterLink } from '@angular/router';
import { TranslatePipe } from '@ngx-translate/core';

const REDUCED_MOTION_QUERY = '(prefers-reduced-motion: reduce)';
const COUNT_DURATION_MS = 1600;

export interface StatItem {
  value: number;
  suffix?: string;
  labelKey: string;
  link?: string;
  fragment?: string;
}

@Component({
  selector: 'app-stats-counter',
  standalone: true,
  imports: [TranslatePipe, RouterLink, NgTemplateOutlet],
  templateUrl: './stats-counter.html',
  styleUrl: './stats-counter.scss'
})
export class StatsCounterComponent implements AfterViewInit, OnDestroy {
  stats = input<StatItem[]>([]);
  displayValues = signal<number[]>([]);

  private observer?: IntersectionObserver;
  private frame?: number;
  private started = false;

  constructor(private host: ElementRef<HTMLElement>) {}

  ngAfterViewInit(): void {
    this.displayValues.set(this.stats().map(() => 0));

    if (window.matchMedia(REDUCED_MOTION_QUERY).matches) {
      this.displayValues.set(this.stats().map((s) => s.value));
      return;
    }

    this.observer = new IntersectionObserver(
      (entries) => {
        for (const entry of entries) {
          if (entry.isIntersecting && !this.started) {
            this.started = true;
            this.runCountUp();
            this.observer?.disconnect();
          }
        }
      },
      { threshold: 0.35 }
    );
    this.observer.observe(this.host.nativeElement);
  }

  ngOnDestroy(): void {
    this.observer?.disconnect();
    if (this.frame) cancelAnimationFrame(this.frame);
  }

  private runCountUp(): void {
    const targets = this.stats().map((s) => s.value);
    const start = performance.now();

    const tick = (now: number) => {
      const elapsed = now - start;
      const progress = Math.min(elapsed / COUNT_DURATION_MS, 1);
      const eased = 1 - Math.pow(1 - progress, 3);
      this.displayValues.set(targets.map((target) => Math.round(target * eased)));

      if (progress < 1) {
        this.frame = requestAnimationFrame(tick);
      }
    };
    this.frame = requestAnimationFrame(tick);
  }

  onCardMove(event: MouseEvent, card: HTMLElement): void {
    if (window.matchMedia(REDUCED_MOTION_QUERY).matches || window.matchMedia('(pointer: coarse)').matches) return;

    const rect = card.getBoundingClientRect();
    const x = (event.clientX - rect.left) / rect.width - 0.5;
    const y = (event.clientY - rect.top) / rect.height - 0.5;
    card.style.transform = `perspective(900px) rotateX(${y * -14}deg) rotateY(${x * 16}deg) translateY(-4px)`;
  }

  onCardLeave(card: HTMLElement): void {
    card.style.transform = 'perspective(900px) rotateX(0deg) rotateY(0deg) translateY(0)';
  }
}
