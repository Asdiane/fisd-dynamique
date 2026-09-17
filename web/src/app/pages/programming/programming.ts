import { Component, OnInit, signal } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { TranslatePipe } from '@ngx-translate/core';
import { forkJoin, map, Observable } from 'rxjs';
import { PageHeroComponent } from '../../shared/page-hero/page-hero';
import { SectionTitleComponent } from '../../shared/section-title/section-title';
import { ScrollRevealDirective } from '../../shared/directives/scroll-reveal.directive';
import { PROGRAMS, Program, ProgramDay } from '../../core/content/site-content';
import { EditionsService } from '../../core/editions.service';
import { Edition } from '../../core/content.models';

@Component({
  selector: 'app-programming',
  standalone: true,
  imports: [TranslatePipe, PageHeroComponent, SectionTitleComponent, ScrollRevealDirective],
  templateUrl: './programming.html'
})
export class ProgrammingComponent implements OnInit {
  private readonly defaultEdition = PROGRAMS.reduce((latest, program) => (Number(program.id) > Number(latest.id) ? program : latest), PROGRAMS[0]);

  edition = signal<Program>(this.defaultEdition);
  activeDayId = signal(this.defaultEdition.days[0]?.id ?? '');

  constructor(private route: ActivatedRoute, private editionsService: EditionsService) {}

  ngOnInit(): void {
    const yearParam = this.route.snapshot.paramMap.get('edition');
    const staticMatch = yearParam ? PROGRAMS.find((program) => program.id === yearParam) : undefined;
    if (staticMatch) {
      this.edition.set(staticMatch);
      this.activeDayId.set(staticMatch.days[0]?.id ?? '');
    }

    // Static content above renders immediately - the API only overrides it once loaded, and any
    // failure at any step below silently keeps whatever is already showing (never a blank page).
    const editions$: Observable<Edition | undefined> = yearParam
      ? this.editionsService.getVisible().pipe(map((editions) => editions.find((e) => e.year.toString() === yearParam)))
      : this.editionsService.getCurrentEdition().pipe(map((edition) => edition ?? undefined));

    editions$.subscribe({
      next: (match) => {
        if (!match) return;

        this.editionsService.getDays(match.id).subscribe({
          next: (days) => {
            if (days.length === 0) return;

            forkJoin(days.map((day) => this.editionsService.getSchedule(day.id))).subscribe({
              next: (scheduleLists) => {
                const program: Program = {
                  id: match.year.toString(),
                  badge: match.badge,
                  title: match.title,
                  text: match.text,
                  days: days.map((day, index) => ({
                    id: day.id,
                    label: day.label,
                    date: day.dateLabel,
                    schedule: scheduleLists[index].map((item) => ({
                      time: item.time,
                      title: item.title,
                      tag: item.tag,
                      detail: item.detail,
                      location: item.location
                    }))
                  }))
                };
                this.edition.set(program);
                this.activeDayId.set(program.days[0]?.id ?? '');
              },
              error: () => undefined
            });
          },
          error: () => undefined
        });
      },
      error: () => undefined
    });
  }

  get activeDay(): ProgramDay {
    const edition = this.edition();
    return edition.days.find((day) => day.id === this.activeDayId()) ?? edition.days[0];
  }

  selectDay(dayId: string): void {
    this.activeDayId.set(dayId);
  }
}
