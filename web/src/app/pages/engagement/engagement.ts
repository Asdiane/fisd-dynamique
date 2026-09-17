import { Component, OnInit, signal } from '@angular/core';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { PageHeroComponent } from '../../shared/page-hero/page-hero';
import { SectionTitleComponent } from '../../shared/section-title/section-title';
import { PartnersWallComponent } from '../../shared/partners-wall/partners-wall';
import { ScrollRevealDirective } from '../../shared/directives/scroll-reveal.directive';
import {
  ENGAGEMENT_ACTIONS,
  PARTNERS,
  EngagementAction as StaticEngagementAction,
  Partner as StaticPartner
} from '../../core/content/site-content';
import { EngagementActionsService } from '../../core/engagement-actions.service';
import { PartnersService } from '../../core/partners.service';

interface EngagementStep {
  number: string;
  titleKey: string;
  textKey: string;
}

@Component({
  selector: 'app-engagement',
  standalone: true,
  imports: [TranslatePipe, PageHeroComponent, SectionTitleComponent, PartnersWallComponent, ScrollRevealDirective],
  templateUrl: './engagement.html'
})
export class EngagementComponent implements OnInit {
  actions = signal<StaticEngagementAction[]>(ENGAGEMENT_ACTIONS);
  partners = signal<StaticPartner[]>(PARTNERS);
  steps: EngagementStep[] = [
    { number: '1', titleKey: 'Engagement.Step1Title', textKey: 'Engagement.Step1Text' },
    { number: '2', titleKey: 'Engagement.Step2Title', textKey: 'Engagement.Step2Text' },
    { number: '3', titleKey: 'Engagement.Step3Title', textKey: 'Engagement.Step3Text' }
  ];

  constructor(
    private translate: TranslateService,
    private engagementActionsService: EngagementActionsService,
    private partnersService: PartnersService
  ) {}

  ngOnInit(): void {
    // Static content above renders immediately - the API only overrides it once loaded, and any
    // failure keeps whatever is already showing (never a blank page).
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

    this.partnersService.getVisible().subscribe({
      next: (partners) => {
        if (partners.length === 0) return;
        this.partners.set(partners.map((p) => ({ name: p.name, label: p.label ?? undefined, logo: p.logoUrl })));
      },
      error: () => undefined
    });
  }

  highlightLabel(index: number): string {
    const keys = ['Engagement.HighlightPartners', 'Engagement.HighlightExhibitors', 'Engagement.HighlightVolunteers'];
    return this.translate.instant(keys[index] ?? keys[0]);
  }
}
