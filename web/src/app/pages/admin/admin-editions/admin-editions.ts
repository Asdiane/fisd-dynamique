import { Component, OnInit, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { EditionsService } from '../../../core/editions.service';
import { AdminEdition } from '../../../core/content.models';
import { AdminPageHeaderComponent } from '../../../shared/admin/admin-page-header/admin-page-header';
import { SnackbarService } from '../../../shared/admin/snackbar/snackbar.service';
import { AdminInfoCardComponent } from '../../../shared/admin/admin-info-card/admin-info-card';
import { ConfirmDialogService } from '../../../shared/admin/confirm-dialog/confirm-dialog.service';
import { NoDataComponent } from '../../../shared/admin/nodata/nodata';

@Component({
  selector: 'app-admin-editions',
  standalone: true,
  imports: [RouterLink, TranslatePipe, AdminPageHeaderComponent, AdminInfoCardComponent, NoDataComponent],
  templateUrl: './admin-editions.html'
})
export class AdminEditionsComponent implements OnInit {
  editions = signal<AdminEdition[]>([]);
  loading = signal(true);
  settingCurrentId = signal<string | null>(null);

  constructor(
    private snackbar: SnackbarService,
    private editionsService: EditionsService,
    private confirmDialog: ConfirmDialogService,
    private translate: TranslateService
  ) {}

  ngOnInit(): void {
    this.refresh();
  }

  private refresh(): void {
    this.loading.set(true);
    this.editionsService.getAll().subscribe({
      next: (editions) => {
        this.editions.set(editions);
        this.loading.set(false);
      },
      error: () => this.loading.set(false)
    });
  }

  setCurrent(edition: AdminEdition): void {
    if (edition.isCurrent || this.settingCurrentId()) return;

    this.settingCurrentId.set(edition.id);
    this.editionsService.setCurrent(edition.id).subscribe({
      next: () => {
        this.settingCurrentId.set(null);
        this.snackbar.success(this.translate.instant('AdminEditions.SetCurrentSuccess'));
        this.refresh();
      },
      error: (err) => {
        this.settingCurrentId.set(null);
        this.snackbar.error(err?.error?.error ?? this.translate.instant('AdminEditions.SetCurrentFailed'));
      }
    });
  }

  async deleteEdition(edition: AdminEdition): Promise<void> {
    const confirmed = await this.confirmDialog.confirm({
      title: this.translate.instant('Common.ConfirmDeleteTitle'),
      message: this.translate.instant('AdminEditions.ConfirmDelete'),
      confirmLabel: this.translate.instant('Common.Delete'),
      variant: 'danger'
    });
    if (!confirmed) return;

    this.editionsService.delete(edition.id).subscribe({
      next: () => {
        this.snackbar.success(this.translate.instant('AdminEditions.DeleteSuccess'));
        this.refresh();
      },
      error: (err) => this.snackbar.error(err?.error?.error ?? this.translate.instant('AdminEditions.DeleteFailed'))
    });
  }
}
