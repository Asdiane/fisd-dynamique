import { Component, OnInit, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { AdminPageHeaderComponent } from '../../../shared/admin/admin-page-header/admin-page-header';
import { SnackbarService } from '../../../shared/admin/snackbar/snackbar.service';
import { AdminInfoCardComponent } from '../../../shared/admin/admin-info-card/admin-info-card';
import { ConfirmDialogService } from '../../../shared/admin/confirm-dialog/confirm-dialog.service';
import { ArticlesService } from '../../../core/articles.service';
import { Article, ArticleStatus } from '../../../core/content.models';
import { NoDataComponent } from '../../../shared/admin/nodata/nodata';

@Component({
  selector: 'app-admin-articles',
  standalone: true,
  imports: [RouterLink, TranslatePipe, AdminPageHeaderComponent, AdminInfoCardComponent, NoDataComponent],
  templateUrl: './admin-articles.html'
})
export class AdminArticlesComponent implements OnInit {
  readonly ArticleStatus = ArticleStatus;
  articles = signal<Article[]>([]);
  loading = signal(true);

  constructor(
    private snackbar: SnackbarService,
    private articlesService: ArticlesService,
    private confirmDialog: ConfirmDialogService,
    private translate: TranslateService
  ) {}

  ngOnInit(): void {
    this.refresh();
  }

  private refresh(): void {
    this.loading.set(true);
    this.articlesService.getAll().subscribe({
      next: (articles) => {
        this.articles.set(articles);
        this.loading.set(false);
      },
      error: () => this.loading.set(false)
    });
  }

  statusLabel(status: ArticleStatus): string {
    return status === ArticleStatus.Published
      ? this.translate.instant('AdminArticles.StatusPublished')
      : this.translate.instant('AdminArticles.StatusDraft');
  }

  async deleteArticle(article: Article): Promise<void> {
    const confirmed = await this.confirmDialog.confirm({
      title: this.translate.instant('Common.ConfirmDeleteTitle'),
      message: this.translate.instant('AdminArticles.ConfirmDelete'),
      confirmLabel: this.translate.instant('Common.Delete'),
      variant: 'danger'
    });
    if (!confirmed) return;

    this.articlesService.delete(article.id).subscribe({
      next: () => {
        this.snackbar.success(this.translate.instant('AdminArticles.DeleteSuccess'));
        this.refresh();
      },
      error: (err) => this.snackbar.error(err?.error?.error ?? this.translate.instant('AdminArticles.DeleteFailed'))
    });
  }
}
