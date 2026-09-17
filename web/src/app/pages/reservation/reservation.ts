import { Component, OnInit, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { TranslatePipe } from '@ngx-translate/core';
import { PageHeroComponent } from '../../shared/page-hero/page-hero';
import { ScrollRevealDirective } from '../../shared/directives/scroll-reveal.directive';
import { ENGAGEMENT_ACTIONS, ZEFFY_URL, EngagementAction as StaticEngagementAction } from '../../core/content/site-content';
import { EditionsService } from '../../core/editions.service';
import { EngagementActionsService } from '../../core/engagement-actions.service';

@Component({
  selector: 'app-reservation',
  standalone: true,
  imports: [RouterLink, TranslatePipe, PageHeroComponent, ScrollRevealDirective],
  templateUrl: './reservation.html'
})
export class ReservationComponent implements OnInit {
  actions = signal<StaticEngagementAction[]>(ENGAGEMENT_ACTIONS);
  zeffyUrl = signal(ZEFFY_URL);
  nextEditionName = signal<string | null>(null);
  nextEditionPlace = signal<string | null>(null);

  constructor(
    private editionsService: EditionsService,
    private engagementActionsService: EngagementActionsService
  ) {}

  ngOnInit(): void {
    this.editionsService.resolveCurrentTicketingUrl(ZEFFY_URL).subscribe((url) => this.zeffyUrl.set(url));

    // Falls back to the static i18n copy (Reservation.NextEditionName/Place) until the real
    // current edition loads - once it does, this never goes stale when a new edition is created,
    // unlike a hardcoded translation string.
    this.editionsService.getCurrentEdition().subscribe({
      next: (edition) => {
        if (!edition) return;
        this.nextEditionName.set(`${edition.badge}  ${edition.title}`);
        this.nextEditionPlace.set(edition.locationLabel ?? null);
      },
      error: () => undefined
    });

    this.engagementActionsService.getVisible().subscribe({
      next: (actions) => {
        if (actions.length === 0) return;
        this.actions.set(
          actions.map((a) => ({
            anchor: a.anchor,
            title: a.title,
            text: a.text,
            icon: a.icon,
            image: a.imageUrl,
            link: a.link,
            cta: a.cta,
            detail: a.detail
          }))
        );
      },
      error: () => undefined
    });
  }
}
