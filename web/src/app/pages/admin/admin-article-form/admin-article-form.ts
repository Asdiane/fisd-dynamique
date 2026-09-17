import { ChangeDetectorRef, Component, HostListener, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { AdminPageHeaderComponent } from '../../../shared/admin/admin-page-header/admin-page-header';
import { SnackbarService } from '../../../shared/admin/snackbar/snackbar.service';
import { ConfirmDialogService } from '../../../shared/admin/confirm-dialog/confirm-dialog.service';
import { firstValueFrom } from 'rxjs';
import { ArticlesService } from '../../../core/articles.service';
import { MediaService } from '../../../core/media.service';
import { Article, ArticleStatus } from '../../../core/content.models';
import { ComponentWithUnsavedChanges } from '../../../core/unsaved-changes.guard';

const ALLOWED_IMAGE_TYPES: Record<string, string> = {
  'image/jpeg': 'jpg',
  'image/png': 'png',
  'image/webp': 'webp'
};
const MAX_IMAGE_BYTES = 5 * 1024 * 1024;

@Component({
  selector: 'app-admin-article-form',
  standalone: true,
  imports: [FormsModule, RouterLink, TranslatePipe, AdminPageHeaderComponent],
  templateUrl: './admin-article-form.html'
})
export class AdminArticleFormComponent implements OnInit, ComponentWithUnsavedChanges {
  readonly ArticleStatus = ArticleStatus;
  editingId: string | null = null;
  current: Article | null = null;
  loading = signal(false);
  busy = signal(false);
  dirty = signal(false);
  previewOpen = signal(false);

  title = '';
  excerpt = '';
  content = '';
  imageUrl = '';
  localImagePreview = '';
  private selectedFile: File | null = null;

  constructor(
    private snackbar: SnackbarService,
    private route: ActivatedRoute,
    private router: Router,
    private articlesService: ArticlesService,
    private mediaService: MediaService,
    private confirmDialog: ConfirmDialogService,
    private translate: TranslateService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (!id) return;

    this.editingId = id;
    this.loading.set(true);
    this.articlesService.getById(id).subscribe({
      next: (article) => {
        this.loading.set(false);
        this.current = article;
        this.title = article.title;
        this.excerpt = article.excerpt ?? '';
        this.content = article.content;
        this.imageUrl = article.imageUrl ?? '';
        this.cdr.detectChanges();
      },
      error: () => this.loading.set(false)
    });
  }

  @HostListener('window:beforeunload', ['$event'])
  onBeforeUnload(event: BeforeUnloadEvent): void {
    if (this.dirty()) {
      event.preventDefault();
      event.returnValue = '';
    }
  }

  hasUnsavedChanges(): boolean {
    return this.dirty();
  }

  confirmDiscardMessage(): string {
    return this.translate.instant('AdminArticles.ConfirmDiscard');
  }

  markDirty(): void {
    this.dirty.set(true);
  }

  onImageSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    const file = input.files?.[0];
    if (this.localImagePreview) URL.revokeObjectURL(this.localImagePreview);
    this.localImagePreview = '';
    this.selectedFile = null;

    if (file && (!ALLOWED_IMAGE_TYPES[file.type] || file.size > MAX_IMAGE_BYTES)) {
      input.value = '';
      this.snackbar.error(this.translate.instant('AdminArticles.InvalidImage'));
      return;
    }

    if (file) {
      this.selectedFile = file;
      this.localImagePreview = URL.createObjectURL(file);
      this.dirty.set(true);
    }
  }

  removeImage(): void {
    if (this.localImagePreview) URL.revokeObjectURL(this.localImagePreview);
    this.localImagePreview = '';
    this.selectedFile = null;
    this.imageUrl = '';
    this.dirty.set(true);
  }

  get coverPreviewUrl(): string {
    return this.localImagePreview || this.imageUrl;
  }

  togglePreview(): void {
    this.previewOpen.update((open) => !open);
  }

  private async uploadSelectedImage(): Promise<void> {
    if (!this.selectedFile) return;

    try {
      const uploaded = await firstValueFrom(this.mediaService.upload('articles', this.selectedFile));
      this.imageUrl = uploaded.url;
    } catch {
      throw new Error(this.translate.instant('AdminArticles.UploadFailed'));
    }
  }

  save(status: ArticleStatus): void {
    if (this.busy()) return;
    if (!this.title.trim() || !this.content.trim()) {
      this.snackbar.error(this.translate.instant('AdminArticles.MissingFields'));
      return;
    }

    this.busy.set(true);

    this.uploadSelectedImage()
      .then(() => {
        const payload = {
          title: this.title.trim(),
          excerpt: this.excerpt.trim(),
          content: this.content.trim(),
          imageUrl: this.imageUrl || null,
          status,
          publishedAt: this.current?.publishedAt ?? null
        };

        const request$ = this.editingId
          ? this.articlesService.update(this.editingId, payload)
          : this.articlesService.create(payload);

        request$.subscribe({
          next: (saved) => {
            this.busy.set(false);
            this.current = saved;
            this.dirty.set(false);
            this.snackbar.success(
              status === ArticleStatus.Published
                ? this.translate.instant('AdminArticles.PublishSuccess')
                : this.translate.instant('AdminArticles.DraftSuccess')
            );

            if (!this.editingId) {
              this.editingId = saved.id;
              this.router.navigate(['/admin/articles', saved.id], { replaceUrl: true });
            }
          },
          error: (err) => {
            this.busy.set(false);
            this.snackbar.error(err?.error?.error ?? this.translate.instant('AdminArticles.SaveFailed'));
          }
        });
      })
      .catch((error: Error) => {
        this.busy.set(false);
        this.snackbar.error(error.message);
      });
  }

  async delete(): Promise<void> {
    if (!this.editingId || this.busy()) return;

    const confirmed = await this.confirmDialog.confirm({
      title: this.translate.instant('Common.ConfirmDeleteTitle'),
      message: this.translate.instant('AdminArticles.ConfirmDelete'),
      confirmLabel: this.translate.instant('Common.Delete'),
      variant: 'danger'
    });
    if (!confirmed) return;

    this.busy.set(true);
    this.dirty.set(false);
    this.articlesService.delete(this.editingId).subscribe({
      next: () => this.router.navigate(['/admin/articles']),
      error: (err) => {
        this.busy.set(false);
        this.snackbar.error(err?.error?.error ?? this.translate.instant('AdminArticles.DeleteFailed'));
      }
    });
  }
}
