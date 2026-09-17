import { Component, OnDestroy, OnInit, computed, input, signal } from '@angular/core';
import { TranslatePipe } from '@ngx-translate/core';

@Component({
  selector: 'app-countdown',
  standalone: true,
  imports: [TranslatePipe],
  templateUrl: './countdown.html',
  styleUrl: './countdown.scss'
})
export class CountdownComponent implements OnInit, OnDestroy {
  targetDate = input<string | null>(null);

  private now = signal(Date.now());
  private timer?: ReturnType<typeof setInterval>;

  private remainingMs = computed(() => {
    const target = this.targetDate();
    if (!target) return 0;
    return Math.max(0, new Date(target).getTime() - this.now());
  });

  hasEvent = computed(() => !!this.targetDate());
  isPast = computed(() => this.hasEvent() && this.remainingMs() <= 0);

  private days = computed(() => Math.floor(this.remainingMs() / 86_400_000));
  private hours = computed(() => Math.floor((this.remainingMs() % 86_400_000) / 3_600_000));
  private minutes = computed(() => Math.floor((this.remainingMs() % 3_600_000) / 60_000));
  private seconds = computed(() => Math.floor((this.remainingMs() % 60_000) / 1000));

  readonly units = [
    { key: 'days', labelKey: 'Countdown.Days', value: this.days },
    { key: 'hours', labelKey: 'Countdown.Hours', value: this.hours },
    { key: 'minutes', labelKey: 'Countdown.Minutes', value: this.minutes },
    { key: 'seconds', labelKey: 'Countdown.Seconds', value: this.seconds }
  ];

  ngOnInit(): void {
    this.timer = setInterval(() => this.now.set(Date.now()), 1000);
  }

  ngOnDestroy(): void {
    clearInterval(this.timer);
  }

  pad(value: number): string {
    return value.toString().padStart(2, '0');
  }
}
