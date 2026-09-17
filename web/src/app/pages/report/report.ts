import { Component } from '@angular/core';
import { TranslatePipe } from '@ngx-translate/core';
import { PageHeroComponent } from '../../shared/page-hero/page-hero';
import { EditorialService } from '../../core/editorial.service';
@Component({
  selector: 'app-report',
  standalone: true,
  imports: [TranslatePipe, PageHeroComponent],
  template: `<app-page-hero
      [eyebrow]="'Nav.PreviousEdition' | translate"
      [title]="'Report.Title' | translate"
      [description]="'Report.Description' | translate"
    ></app-page-hero>
    <section
      class="mx-auto my-16 w-[min(900px,calc(100%-36px))] rounded-[28px] border border-line bg-white p-10 shadow-card"
    >
      @if (editorial.content().reportUrl) {
        <a
          [href]="editorial.content().reportUrl"
          target="_blank"
          rel="noopener noreferrer"
          class="inline-flex rounded-2xl bg-primary px-6 py-3 font-bold text-white"
          >{{ 'Report.Download' | translate }}</a
        >
      } @else {
        <p class="text-lg text-muted">{{ 'Report.Empty' | translate }}</p>
      }
    </section>`,
})
export class ReportComponent {
  constructor(public editorial: EditorialService) {}
}
