import { Component, input } from '@angular/core';

@Component({
  selector: 'app-page-hero',
  standalone: true,
  templateUrl: './page-hero.html'
})
export class PageHeroComponent {
  eyebrow = input.required<string>();
  title = input.required<string>();
  description = input.required<string>();
  image = input<string | null>(null);
}
