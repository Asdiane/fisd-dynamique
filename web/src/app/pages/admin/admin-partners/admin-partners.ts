import { Component, OnInit, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { AdminPageHeaderComponent } from '../../../shared/admin/admin-page-header/admin-page-header';
import { SnackbarService } from '../../../shared/admin/snackbar/snackbar.service';
import { AdminInfoCardComponent } from '../../../shared/admin/admin-info-card/admin-info-card';
import { ConfirmDialogService } from '../../../shared/admin/confirm-dialog/confirm-dialog.service';
import { PartnersService } from '../../../core/partners.service';
import { Partner } from '../../../core/content.models';
import { NoDataComponent } from '../../../shared/admin/nodata/nodata';

@Component({
  selector: 'app-admin-partners',
  standalone: true,
  imports: [RouterLink, TranslatePipe, AdminPageHeaderComponent, AdminInfoCardComponent, NoDataComponent],
  templateUrl: './admin-partners.html'
})
export class AdminPartnersComponent implements OnInit {
  partners = signal<Partner[]>([]);
  loading = signal(true);

  constructor(
    private snackbar: SnackbarService,
    private partnersService: PartnersService,
    private confirmDialog: ConfirmDialogService,
    private translate: TranslateService
  ) {}

  ngOnInit(): void {
    this.refresh();
  }

  private refresh(): void {
    this.loading.set(true);
    this.partnersService.getAll().subscribe({
      next: (partners) => {
        this.partners.set(partners);
        this.loading.set(false);
      },
      error: () => this.loading.set(false)
    });
  }

  async deletePartner(partner: Partner): Promise<void> {
    const confirmed = await this.confirmDialog.confirm({
      title: this.translate.instant('Common.ConfirmDeleteTitle'),
      message: this.translate.instant('AdminPartners.ConfirmDelete'),
      confirmLabel: this.translate.instant('Common.Delete'),
      variant: 'danger'
    });
    if (!confirmed) return;

    this.partnersService.delete(partner.id).subscribe({
      next: () => {
        this.snackbar.success(this.translate.instant('AdminPartners.DeleteSuccess'));
        this.refresh();
      },
      error: (err) => this.snackbar.error(err?.error?.error ?? this.translate.instant('AdminPartners.DeleteFailed'))
    });
  }
}
