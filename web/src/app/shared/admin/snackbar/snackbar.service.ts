import { Injectable, signal } from '@angular/core';

export type SnackbarType = 'success' | 'error' | 'info' | 'warning';

export interface SnackbarMessage {
  id: string;
  type: SnackbarType;
  text: string;
}

const DEFAULT_DURATION_MS = 6000;

@Injectable({ providedIn: 'root' })
export class SnackbarService {
  messages = signal<SnackbarMessage[]>([]);

  success(text: string, duration = DEFAULT_DURATION_MS): void {
    this.push('success', text, duration);
  }

  error(text: string, duration = DEFAULT_DURATION_MS): void {
    this.push('error', text, duration);
  }

  info(text: string, duration = DEFAULT_DURATION_MS): void {
    this.push('info', text, duration);
  }

  warning(text: string, duration = DEFAULT_DURATION_MS): void {
    this.push('warning', text, duration);
  }

  dismiss(id: string): void {
    this.messages.update((messages) => messages.filter((m) => m.id !== id));
  }

  private push(type: SnackbarType, text: string, duration: number): void {
    const id = crypto.randomUUID();
    this.messages.update((messages) => [...messages, { id, type, text }]);
    setTimeout(() => this.dismiss(id), duration);
  }
}
