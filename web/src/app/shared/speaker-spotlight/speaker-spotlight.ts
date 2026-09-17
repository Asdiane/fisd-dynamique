import { Component, OnDestroy, OnInit, input, signal } from '@angular/core';
import { Speaker } from '../../core/content/site-content';
import { Tilt3dDirective } from '../directives/tilt-3d.directive';

const ROTATION_INTERVAL_MS = 4200;

@Component({
  selector: 'app-speaker-spotlight',
  standalone: true,
  imports: [Tilt3dDirective],
  templateUrl: './speaker-spotlight.html'
})
export class SpeakerSpotlightComponent implements OnInit, OnDestroy {
  speakers = input.required<Speaker[]>();
  activeIndex = signal(0);
  private rotationTimer?: ReturnType<typeof setInterval>;

  get activeSpeaker(): Speaker {
    return this.speakers()[this.activeIndex()] ?? this.speakers()[0];
  }

  ngOnInit(): void {
    this.rotationTimer = setInterval(() => {
      this.activeIndex.update((index) => (index + 1) % this.speakers().length);
    }, ROTATION_INTERVAL_MS);
  }

  ngOnDestroy(): void {
    clearInterval(this.rotationTimer);
  }

  previous(): void {
    this.activeIndex.update((index) => (index - 1 + this.speakers().length) % this.speakers().length);
  }

  next(): void {
    this.activeIndex.update((index) => (index + 1) % this.speakers().length);
  }

  select(index: number): void {
    this.activeIndex.set(index);
  }
}
