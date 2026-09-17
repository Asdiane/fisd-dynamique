import { EditorialService } from '../../core/editorial.service';
import { TestimonialsService } from '../../core/testimonials.service';
import { STATIC_TESTIMONIALS } from '../../core/content/site-content';
import { Component, OnDestroy, OnInit, computed, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { SectionTitleComponent } from '../../shared/section-title/section-title';
import { PartnersWallComponent } from '../../shared/partners-wall/partners-wall';
import { SpeakerSpotlightComponent } from '../../shared/speaker-spotlight/speaker-spotlight';
import { ScrollRevealDirective } from '../../shared/directives/scroll-reveal.directive';
import { Tilt3dDirective } from '../../shared/directives/tilt-3d.directive';
import { CountdownComponent } from '../../shared/countdown/countdown';
import { TypewriterComponent } from '../../shared/typewriter/typewriter';
import { StatsCounterComponent, StatItem } from '../../shared/stats-counter/stats-counter';
import {
  SLIDES,
  PILLARS,
  PARTICIPANTS,
  PARTNERS,
  SPEAKERS,
  Slide as StaticSlide,
  Pillar as StaticPillar,
  Participant as StaticParticipant,
  Partner as StaticPartner,
  Speaker as StaticSpeaker,
} from '../../core/content/site-content';
import { SlidesService } from '../../core/slides.service';
import { PillarsService } from '../../core/pillars.service';
import { ParticipantsService } from '../../core/participants.service';
import { PartnersService } from '../../core/partners.service';
import { SpeakersService } from '../../core/speakers.service';
import { EditionsService } from '../../core/editions.service';
import { SiteSettingsService } from '../../core/site-settings.service';

const DEFAULT_SLIDE_DURATION_SECONDS = 5;
const STATS_BG_INTERVAL_MS = 5000;
const STATS_BG_IMAGES = [
  'assets/images/slider-panel-large.jpg',
  'assets/images/concertations.jpeg',
  'assets/images/fisd-est-visibilite.jpg',
  'assets/images/plaidoyer-actions-citoyennes.jpeg',
];

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [
    RouterLink,
    TranslatePipe,
    SectionTitleComponent,
    PartnersWallComponent,
    SpeakerSpotlightComponent,
    ScrollRevealDirective,
    Tilt3dDirective,
    CountdownComponent,
    TypewriterComponent,
  ],
  templateUrl: './home.html',
  styleUrl: './home.scss',
})
export class HomeComponent implements OnInit, OnDestroy {
  readonly testimonials =
    signal<
      { authorName: string; authorRole: string | null; content: string; imageUrl?: string | null }[]
    >(STATIC_TESTIMONIALS);
  readonly pillarImages = [
    'assets/images/fisd-est-visibilite.jpg',
    'assets/images/concertations.jpeg',
    'assets/images/plaidoyer-actions-citoyennes.jpeg',
  ];
  slides = signal<StaticSlide[]>(SLIDES);
  pillars = signal<StaticPillar[]>(PILLARS);
  participants = signal<StaticParticipant[]>(PARTICIPANTS);
  partners = signal<StaticPartner[]>(PARTNERS);
  speakers = signal<StaticSpeaker[]>(SPEAKERS);
  activeSlide = signal(0);
  presentationExpanded = signal(false);
  countdownDate = signal<string | null>('2026-11-26T00:00:00');
  editionsCount = signal(3);
  readonly statsBgImages = STATS_BG_IMAGES;
  statsBgIndex = signal(0);
  private slideTimer?: ReturnType<typeof setInterval>;
  private statsBgTimer?: ReturnType<typeof setInterval>;

  readonly taglinePhrases = PILLARS.map(
    (pillar) => pillar.title.charAt(0).toLowerCase() + pillar.title.slice(1) + '.',
  );

  readonly stats = computed<StatItem[]>(() => {
    const items: StatItem[] = [
      { value: this.editionsCount(), labelKey: 'Home.StatsEditions', link: '/programmation' },
      {
        value: this.partners().length,
        labelKey: 'Home.StatsPartners',
        link: '/',
        fragment: 'partners',
      },
      {
        value: this.pillars().length,
        labelKey: 'Home.StatsPillars',
        link: '/',
        fragment: 'objectifs',
      },
      {
        value: this.participants().length,
        labelKey: 'Home.StatsParticipants',
        link: '/',
        fragment: 'participants',
      },
      {
        value: Math.max(this.speakers().length, SPEAKERS.length),
        labelKey: 'Home.StatsSpeakers',
        link: '/',
        fragment: 'speakers',
      },
    ];
    return items.filter((item) => item.value > 0);
  });

