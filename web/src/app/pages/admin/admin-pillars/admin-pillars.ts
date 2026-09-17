import { Component, OnInit, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { AdminPageHeaderComponent } from '../../../shared/admin/admin-page-header/admin-page-header';
import { SnackbarService } from '../../../shared/admin/snackbar/snackbar.service';
import { AdminInfoCardComponent } from '../../../shared/admin/admin-info-card/admin-info-card';
import { ConfirmDialogService } from '../../../shared/admin/confirm-dialog/confirm-dialog.service';
import { PillarsService } from '../../../core/pillars.service';
import { AdminPillar } from '../../../core/content.models';
import { NoDataComponent } from '../../../shared/admin/nodata/nodata';

@Component({
  selector: 'app-admin-pillars',
  standalone: true,
  imports: [RouterLink, TranslatePipe, AdminPageHeaderComponent, AdminInfoCardComponent, NoDataComponent],
  templateUrl: './admin-pillars.html'
})
export class AdminPillarsComponent implements OnInit {
  pillars = signal<AdminPillar[]>([]);
  loading = signal(true);

  constructor(
    private snackbar: SnackbarService,
    private pillarsService: PillarsService,
    private confirmDialog: ConfirmDialogService,
    private translate: TranslateService
  ) {}

  ngOnInit(): void {
    this.refresh();
  }

  private refresh(): void {
    this.loading.set(true);
    this.pillarsService.getAll().subscribe({
      next: (pillars) => {
        this.pillars.set(pillars);
        this.loading.set(false);
      },
      error: () => this.loading.set(false)
    });
  }

  async deletePillar(pillar: AdminPillar): Promise<void> {
    const confirmed = await this.confirmDialog.confirm({
      title: this.translate.instant('Common.ConfirmDeleteTitle'),
      message: this.translate.instant('AdminPillars.ConfirmDelete'),
      confirmLabel: this.translate.instant('Common.Delete'),
      variant: 'danger'
    });
    if (!confirmed) return;

    this.pillarsService.delete(pillar.id).subscribe({
      next: () => {
        this.snackbar.success(this.translate.instant('AdminPillars.DeleteSuccess'));
        this.refresh();
      },
      error: (err) => this.snackbar.error(err?.error?.error ?? this.translate.instant('AdminPillars.DeleteFailed'))
    });
  }
}
