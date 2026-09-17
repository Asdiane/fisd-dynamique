import { Component, input } from '@angular/core';

@Component({
  selector: 'app-admin-page-header',
  standalone: true,
  templateUrl: './admin-page-header.html'
})
export class AdminPageHeaderComponent {
  eyebrow = input.required<string>();
  title = input.required<string>();
  subtitle = input<string>();
}
