import { Component, OnInit, signal } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { TranslatePipe } from '@ngx-translate/core';
import { ArticlesService } from '../../core/articles.service';
import { Article } from '../../core/content.models';

@Component({
  selector: 'app-news-detail',
  standalone: true,
  imports: [RouterLink, TranslatePipe],
  templateUrl: './news-detail.html'
})
export class NewsDetailComponent implements OnInit {
  article = signal<Article | null>(null);
  loaded = signal(false);

  constructor(private route: ActivatedRoute, private articlesService: ArticlesService) {}

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (!id) {
      this.loaded.set(true);
      return;
    }
    this.articlesService.getById(id).subscribe({
      next: (article) => {
        this.article.set(article);
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
