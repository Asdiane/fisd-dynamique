import { Component, input } from '@angular/core';

@Component({
  selector: 'app-admin-info-card',
  standalone: true,
  templateUrl: './admin-info-card.html'
})
export class AdminInfoCardComponent {
  title = input.required<string>();
  description = input.required<string>();
}
