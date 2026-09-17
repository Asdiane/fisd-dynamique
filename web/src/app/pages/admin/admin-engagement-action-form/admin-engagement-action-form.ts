import { ChangeDetectorRef, Component, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { AdminPageHeaderComponent } from '../../../shared/admin/admin-page-header/admin-page-header';
import { SnackbarService } from '../../../shared/admin/snackbar/snackbar.service';
import { AdminHelpTipComponent } from '../../../shared/admin/admin-help-tip/admin-help-tip';
import { FormStepsComponent, FormStep } from '../../../shared/admin/form-steps/form-steps';
import { AppSelectComponent, AppSelectOption } from '../../../shared/admin/app-select/app-select';
import { ConfirmDialogService } from '../../../shared/admin/confirm-dialog/confirm-dialog.service';
import { firstValueFrom } from 'rxjs';
import { EngagementActionsService } from '../../../core/engagement-actions.service';
import { MediaService } from '../../../core/media.service';
import { ContentStatus } from '../../../core/content.models';

const ALLOWED_IMAGE_TYPES: Record<string, string> = {
  'image/jpeg': 'jpg',
  'image/png': 'png',
  'image/webp': 'webp'
};
const MAX_IMAGE_BYTES = 5 * 1024 * 1024;

@Component({
  selector: 'app-admin-engagement-action-form',
  standalone: true,
  imports: [FormsModule, RouterLink, TranslatePipe, AdminPageHeaderComponent, AdminHelpTipComponent, FormStepsComponent, AppSelectComponent],
  templateUrl: './admin-engagement-action-form.html'
})
export class AdminEngagementActionFormComponent implements OnInit {
  editingId: string | null = null;
  loading = signal(false);
  busy = signal(false);
  currentStep = signal(0);

  get steps(): FormStep[] {
    return [
      { label: this.translate.instant('AdminEngagementActions.StepContent') },
      { label: this.translate.instant('AdminEngagementActions.StepAppearance') },
      { label: this.translate.instant('AdminEngagementActions.StepPublish') }
    ];
  }

  activeLang = signal<'fr' | 'en'>('fr');
  anchor = '';
  titleFr = '';
  titleEn = '';
  textFr = '';
  textEn = '';
  icon = '';
  link = '';
  ctaFr = '';
  ctaEn = '';
  detailFr = '';
  detailEn = '';
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
    private engagementActionsService: EngagementActionsService,
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
    this.engagementActionsService.getById(id).subscribe({
      next: (action) => {
        this.loading.set(false);
        if (!action) return;
        this.anchor = action.anchor;
        this.titleFr = action.titleFr;
        this.titleEn = action.titleEn;
        this.textFr = action.textFr;
        this.textEn = action.textEn;
        this.icon = action.icon;
        this.link = action.link;
        this.ctaFr = action.ctaFr;
        this.ctaEn = action.ctaEn;
        this.detailFr = action.detailFr;
        this.detailEn = action.detailEn;
        this.imageUrl = action.imageUrl;
        this.isVisible = action.isVisible;
        this.status = action.status;
        this.displayOrder = action.displayOrder;
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

  private hasRequiredContent(): boolean {
    return !!(
      this.anchor.trim() &&
      this.titleFr.trim() &&
      this.titleEn.trim() &&
      this.textFr.trim() &&
      this.textEn.trim() &&
      this.detailFr.trim() &&
      this.detailEn.trim()
    );
  }

  private hasRequiredAppearance(): boolean {
    return !!(this.icon.trim() && this.link.trim() && this.ctaFr.trim() && this.ctaEn.trim() && (this.imageUrl || this.selectedFile));
  }

  nextStep(): void {
    if (this.currentStep() === 0 && !this.hasRequiredContent()) {
      this.snackbar.error(this.translate.instant('AdminEngagementActions.StepMissingFields'));
      return;
    }
    if (this.currentStep() === 1 && !this.hasRequiredAppearance()) {
      this.snackbar.error(this.translate.instant('AdminEngagementActions.StepMissingFields'));
      return;
    }
    this.currentStep.update((step) => step + 1);
  }

  previousStep(): void {
    this.currentStep.update((step) => Math.max(0, step - 1));
  }

  private async uploadSelectedImage(): Promise<void> {
    if (!this.selectedFile) return;

    try {
      const uploaded = await firstValueFrom(this.mediaService.upload('engagement-actions', this.selectedFile));
      this.imageUrl = uploaded.url;
    } catch {
      throw new Error(this.translate.instant('AdminArticles.UploadFailed'));
    }
  }

  save(): void {
    if (this.busy()) return;
    if (!this.hasRequiredContent() || !this.hasRequiredAppearance()) {
      this.snackbar.error(this.translate.instant('AdminEngagementActions.MissingFields'));
      return;
    }

    this.busy.set(true);

    this.uploadSelectedImage()
      .then(() => {
        const payload = {
          anchor: this.anchor.trim(),
          titleFr: this.titleFr.trim(),
          titleEn: this.titleEn.trim(),
          textFr: this.textFr.trim(),
          textEn: this.textEn.trim(),
          icon: this.icon.trim(),
          link: this.link.trim(),
          ctaFr: this.ctaFr.trim(),
          ctaEn: this.ctaEn.trim(),
          detailFr: this.detailFr.trim(),
          detailEn: this.detailEn.trim(),
          imageUrl: this.imageUrl,
          isVisible: this.isVisible,
          status: this.status,
          displayOrder: this.displayOrder
        };

        const request$ = this.editingId
          ? this.engagementActionsService.update(this.editingId, payload)
          : this.engagementActionsService.create(payload);

        request$.subscribe({
          next: () => this.router.navigate(['/admin/engagement']),
          error: (err) => {
            this.busy.set(false);
            this.snackbar.error(err?.error?.error ?? this.translate.instant('AdminEngagementActions.SaveFailed'));
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
      message: this.translate.instant('AdminEngagementActions.ConfirmDelete'),
      confirmLabel: this.translate.instant('Common.Delete'),
      variant: 'danger'
    });
    if (!confirmed) return;

    this.busy.set(true);
    this.engagementActionsService.delete(this.editingId).subscribe({
      next: () => this.router.navigate(['/admin/engagement']),
      error: (err) => {
        this.busy.set(false);
        this.snackbar.error(err?.error?.error ?? this.translate.instant('AdminEngagementActions.DeleteFailed'));
      }
    });
  }
}
