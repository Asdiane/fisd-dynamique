import { Component, OnInit, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { AdminPageHeaderComponent } from '../../../shared/admin/admin-page-header/admin-page-header';
import { SnackbarService } from '../../../shared/admin/snackbar/snackbar.service';
import { AdminInfoCardComponent } from '../../../shared/admin/admin-info-card/admin-info-card';
import { ConfirmDialogService } from '../../../shared/admin/confirm-dialog/confirm-dialog.service';
import { EngagementActionsService } from '../../../core/engagement-actions.service';
import { AdminEngagementAction } from '../../../core/content.models';
import { NoDataComponent } from '../../../shared/admin/nodata/nodata';

@Component({
  selector: 'app-admin-engagement-actions',
  standalone: true,
  imports: [RouterLink, TranslatePipe, AdminPageHeaderComponent, AdminInfoCardComponent, NoDataComponent],
  templateUrl: './admin-engagement-actions.html'
})
export class AdminEngagementActionsComponent implements OnInit {
  actions = signal<AdminEngagementAction[]>([]);
  loading = signal(true);

  constructor(
    private snackbar: SnackbarService,
    private engagementActionsService: EngagementActionsService,
    private confirmDialog: ConfirmDialogService,
    private translate: TranslateService
  ) {}

  ngOnInit(): void {
    this.refresh();
  }

  private refresh(): void {
    this.loading.set(true);
    this.engagementActionsService.getAll().subscribe({
      next: (actions) => {
        this.actions.set(actions);
        this.loading.set(false);
      },
      error: () => this.loading.set(false)
    });
  }

  async deleteAction(action: AdminEngagementAction): Promise<void> {
    const confirmed = await this.confirmDialog.confirm({
      title: this.translate.instant('Common.ConfirmDeleteTitle'),
      message: this.translate.instant('AdminEngagementActions.ConfirmDelete'),
      confirmLabel: this.translate.instant('Common.Delete'),
      variant: 'danger'
    });
    if (!confirmed) return;

    this.engagementActionsService.delete(action.id).subscribe({
      next: () => {
        this.snackbar.success(this.translate.instant('AdminEngagementActions.DeleteSuccess'));
        this.refresh();
      },
      error: (err) => this.snackbar.error(err?.error?.error ?? this.translate.instant('AdminEngagementActions.DeleteFailed'))
    });
  }
}
