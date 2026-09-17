import { ChangeDetectorRef, Component, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { AdminPageHeaderComponent } from '../../../shared/admin/admin-page-header/admin-page-header';
import { SnackbarService } from '../../../shared/admin/snackbar/snackbar.service';
import { FormStepsComponent, FormStep } from '../../../shared/admin/form-steps/form-steps';
import { ConfirmDialogService } from '../../../shared/admin/confirm-dialog/confirm-dialog.service';
import { firstValueFrom } from 'rxjs';
import { SlidesService } from '../../../core/slides.service';
import { MediaService } from '../../../core/media.service';
import { AppSelectComponent, AppSelectOption } from '../../../shared/admin/app-select/app-select';
import { ContentStatus } from '../../../core/content.models';

const ALLOWED_IMAGE_TYPES: Record<string, string> = {
  'image/jpeg': 'jpg',
  'image/png': 'png',
  'image/webp': 'webp'
};
const MAX_IMAGE_BYTES = 5 * 1024 * 1024;

@Component({
  selector: 'app-admin-slide-form',
  standalone: true,
  imports: [FormsModule, RouterLink, TranslatePipe, AdminPageHeaderComponent, FormStepsComponent, AppSelectComponent],
  templateUrl: './admin-slide-form.html'
})
export class AdminSlideFormComponent implements OnInit {
  editingId: string | null = null;
  loading = signal(false);
  busy = signal(false);
  currentStep = signal(0);

  get steps(): FormStep[] {
    return [
      { label: this.translate.instant('AdminEngagementActions.StepContent') },
      { label: this.translate.instant('AdminSpeakers.StepPhoto') },
      { label: this.translate.instant('AdminEngagementActions.StepPublish') }
    ];
  }

  activeLang = signal<'fr' | 'en'>('fr');
  kickerFr = '';
  kickerEn = '';
  titleFr = '';
  titleEn = '';
  textFr = '';
  textEn = '';
  placeFr = '';
  placeEn = '';
  focalPoint = '';
  imageUrl = '';
  localImagePreview = '';
  isVisible = true;
  status: ContentStatus = ContentStatus.Draft;
  displayOrder = 1;
  private selectedFile: File | null = null;

  get statusOptions(): AppSelectOption[] {
    return [
      { value: ContentStatus.Draft, label: this.translate.instant('Common.StatusDraft') },
      { value: ContentStatus.Published, label: this.translate.instant('Common.StatusPublished') }
    ];
  }

  constructor(
    private snackbar: SnackbarService,
    private route: ActivatedRoute,
    private router: Router,
    private slidesService: SlidesService,
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
    this.slidesService.getById(id).subscribe({
      next: (slide) => {
        this.loading.set(false);
        if (!slide) return;
        this.kickerFr = slide.kickerFr;
        this.kickerEn = slide.kickerEn;
        this.titleFr = slide.titleFr;
        this.titleEn = slide.titleEn;
        this.textFr = slide.textFr;
        this.textEn = slide.textEn;
        this.placeFr = slide.placeFr ?? '';
        this.placeEn = slide.placeEn ?? '';
        this.focalPoint = slide.focalPoint ?? '';
        this.imageUrl = slide.imageUrl;
        this.isVisible = slide.isVisible;
        this.status = slide.status;
        this.displayOrder = slide.displayOrder;
        this.cdr.detectChanges();
      },
      error: () => this.loading.set(false)
    });
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
    }
  }

  get imagePreviewUrl(): string {
    return this.localImagePreview || this.imageUrl;
  }

  private async uploadSelectedImage(): Promise<void> {
    if (!this.selectedFile) return;

    try {
      const uploaded = await firstValueFrom(this.mediaService.upload('slides', this.selectedFile));
      this.imageUrl = uploaded.url;
    } catch {
      throw new Error(this.translate.instant('AdminArticles.UploadFailed'));
    }
  }

  nextStep(): void {
    if (this.currentStep() === 0 && !this.hasRequiredContent()) {
      this.snackbar.error(this.translate.instant('AdminEngagementActions.StepMissingFields'));
      return;
    }
    if (this.currentStep() === 1 && !this.imageUrl && !this.selectedFile) {
      this.snackbar.error(this.translate.instant('AdminEngagementActions.StepMissingFields'));
      return;
    }
    this.currentStep.update((step) => step + 1);
  }

  previousStep(): void {
    this.currentStep.update((step) => Math.max(0, step - 1));
  }

  private hasRequiredContent(): boolean {
    return !!(this.kickerFr.trim() && this.kickerEn.trim() && this.titleFr.trim() && this.titleEn.trim() && this.textFr.trim() && this.textEn.trim());
  }

  save(): void {
    if (this.busy()) return;
    if (!this.hasRequiredContent() || (!this.imageUrl && !this.selectedFile)) {
      this.snackbar.error(this.translate.instant('AdminSlides.MissingFields'));
      return;
    }

    this.busy.set(true);

    this.uploadSelectedImage()
      .then(() => {
        const payload = {
          kickerFr: this.kickerFr.trim(),
          kickerEn: this.kickerEn.trim(),
          titleFr: this.titleFr.trim(),
          titleEn: this.titleEn.trim(),
          textFr: this.textFr.trim(),
          textEn: this.textEn.trim(),
          placeFr: this.placeFr.trim() || null,
          placeEn: this.placeEn.trim() || null,
          videoUrl: null,
          focalPoint: this.focalPoint.trim() || null,
          imageUrl: this.imageUrl,
          isVisible: this.isVisible,
          status: this.status,
          displayOrder: this.displayOrder
        };

        const request$ = this.editingId
          ? this.slidesService.update(this.editingId, payload)
          : this.slidesService.create(payload);

        request$.subscribe({
          next: () => this.router.navigate(['/admin/diaporama']),
          error: (err) => {
            this.busy.set(false);
            this.snackbar.error(err?.error?.error ?? this.translate.instant('AdminSlides.SaveFailed'));
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
      message: this.translate.instant('AdminSlides.ConfirmDelete'),
      confirmLabel: this.translate.instant('Common.Delete'),
      variant: 'danger'
    });
    if (!confirmed) return;

    this.busy.set(true);
    this.slidesService.delete(this.editingId).subscribe({
      next: () => this.router.navigate(['/admin/diaporama']),
      error: (err) => {
        this.busy.set(false);
        this.snackbar.error(err?.error?.error ?? this.translate.instant('AdminSlides.DeleteFailed'));
      }
    });
  }
}
