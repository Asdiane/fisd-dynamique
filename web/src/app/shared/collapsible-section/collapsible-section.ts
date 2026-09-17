import { Component, input, signal } from '@angular/core';

@Component({
  selector: 'app-collapsible-section',
  standalone: true,
  templateUrl: './collapsible-section.html'
})
export class CollapsibleSectionComponent {
  title = input.required<string>();
  open = signal(false);

  toggle(): void {
    this.open.update((value) => !value);
  }
}
