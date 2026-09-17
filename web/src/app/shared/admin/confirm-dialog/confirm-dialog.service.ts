import { Injectable, signal } from '@angular/core';

export interface ConfirmDialogOptions {
  title: string;
  message: string;
  confirmLabel?: string;
  cancelLabel?: string;
  variant?: 'danger' | 'primary';
}

interface ConfirmDialogState extends ConfirmDialogOptions {
  resolve: (confirmed: boolean) => void;
}

@Injectable({ providedIn: 'root' })
export class ConfirmDialogService {
  state = signal<ConfirmDialogState | null>(null);

  confirm(options: ConfirmDialogOptions): Promise<boolean> {
    return new Promise((resolve) => {
      this.state.set({ ...options, resolve });
    });
  }

  respond(confirmed: boolean): void {
    this.state()?.resolve(confirmed);
    this.state.set(null);
  }
}
