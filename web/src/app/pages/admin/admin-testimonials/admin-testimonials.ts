import { Component, OnInit, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { AdminPageHeaderComponent } from '../../../shared/admin/admin-page-header/admin-page-header';
import { SnackbarService } from '../../../shared/admin/snackbar/snackbar.service';
import { AdminInfoCardComponent } from '../../../shared/admin/admin-info-card/admin-info-card';
import { ConfirmDialogService } from '../../../shared/admin/confirm-dialog/confirm-dialog.service';
import { TestimonialsService } from '../../../core/testimonials.service';
import { AdminTestimonial } from '../../../core/content.models';
import { NoDataComponent } from '../../../shared/admin/nodata/nodata';

@Component({
  selector: 'app-admin-testimonials',
  standalone: true,
  imports: [RouterLink, TranslatePipe, AdminPageHeaderComponent, AdminInfoCardComponent, NoDataComponent],
  templateUrl: './admin-testimonials.html'
})
export class AdminTestimonialsComponent implements OnInit {
  testimonials = signal<AdminTestimonial[]>([]);
  loading = signal(true);

  constructor(
    private snackbar: SnackbarService,
    private testimonialsService: TestimonialsService,
    private confirmDialog: ConfirmDialogService,
    private translate: TranslateService
  ) {}

  ngOnInit(): void {
    this.refresh();
  }

  private refresh(): void {
    this.loading.set(true);
    this.testimonialsService.getAll().subscribe({
      next: (testimonials) => {
        this.testimonials.set(testimonials);
        this.loading.set(false);
      },
      error: () => this.loading.set(false)
    });
  }

  async deleteTestimonial(testimonial: AdminTestimonial): Promise<void> {
    const confirmed = await this.confirmDialog.confirm({
      title: this.translate.instant('Common.ConfirmDeleteTitle'),
      message: this.translate.instant('AdminTestimonials.ConfirmDelete'),
      confirmLabel: this.translate.instant('Common.Delete'),
      variant: 'danger'
    });
    if (!confirmed) return;

    this.testimonialsService.delete(testimonial.id).subscribe({
      next: () => {
        this.snackbar.success(this.translate.instant('AdminTestimonials.DeleteSuccess'));
        this.refresh();
      },
      error: (err) => this.snackbar.error(err?.error?.error ?? this.translate.instant('AdminTestimonials.DeleteFailed'))
    });
  }
}
