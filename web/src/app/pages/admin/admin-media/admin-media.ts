import { Component, OnInit, signal } from '@angular/core';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { AdminPageHeaderComponent } from '../../../shared/admin/admin-page-header/admin-page-header';
import { SnackbarService } from '../../../shared/admin/snackbar/snackbar.service';
import { AdminInfoCardComponent } from '../../../shared/admin/admin-info-card/admin-info-card';
import { ConfirmDialogService } from '../../../shared/admin/confirm-dialog/confirm-dialog.service';
import { MediaService, MediaFile } from '../../../core/media.service';
import { NoDataComponent } from '../../../shared/admin/nodata/nodata';

@Component({
  selector: 'app-admin-media',
  standalone: true,
  imports: [TranslatePipe, AdminPageHeaderComponent, AdminInfoCardComponent, NoDataComponent],
  templateUrl: './admin-media.html'
})
export class AdminMediaComponent implements OnInit {
  files = signal<MediaFile[]>([]);
  loading = signal(true);
  categoryFilter = signal<string>('all');

  constructor(
    private snackbar: SnackbarService,
    private mediaService: MediaService,
    private confirmDialog: ConfirmDialogService,
    private translate: TranslateService
  ) {}

  ngOnInit(): void {
    this.refresh();
  }

  private refresh(): void {
    this.loading.set(true);
    this.mediaService.getAll().subscribe({
      next: (files) => {
        this.files.set(files);
        this.loading.set(false);
      },
      error: () => this.loading.set(false)
    });
  }

  get categories(): string[] {
    return Array.from(new Set(this.files().map((f) => f.category))).sort();
  }

  get filteredFiles(): MediaFile[] {
    const filter = this.categoryFilter();
    return filter === 'all' ? this.files() : this.files().filter((f) => f.category === filter);
  }

  setFilter(category: string): void {
    this.categoryFilter.set(category);
  }

  formatSize(bytes: number): string {
    if (bytes < 1024) return `${bytes} o`;
    if (bytes < 1024 * 1024) return `${(bytes / 1024).toFixed(1)} Ko`;
    return `${(bytes / (1024 * 1024)).toFixed(1)} Mo`;
  }

  async deleteFile(file: MediaFile): Promise<void> {
    const confirmed = await this.confirmDialog.confirm({
      title: this.translate.instant('Common.ConfirmDeleteTitle'),
      message: this.translate.instant('AdminMedia.ConfirmDelete'),
      confirmLabel: this.translate.instant('Common.Delete'),
      variant: 'danger'
    });
    if (!confirmed) return;

    this.mediaService.delete(file.id).subscribe({
      next: () => {
        this.snackbar.success(this.translate.instant('AdminMedia.DeleteSuccess'));
        this.refresh();
      },
      error: (err) => this.snackbar.error(err?.error?.error ?? this.translate.instant('AdminMedia.DeleteFailed'))
    });
  }
}
