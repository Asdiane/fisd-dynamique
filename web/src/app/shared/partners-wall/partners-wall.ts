import { Component, computed, input } from '@angular/core';
import { Partner } from '../../core/content/site-content';

const SECONDS_PER_LOGO = 2.6;

@Component({
  selector: 'app-partners-wall',
  standalone: true,
  templateUrl: './partners-wall.html',
  styleUrl: './partners-wall.scss'
})
export class PartnersWallComponent {
  partners = input.required<Partner[]>();

  // Longer logo lists need a longer loop to keep the scroll speed constant instead of
  // whipping past faster as more partners are added.
  protected readonly marqueeDuration = computed(() => `${Math.max(18, this.partners().length * SECONDS_PER_LOGO)}s`);
}
