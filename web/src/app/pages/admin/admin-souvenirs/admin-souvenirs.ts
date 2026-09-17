import { Component, OnInit, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { AdminPageHeaderComponent } from '../../../shared/admin/admin-page-header/admin-page-header';
import { SnackbarService } from '../../../shared/admin/snackbar/snackbar.service';
import { AdminInfoCardComponent } from '../../../shared/admin/admin-info-card/admin-info-card';
import { ConfirmDialogService } from '../../../shared/admin/confirm-dialog/confirm-dialog.service';
import { SouvenirsService } from '../../../core/souvenirs.service';
import { Souvenir, SouvenirStatus } from '../../../core/content.models';
import { NoDataComponent } from '../../../shared/admin/nodata/nodata';

@Component({
  selector: 'app-admin-souvenirs',
  standalone: true,
  imports: [RouterLink, TranslatePipe, AdminPageHeaderComponent, AdminInfoCardComponent, NoDataComponent],
  templateUrl: './admin-souvenirs.html'
})
export class AdminSouvenirsComponent implements OnInit {
  readonly SouvenirStatus = SouvenirStatus;
  souvenirs = signal<Souvenir[]>([]);
  loading = signal(true);

  constructor(
    private snackbar: SnackbarService,
    private souvenirsService: SouvenirsService,
    private confirmDialog: ConfirmDialogService,
    private translate: TranslateService
  ) {}

  ngOnInit(): void {
    this.refresh();
  }

  private refresh(): void {
    this.loading.set(true);
    this.souvenirsService.getAll().subscribe({
      next: (souvenirs) => {
        this.souvenirs.set(souvenirs);
        this.loading.set(false);
      },
      error: () => this.loading.set(false)
    });
  }

  async deleteSouvenir(souvenir: Souvenir): Promise<void> {
    const confirmed = await this.confirmDialog.confirm({
      title: this.translate.instant('Common.ConfirmDeleteTitle'),
      message: this.translate.instant('AdminSouvenirs.ConfirmDelete'),
      confirmLabel: this.translate.instant('Common.Delete'),
      variant: 'danger'
    });
    if (!confirmed) return;

    this.souvenirsService.delete(souvenir.id).subscribe({
      next: () => {
        this.snackbar.success(this.translate.instant('AdminSouvenirs.DeleteSuccess'));
        this.refresh();
      },
      error: (err) => this.snackbar.error(err?.error?.error ?? this.translate.instant('AdminSouvenirs.DeleteFailed'))
    });
  }
}
