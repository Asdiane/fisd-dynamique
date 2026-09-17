import { Injectable, computed, signal } from '@angular/core';

// A counter per loaderId, keyed by the caller's own id (not global to every request) - a
// call only shows the overlay if it explicitly opts in with a `loaderId` header, so
// silent/background calls (auth checks, polling...) never do.
@Injectable({ providedIn: 'root' })
export class LoadingService {
  private loaders = new Map<string, number>();
  private version = signal(0);

  readonly isLoading = computed(() => {
    this.version();
    return this.loaders.size > 0;
  });

  setLoading(loaderId: string, loading: boolean): void {
    if (!loaderId) {
      return;
    }

    const counter = this.loaders.get(loaderId) ?? 0;
    const next = loading ? counter + 1 : counter - 1;

    if (next > 0) {
      this.loaders.set(loaderId, next);
    } else {
      this.loaders.delete(loaderId);
    }
    this.version.update((v) => v + 1);
  }
}
