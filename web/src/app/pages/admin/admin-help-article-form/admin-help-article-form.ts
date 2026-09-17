import { ChangeDetectorRef, Component, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { AdminPageHeaderComponent } from '../../../shared/admin/admin-page-header/admin-page-header';
import { SnackbarService } from '../../../shared/admin/snackbar/snackbar.service';
import { ConfirmDialogService } from '../../../shared/admin/confirm-dialog/confirm-dialog.service';
import { HelpArticlesService } from '../../../core/help-articles.service';

@Component({
  selector: 'app-admin-help-article-form',
  standalone: true,
  imports: [FormsModule, RouterLink, TranslatePipe, AdminPageHeaderComponent],
  templateUrl: './admin-help-article-form.html'
})
export class AdminHelpArticleFormComponent implements OnInit {
  editingId: string | null = null;
  loading = signal(false);
  busy = signal(false);
  activeLang = signal<'fr' | 'en'>('fr');

  category = '';
  titleFr = '';
  titleEn = '';
  contentFr = '';
  contentEn = '';
  isVisible = true;
  displayOrder = 1;
  isPlatformAdminOnly = false;

  constructor(
    private snackbar: SnackbarService,
    private route: ActivatedRoute,
    private router: Router,
    private helpArticlesService: HelpArticlesService,
    private confirmDialog: ConfirmDialogService,
    private translate: TranslateService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (!id) return;

    this.editingId = id;
    this.loading.set(true);
    this.helpArticlesService.getById(id).subscribe({
      next: (article) => {
        this.loading.set(false);
        if (!article) return;
        this.category = article.category;
        this.titleFr = article.titleFr;
        this.titleEn = article.titleEn;
        this.contentFr = article.contentFr;
        this.contentEn = article.contentEn;
        this.isVisible = article.isVisible;
        this.displayOrder = article.displayOrder;
        this.isPlatformAdminOnly = article.isPlatformAdminOnly;
        this.cdr.detectChanges();
      },
      error: () => this.loading.set(false)
    });
  }

  private hasRequiredContent(): boolean {
    return !!(this.category.trim() && this.titleFr.trim() && this.titleEn.trim() && this.contentFr.trim() && this.contentEn.trim());
  }

  save(): void {
    if (this.busy()) return;
    if (!this.hasRequiredContent()) {
      this.snackbar.error(this.translate.instant('AdminHelpArticles.MissingFields'));
      return;
    }

    this.busy.set(true);

    const payload = {
      category: this.category.trim(),
      titleFr: this.titleFr.trim(),
      titleEn: this.titleEn.trim(),
      contentFr: this.contentFr.trim(),
      contentEn: this.contentEn.trim(),
      isVisible: this.isVisible,
      displayOrder: this.displayOrder,
      isPlatformAdminOnly: this.isPlatformAdminOnly
    };

    const request$ = this.editingId
      ? this.helpArticlesService.update(this.editingId, payload)
      : this.helpArticlesService.create(payload);

    request$.subscribe({
      next: () => this.router.navigate(['/admin/help/manage']),
      error: (err) => {
        this.busy.set(false);
        this.snackbar.error(err?.error?.error ?? this.translate.instant('AdminHelpArticles.SaveFailed'));
      }
    });
  }

  async delete(): Promise<void> {
    if (!this.editingId || this.busy()) return;

    const confirmed = await this.confirmDialog.confirm({
      title: this.translate.instant('Common.ConfirmDeleteTitle'),
      message: this.translate.instant('AdminHelpArticles.ConfirmDelete'),
      confirmLabel: this.translate.instant('Common.Delete'),
      variant: 'danger'
    });
    if (!confirmed) return;

    this.busy.set(true);
    this.helpArticlesService.delete(this.editingId).subscribe({
      next: () => this.router.navigate(['/admin/help/manage']),
      error: (err) => {
        this.busy.set(false);
        this.snackbar.error(err?.error?.error ?? this.translate.instant('AdminHelpArticles.DeleteFailed'));
      }
    });
  }
}
