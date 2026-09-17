import { Component, OnInit, signal } from '@angular/core';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { forkJoin, of } from 'rxjs';
import { PageHeroComponent } from '../../shared/page-hero/page-hero';
import { SectionTitleComponent } from '../../shared/section-title/section-title';
import { ScrollRevealDirective } from '../../shared/directives/scroll-reveal.directive';
import { SouvenirsService } from '../../core/souvenirs.service';
import { TestimonialsService } from '../../core/testimonials.service';
import { EditionsService } from '../../core/editions.service';
import { STATIC_EVENTS, StaticEvent, StaticPhoto, PROGRAMS, Program, STATIC_TESTIMONIALS, StaticTestimonial } from '../../core/content/site-content';
import { SouvenirStatus, Edition } from '../../core/content.models';

function excludeMostRecentEdition(programs: Program[]): Program[] {
  const maxId = Math.max(...programs.map((program) => Number(program.id)));
  return programs.filter((program) => Number(program.id) !== maxId);
}

@Component({
  selector: 'app-gallery',
  standalone: true,
  imports: [TranslatePipe, PageHeroComponent, SectionTitleComponent, ScrollRevealDirective],
  templateUrl: './gallery.html'
})
export class GalleryComponent implements OnInit {
  events = signal<StaticEvent[]>(STATIC_EVENTS);
  testimonials = signal<StaticTestimonial[]>(STATIC_TESTIMONIALS);
  // The most recent edition (highest year) has no completed program yet - it's excluded from
  // "archived program" links until its own program data exists, whether static or API-backed.
  programs = signal<Program[]>(excludeMostRecentEdition(PROGRAMS));
  activeEditionId = signal('2025');
  activePhotoIndex = signal(0);
  archiveOpen = signal(false);

  constructor(
    private souvenirsService: SouvenirsService,
    private testimonialsService: TestimonialsService,
    private editionsService: EditionsService,
    private translate: TranslateService
  ) {}

  get activeEdition(): StaticEvent {
    return this.events().find((event) => event.id === this.activeEditionId()) ?? this.events()[0];
  }

  get activePhoto(): StaticPhoto | undefined {
    return this.activeEdition.photos[this.activePhotoIndex()];
  }

  get archivedProgram() {
    return this.programs().find((program) => program.id === this.activeEdition.id);
  }

  get statusTitle(): string {
    return this.activeEdition.statusTitle || this.translate.instant('Gallery.DefaultUpcomingTitle');
  }

  get statusText(): string {
    return this.activeEdition.statusText || this.translate.instant('Gallery.DefaultUpcomingText');
  }

  ngOnInit(): void {
    forkJoin({
      souvenirs: this.souvenirsService.getAll(),
      testimonials: this.testimonialsService.getVisible(),
      editions: this.editionsService.getVisible()
    }).subscribe({
      next: ({ souvenirs, testimonials, editions }) => {
        if (souvenirs.length > 0) {
          forkJoin(souvenirs.map((souvenir) => this.souvenirsService.getPhotos(souvenir.id))).subscribe((photoLists) => {
            const events: StaticEvent[] = souvenirs.map((souvenir, index) => ({
              id: String(souvenir.year),
              year: `Édition ${souvenir.year}`,
              title: souvenir.title,
              text: souvenir.description ?? '',
              statusTitle: souvenir.status === SouvenirStatus.ComingSoon ? this.translate.instant('Gallery.DefaultUpcomingTitle') : undefined,
              statusText: souvenir.status === SouvenirStatus.ComingSoon ? this.translate.instant('Gallery.DefaultUpcomingText') : undefined,
              photos: photoLists[index].map((photo) => ({
                caption: photo.caption ?? '',
                imageUrl: photo.imageUrl,
                description: photo.caption ?? ''
              }))
            }));
            this.events.set(events);
            if (!events.some((event) => event.id === this.activeEditionId())) {
              this.activeEditionId.set(events[0]?.id ?? '');
            }
          });
        }
        if (testimonials.length > 0) {
          this.testimonials.set(
            testimonials.map((testimonial) => ({
              authorName: testimonial.authorName,
              authorRole: testimonial.authorRole ?? '',
              content: testimonial.content
            }))
          );
        }
        if (editions.length > 0) {
          this.loadArchivedPrograms(editions);
        }
      },
      error: () => {
        // API unreachable - the static fallback content already set above stays in place.
      }
    });
  }

  private loadArchivedPrograms(editions: Edition[]): void {
    const maxYear = Math.max(...editions.map((edition) => edition.year));
    const archivable = editions.filter((edition) => edition.year !== maxYear);
    if (archivable.length === 0) return;

    forkJoin(archivable.map((edition) => this.editionsService.getDays(edition.id))).subscribe({
      next: (daysLists) => {
        forkJoin(daysLists.map((days) => (days.length === 0 ? of([]) : forkJoin(days.map((day) => this.editionsService.getSchedule(day.id)))))).subscribe({
          next: (scheduleListsPerEdition) => {
            const programs: Program[] = archivable.map((edition, editionIndex) => ({
              id: edition.year.toString(),
              badge: edition.badge,
              title: edition.title,
              text: edition.text,
              days: daysLists[editionIndex].map((day, dayIndex) => ({
                id: day.id,
                label: day.label,
                date: day.dateLabel,
                schedule: scheduleListsPerEdition[editionIndex][dayIndex].map((item) => ({
                  time: item.time,
                  title: item.title,
                  tag: item.tag,
                  detail: item.detail,
                  location: item.location
                }))
              }))
            }));
            this.programs.set(programs);
          },
          error: () => undefined
        });
      },
      error: () => undefined
    });
  }

  selectEdition(id: string): void {
    this.activeEditionId.set(id);
    this.activePhotoIndex.set(0);
  }

  selectPhoto(index: number): void {
    this.activePhotoIndex.set(index);
  }

  toggleArchive(): void {
    this.archiveOpen.update((open) => !open);
  }
}
