import { Component, OnInit, computed, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { TranslatePipe } from '@ngx-translate/core';
import { AdminPageHeaderComponent } from '../../../shared/admin/admin-page-header/admin-page-header';
import { NoDataComponent } from '../../../shared/admin/nodata/nodata';
import { HelpArticlesService } from '../../../core/help-articles.service';
import { HelpArticle } from '../../../core/content.models';
import { AuthService } from '../../../core/auth.service';

@Component({
  selector: 'app-admin-help',
  standalone: true,
  imports: [RouterLink, TranslatePipe, AdminPageHeaderComponent, NoDataComponent],
  templateUrl: './admin-help.html'
})
export class AdminHelpComponent implements OnInit {
  articles = signal<HelpArticle[]>([]);
  loading = signal(true);
  openId = signal<string | null>(null);

  readonly categories = computed(() => {
    const seen: string[] = [];
    for (const article of this.articles()) {
      if (!seen.includes(article.category)) seen.push(article.category);
    }
    return seen;
  });

  constructor(
    private helpArticlesService: HelpArticlesService,
    private authService: AuthService
  ) {}

  get isPlatformAdmin(): boolean {
    return this.authService.isPlatformAdmin();
  }

  ngOnInit(): void {
    this.helpArticlesService.getVisible().subscribe({
      next: (articles) => {
        this.articles.set(articles);
        this.loading.set(false);
        if (articles.length > 0) this.openId.set(articles[0].id);
      },
      error: () => this.loading.set(false)
    });
  }

  articlesFor(category: string): HelpArticle[] {
    return this.articles().filter((a) => a.category === category);
  }

  toggle(article: HelpArticle): void {
    this.openId.set(this.openId() === article.id ? null : article.id);
  }
}
