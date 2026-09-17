import { Component, OnInit, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { TranslatePipe } from '@ngx-translate/core';
import { PageHeroComponent } from '../../shared/page-hero/page-hero';
import { SectionTitleComponent } from '../../shared/section-title/section-title';
import { ScrollRevealDirective } from '../../shared/directives/scroll-reveal.directive';
import { ArticlesService } from '../../core/articles.service';
import { Article } from '../../core/content.models';

@Component({
  selector: 'app-news',
  standalone: true,
  imports: [RouterLink, TranslatePipe, PageHeroComponent, SectionTitleComponent, ScrollRevealDirective],
  templateUrl: './news.html'
})
export class NewsComponent implements OnInit {
  articles = signal<Article[]>([]);
  loaded = signal(false);

  constructor(private articlesService: ArticlesService) {}

  ngOnInit(): void {
    this.articlesService.getPublished().subscribe({
      next: (articles) => {
        this.articles.set(articles);
        this.loaded.set(true);
      },
      error: () => this.loaded.set(true)
    });
  }

  formatDate(value: string | null): string {
    if (!value) return '';
    const date = new Date(value);
    if (Number.isNaN(date.getTime())) return '';
    return date.toLocaleDateString('fr-CA', { year: 'numeric', month: 'long', day: 'numeric' });
  }
}
