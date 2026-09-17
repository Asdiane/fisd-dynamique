import { Component, OnInit, HostListener, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import {
  EditorialService,
  EditorialContent,
  defaultEditorial,
  HomeSection,
} from '../../../core/editorial.service';
import { EDITORIAL_TEXTS } from '../../../core/editorial-texts';
import { AdminPageHeaderComponent } from '../../../shared/admin/admin-page-header/admin-page-header';
@Component({
  selector: 'app-admin-editorial',
  standalone: true,
  imports: [FormsModule, TranslatePipe, AdminPageHeaderComponent],
  templateUrl: './admin-editorial.html',
})
export class AdminEditorialComponent implements OnInit {
  model: EditorialContent = defaultEditorial();
  loaded = signal(false);
  busy = signal(false);
  dirty = signal(false);
  message = signal('');
  error = signal('');
  search = '';
  group = 'Home';
  readonly groups = [...new Set(EDITORIAL_TEXTS.map((t) => t.group))];
  readonly texts = EDITORIAL_TEXTS;
  readonly resourceFields = [
    'themeImage',
    'reportUrl',
    'newsletterUrl',
    'tikTokUrl',
    'youTubeUrl',
  ] as const;
  readonly resourceLabels = {
    themeImage: 'AdminEditorial.ThemeImage',
    reportUrl: 'AdminEditorial.ReportUrl',
    newsletterUrl: 'AdminEditorial.NewsletterUrl',
    tikTokUrl: 'TikTok',
    youTubeUrl: 'YouTube',
  };
  constructor(
    public editorial: EditorialService,
    private translate: TranslateService,
  ) {}
  ngOnInit() {
    this.editorial.get().subscribe({
      next: (value) => {
        this.model = structuredClone(value ?? defaultEditorial());
        this.loaded.set(true);
      },
      error: () => this.error.set('AdminEditorial.LoadError'),
    });
  }
  get filteredTexts() {
    const q = this.search.toLocaleLowerCase();
    return this.texts.filter(
      (t) => t.group === this.group && (!q || (t.fr + ' ' + t.en).toLocaleLowerCase().includes(q)),
    );
  }
  groupLabel(group: string) {
    const labels: Record<string, string> = {
      Home: 'Nav.Home',
      Nav: 'Footer.Menu',
      Footer: 'AdminEditorial.Resources',
      Programming: 'Nav.Programming',
      Gallery: 'Nav.PreviousEdition',
      News: 'Nav.News',
      NewsDetail: 'Nav.News',
      Contact: 'Nav.Contact',
      Engagement: 'Nav.Engage',
      Reservation: 'Home.HeroReserve',
      Conditions: 'Footer.Conditions',
      Report: 'Nav.Report',
    };
    return this.translate.instant(labels[group] ?? group);
  }
  value(key: string, lang: 'fr' | 'en') {
    return this.model[lang][key] ?? this.texts.find((t) => t.key === key)?.[lang] ?? '';
  }
  setText(key: string, lang: 'fr' | 'en', value: string) {
    this.model[lang][key] = value;
    this.changed();
  }
  resetText(key: string) {
    delete this.model.fr[key];
    delete this.model.en[key];
    this.changed();
  }
  changed() {
    this.dirty.set(true);
    this.message.set('');
  }
  sectionLabel(section: HomeSection) {
    const labels: Record<string, string> = {
      theme: 'Home.PresentationEyebrow',
      program: 'Home.ProgramTitle',
      speakers: 'Home.SpeakersTitle',
      stats: 'Home.StatsTitle',
      pillars: 'Home.PillarsTitle',
      testimonials: 'Gallery.TestimonialsTitle',
      partners: 'Home.PartnersTitle',
    };
    return section.kind === 'custom'
      ? this.editorial.local(section.titleFr, section.titleEn) ||
          this.translate.instant('AdminEditorial.AddSection')
      : this.translate.instant(labels[section.kind]);
  }
  move(index: number, delta: number) {
    const next = index + delta;
    if (next < 0 || next >= this.model.sections.length) return;
    [this.model.sections[index], this.model.sections[next]] = [
      this.model.sections[next],
      this.model.sections[index],
    ];
    this.changed();
  }
  addSection() {
    this.model.sections.push({
      id: crypto.randomUUID(),
      kind: 'custom',
      visible: true,
      titleFr: '',
      titleEn: '',
      textFr: '',
      textEn: '',
      buttons: [],
    });
    this.changed();
  }
  addStat() {
    this.model.stats.push({ value: 0, suffix: '', labelFr: '', labelEn: '' });
    this.changed();
  }
  save() {
    if (this.busy() || !this.loaded()) return;
    this.busy.set(true);
    this.error.set('');
    this.editorial.save(this.model).subscribe({
      next: (value) => {
        this.model = structuredClone(value);
        this.dirty.set(false);
        this.busy.set(false);
        this.message.set('AdminEditorial.Saved');
        this.editorial.apply(value);
      },
      error: () => {
        this.busy.set(false);
        this.error.set('AdminEditorial.Error');
      },
    });
  }
  hasUnsavedChanges() {
    return this.dirty();
  }
  confirmDiscardMessage() {
    return this.translate.instant('AdminArticles.ConfirmDiscard');
  }
  @HostListener('window:beforeunload', ['$event']) beforeUnload(event: BeforeUnloadEvent) {
    if (this.dirty()) {
      event.preventDefault();
      event.returnValue = '';
    }
  }
}
