import { Component, input } from '@angular/core';

@Component({
  selector: 'app-section-title',
  standalone: true,
  templateUrl: './section-title.html'
})
export class SectionTitleComponent {
  eyebrow = input.required<string>();
  title = input.required<string>();
  description = input<string>('');
  light = input(false);
}
