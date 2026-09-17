import { Component, OnInit, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { AdminPageHeaderComponent } from '../../../shared/admin/admin-page-header/admin-page-header';
import { SnackbarService } from '../../../shared/admin/snackbar/snackbar.service';
import { AdminInfoCardComponent } from '../../../shared/admin/admin-info-card/admin-info-card';
import { ConfirmDialogService } from '../../../shared/admin/confirm-dialog/confirm-dialog.service';
import { SlidesService } from '../../../core/slides.service';
import { AdminSlide } from '../../../core/content.models';
import { NoDataComponent } from '../../../shared/admin/nodata/nodata';

@Component({
  selector: 'app-admin-slides',
  standalone: true,
  imports: [RouterLink, TranslatePipe, AdminPageHeaderComponent, AdminInfoCardComponent, NoDataComponent],
  templateUrl: './admin-slides.html'
})
export class AdminSlidesComponent implements OnInit {
  slides = signal<AdminSlide[]>([]);
  loading = signal(true);

  constructor(
    private snackbar: SnackbarService,
    private slidesService: SlidesService,
    private confirmDialog: ConfirmDialogService,
    private translate: TranslateService
  ) {}

  ngOnInit(): void {
    this.refresh();
  }

  private refresh(): void {
    this.loading.set(true);
    this.slidesService.getAll().subscribe({
      next: (slides) => {
        this.slides.set(slides);
        this.loading.set(false);
      },
      error: () => this.loading.set(false)
    });
  }

  async deleteSlide(slide: AdminSlide): Promise<void> {
    const confirmed = await this.confirmDialog.confirm({
      title: this.translate.instant('Common.ConfirmDeleteTitle'),
      message: this.translate.instant('AdminSlides.ConfirmDelete'),
      confirmLabel: this.translate.instant('Common.Delete'),
      variant: 'danger'
    });
    if (!confirmed) return;

    this.slidesService.delete(slide.id).subscribe({
      next: () => {
        this.snackbar.success(this.translate.instant('AdminSlides.DeleteSuccess'));
        this.refresh();
      },
      error: (err) => this.snackbar.error(err?.error?.error ?? this.translate.instant('AdminSlides.DeleteFailed'))
    });
  }
}
