import { Component, OnInit, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { AdminPageHeaderComponent } from '../../../shared/admin/admin-page-header/admin-page-header';
import { AdminInfoCardComponent } from '../../../shared/admin/admin-info-card/admin-info-card';
import { SnackbarService } from '../../../shared/admin/snackbar/snackbar.service';
import { ConfirmDialogService } from '../../../shared/admin/confirm-dialog/confirm-dialog.service';
import { HelpArticlesService } from '../../../core/help-articles.service';
import { AdminHelpArticle } from '../../../core/content.models';
import { NoDataComponent } from '../../../shared/admin/nodata/nodata';

@Component({
  selector: 'app-admin-help-articles',
  standalone: true,
  imports: [RouterLink, TranslatePipe, AdminPageHeaderComponent, AdminInfoCardComponent, NoDataComponent],
  templateUrl: './admin-help-articles.html'
})
export class AdminHelpArticlesComponent implements OnInit {
  articles = signal<AdminHelpArticle[]>([]);
  loading = signal(true);

  constructor(
    private snackbar: SnackbarService,
    private helpArticlesService: HelpArticlesService,
    private confirmDialog: ConfirmDialogService,
    private translate: TranslateService
  ) {}

  ngOnInit(): void {
    this.refresh();
  }

  private refresh(): void {
    this.loading.set(true);
    this.helpArticlesService.getAll().subscribe({
      next: (articles) => {
        this.articles.set(articles);
        this.loading.set(false);
      },
      error: () => this.loading.set(false)
    });
  }

  async deleteArticle(article: AdminHelpArticle): Promise<void> {
    const confirmed = await this.confirmDialog.confirm({
      title: this.translate.instant('Common.ConfirmDeleteTitle'),
      message: this.translate.instant('AdminHelpArticles.ConfirmDelete'),
      confirmLabel: this.translate.instant('Common.Delete'),
      variant: 'danger'
    });
    if (!confirmed) return;

    this.helpArticlesService.delete(article.id).subscribe({
      next: () => {
        this.snackbar.success(this.translate.instant('AdminHelpArticles.DeleteSuccess'));
        this.refresh();
      },
      error: (err) => this.snackbar.error(err?.error?.error ?? this.translate.instant('AdminHelpArticles.DeleteFailed'))
    });
  }
}
