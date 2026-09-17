import { MediaService } from '../../../core/media.service';
import { ChangeDetectorRef, Component, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { AdminPageHeaderComponent } from '../../../shared/admin/admin-page-header/admin-page-header';
import { SnackbarService } from '../../../shared/admin/snackbar/snackbar.service';
import { ConfirmDialogService } from '../../../shared/admin/confirm-dialog/confirm-dialog.service';
import { TestimonialsService } from '../../../core/testimonials.service';
import { AppSelectComponent, AppSelectOption } from '../../../shared/admin/app-select/app-select';
import { ContentStatus } from '../../../core/content.models';

@Component({
  selector: 'app-admin-testimonial-form',
  standalone: true,
  imports: [FormsModule, RouterLink, TranslatePipe, AdminPageHeaderComponent, AppSelectComponent],
  templateUrl: './admin-testimonial-form.html',
})
export class AdminTestimonialFormComponent implements OnInit {
  editingId: string | null = null;
  loading = signal(false);
  busy = signal(false);

  activeLang = signal<'fr' | 'en'>('fr');
  imageUrl = '';
  uploading = signal(false);
  authorName = '';
  authorRoleFr = '';
  authorRoleEn = '';
  contentFr = '';
  contentEn = '';
  isVisible = true;
  status: ContentStatus = ContentStatus.Draft;
  displayOrder = 1;

  get statusOptions(): AppSelectOption[] {
    return [
      { value: ContentStatus.Draft, label: this.translate.instant('Common.StatusDraft') },
      { value: ContentStatus.Published, label: this.translate.instant('Common.StatusPublished') },
    ];
  }

  constructor(
    private mediaService: MediaService,
    private snackbar: SnackbarService,
    private route: ActivatedRoute,
    private router: Router,
    private testimonialsService: TestimonialsService,
    private confirmDialog: ConfirmDialogService,
    private translate: TranslateService,
    private cdr: ChangeDetectorRef,
  ) {}

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (!id) return;

    this.editingId = id;
    this.loading.set(true);
    this.testimonialsService.getById(id).subscribe({
      next: (testimonial) => {
        this.loading.set(false);
        if (!testimonial) return;
        this.authorName = testimonial.authorName;
        this.imageUrl = testimonial.imageUrl ?? '';
        this.authorRoleFr = testimonial.authorRoleFr ?? '';
        this.authorRoleEn = testimonial.authorRoleEn ?? '';
        this.contentFr = testimonial.contentFr;
        this.contentEn = testimonial.contentEn;
        this.isVisible = testimonial.isVisible;
        this.status = testimonial.status;
        this.displayOrder = testimonial.displayOrder;
        this.cdr.detectChanges();
      },
      error: () => this.loading.set(false),
    });
  }

  uploadImage(event: Event): void {
    const file = (event.target as HTMLInputElement).files?.[0];
    if (!file) return;
    if (
      !['image/jpeg', 'image/png', 'image/webp'].includes(file.type) ||
      file.size > 5 * 1024 * 1024
    ) {
      this.snackbar.error(this.translate.instant('AdminArticles.UploadFailed'));
      return;
    }
    this.uploading.set(true);
    this.mediaService.upload('testimonials', file).subscribe({
      next: (media) => {
        this.imageUrl = media.url;
        this.uploading.set(false);
        this.cdr.detectChanges();
      },
      error: () => {
        this.uploading.set(false);
        this.snackbar.error(this.translate.instant('AdminArticles.UploadFailed'));
      },
    });
  }
  save(): void {
    if (this.busy() || this.uploading()) return;
    if (!this.authorName.trim() || !this.contentFr.trim() || !this.contentEn.trim()) {
      this.snackbar.error(this.translate.instant('AdminTestimonials.MissingFields'));
      return;
    }

    this.busy.set(true);

    const payload = {
      imageUrl: this.imageUrl || null,
      authorName: this.authorName.trim(),
      authorRoleFr: this.authorRoleFr.trim() || null,
      authorRoleEn: this.authorRoleEn.trim() || null,
      contentFr: this.contentFr.trim(),
      contentEn: this.contentEn.trim(),
      isVisible: this.isVisible,
      status: this.status,
      displayOrder: this.displayOrder,
    };

    const request$ = this.editingId
      ? this.testimonialsService.update(this.editingId, payload)
      : this.testimonialsService.create(payload);

    request$.subscribe({
      next: () => this.router.navigate(['/admin/temoignages']),
      error: (err) => {
        this.busy.set(false);
        this.snackbar.error(
          err?.error?.error ?? this.translate.instant('AdminTestimonials.SaveFailed'),
        );
      },
    });
  }

  async delete(): Promise<void> {
    if (!this.editingId || this.busy()) return;

    const confirmed = await this.confirmDialog.confirm({
      title: this.translate.instant('Common.ConfirmDeleteTitle'),
      message: this.translate.instant('AdminTestimonials.ConfirmDelete'),
      confirmLabel: this.translate.instant('Common.Delete'),
      variant: 'danger',
    });
    if (!confirmed) return;

    this.busy.set(true);
    this.testimonialsService.delete(this.editingId).subscribe({
      next: () => this.router.navigate(['/admin/temoignages']),
      error: (err) => {
        this.busy.set(false);
        this.snackbar.error(
          err?.error?.error ?? this.translate.instant('AdminTestimonials.DeleteFailed'),
        );
      },
    });
  }
}