  readonly statsHeadlinePhrases = computed<string[]>(() => {
    const phrases = [this.translate.instant('Home.StatsTitle')];
    if (this.partners().length > 0) {
      phrases.push(
        this.translate.instant('Home.StatsHeadlinePartners', { count: this.partners().length }),
      );
    }
    if (this.editionsCount() > 0) {
      phrases.push(
        this.translate.instant('Home.StatsHeadlineEditions', { count: this.editionsCount() }),
      );
    }
    if (this.pillars().length > 0) {
      phrases.push(
        this.translate.instant('Home.StatsHeadlineAxes', { count: this.pillars().length }),
      );
    }
    return phrases;
  });

  constructor(
    public editorial: EditorialService,
    private testimonialsService: TestimonialsService,
    private translate: TranslateService,
    private slidesService: SlidesService,
    private pillarsService: PillarsService,
    private participantsService: ParticipantsService,
    private partnersService: PartnersService,
    private speakersService: SpeakersService,
    private editionsService: EditionsService,
    private siteSettingsService: SiteSettingsService,
  ) {}

  ngOnInit(): void {
    this.testimonialsService.getVisible().subscribe({
      next: (items) => {
        this.testimonials.set(items);
      },
      error: () => undefined,
    });
    this.startSlideTimer(DEFAULT_SLIDE_DURATION_SECONDS);

    this.statsBgTimer = setInterval(() => {
      this.statsBgIndex.update((index) => (index + 1) % this.statsBgImages.length);
    }, STATS_BG_INTERVAL_MS);

    this.siteSettingsService.get().subscribe({
      next: (settings) => this.startSlideTimer(settings.slideDurationSeconds),
      error: () => undefined,
    });

    // Static content above renders immediately - the API only overrides it once loaded, and any
    // failure keeps whatever is already showing (never a blank homepage).
    this.slidesService.getVisible().subscribe({
      next: (slides) => {
        if (slides.length === 0) return;
        this.slides.set(
          slides.map((s) => ({
            image: s.imageUrl,
            focalPoint: s.focalPoint ?? undefined,
            kicker: s.kicker,
            title: s.title,
            text: s.text,
            place: s.place ?? undefined,
          })),
        );
        this.activeSlide.set(0);
      },
      error: () => undefined,
    });

    this.pillarsService.getVisible().subscribe({
      next: (pillars) => {
        if (pillars.length === 0) return;
        this.pillars.set(
          pillars.map((p) => ({ title: p.title, text: p.text, image: p.imageUrl, icon: p.icon })),
        );
      },
      error: () => undefined,
    });

    this.participantsService.getVisible().subscribe({
      next: (participants) => {
        if (participants.length === 0) return;
        this.participants.set(
          participants.map((p) => ({ name: p.name, description: p.description })),
        );
      },
      error: () => undefined,
    });

    this.partnersService.getVisible().subscribe({
      next: (partners) => {
        if (partners.length === 0) return;
        this.partners.set(
          partners.map((p) => ({ name: p.name, label: p.label ?? undefined, logo: p.logoUrl })),
        );
      },
      error: () => undefined,
    });

    // Speakers are scoped to an edition - resolve the current one first, then ask only for its
    // speakers. Once the current edition is known, its speakers (even zero) replace the static
    // fallback, so last year's panelists never linger under a heading naming the new edition.
    this.editionsService.getVisible().subscribe({
      next: (editions) => {
        if (editions.length === 0) return;
        this.editionsCount.set(editions.length);
      },
      error: () => undefined,
    });

    this.editionsService.getCurrentEdition().subscribe({
      next: (current) => {
        if (!current) return;
        this.countdownDate.set(current.startDate);

        this.speakersService.getVisibleByEdition(current.id).subscribe({
          next: (speakers) => {
            this.speakers.set(
              speakers.map((s) => ({
                name: s.name,
                role: s.role,
                image: s.imageUrl,
                description: s.description,
              })),
            );
          },
          error: () => undefined,
        });
      },
      error: () => undefined,
    });
  }

  ngOnDestroy(): void {
    clearInterval(this.slideTimer);
    clearInterval(this.statsBgTimer);
  }

  private startSlideTimer(durationSeconds: number): void {
    clearInterval(this.slideTimer);
    this.slideTimer = setInterval(() => {
      this.activeSlide.update((index) => (index + 1) % this.slides().length);
    }, durationSeconds * 1000);
  }

  goToSlide(index: number): void {
    this.activeSlide.set(index);
  }

  togglePresentation(): void {
    this.presentationExpanded.update((value) => !value);
  }

  // A short punchy line ("FISD 2026") and a full honorific name ("Son Excellence Madame...")
  // don't read well at the same size - scale the clamp down as the title gets longer so long
  // titles stay legible and dignified instead of overflowing into an oversized wrap.
  heroTitleSize(title: string): string {
    const length = title.length;
    if (length > 45) return 'clamp(1.7rem,3.6vw,3.4rem)';
    if (length > 30) return 'clamp(1.85rem,4.4vw,4.4rem)';
    return 'clamp(2rem,5.4vw,6rem)';
  }
}
