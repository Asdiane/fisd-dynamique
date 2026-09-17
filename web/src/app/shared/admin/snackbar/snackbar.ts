import { Component } from '@angular/core';
import { SnackbarService, SnackbarType } from './snackbar.service';

@Component({
  selector: 'app-snackbar',
  standalone: true,
  templateUrl: './snackbar.html'
})
export class SnackbarComponent {
  constructor(public snackbar: SnackbarService) {}

  colorClass(type: SnackbarType): string {
    switch (type) {
      case 'success':
        return 'bg-valid';
      case 'error':
        return 'bg-warn';
      case 'warning':
        return 'bg-amber-500';
      default:
        return 'bg-primary';
    }
  }
}
