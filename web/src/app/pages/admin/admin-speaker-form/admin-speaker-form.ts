import { ChangeDetectorRef, Component, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { AdminPageHeaderComponent } from '../../../shared/admin/admin-page-header/admin-page-header';
import { SnackbarService } from '../../../shared/admin/snackbar/snackbar.service';
import { AdminHelpTipComponent } from '../../../shared/admin/admin-help-tip/admin-help-tip';
import { AppSelectComponent, AppSelectOption } from '../../../shared/admin/app-select/app-select';
import { FormStepsComponent, FormStep } from '../../../shared/admin/form-steps/form-steps';
import { ConfirmDialogService } from '../../../shared/admin/confirm-dialog/confirm-dialog.service';
import { firstValueFrom } from 'rxjs';
import { SpeakersService } from '../../../core/speakers.service';
import { EditionsService } from '../../../core/editions.service';
import { MediaService } from '../../../core/media.service';
import { AdminEdition, ContentStatus } from '../../../core/content.models';

const ALLOWED_IMAGE_TYPES: Record<string, string> = {
  'image/jpeg': 'jpg',
  'image/png': 'png',
  'image/webp': 'webp'
};
const MAX_IMAGE_BYTES = 5 * 1024 * 1024;

@Component({
  selector: 'app-admin-speaker-form',
  standalone: true,
  imports: [FormsModule, RouterLink, TranslatePipe, AdminPageHeaderComponent, AdminHelpTipComponent, AppSelectComponent, FormStepsComponent],
  templateUrl: './admin-speaker-form.html'
})
export class AdminSpeakerFormComponent implements OnInit {
  editingId: string | null = null;
  loading = signal(false);
  busy = signal(false);
  currentStep = signal(0);
  editions = signal<AdminEdition[]>([]);

  get steps(): FormStep[] {
    return [
      { label: this.translate.instant('AdminSpeakers.StepProfile') },
      { label: this.translate.instant('AdminSpeakers.StepPhoto') },
      { label: this.translate.instant('AdminEngagementActions.StepPublish') }
    ];
  }

  activeLang = signal<'fr' | 'en'>('fr');
  editionId = '';
  name = '';
  roleFr = '';
  roleEn = '';
  descriptionFr = '';
  descriptionEn = '';
  imageUrl = '';
  localImagePreview = '';
  isVisible = true;
  status: ContentStatus = ContentStatus.Draft;
  displayOrder = 1;
  private selectedFile: File | null = null;

  get editionOptions(): AppSelectOption[] {
    return this.editions().map((edition) => ({ value: edition.id, label: edition.badgeFr }));
  }

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
    private speakersService: SpeakersService,
    private editionsService: EditionsService,
    private mediaService: MediaService,
    private confirmDialog: ConfirmDialogService,
    private translate: TranslateService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.editionsService.getAll().subscribe({
      next: (editions) => {
        this.editions.set(editions);
        if (!this.editingId && !this.editionId) this.editionId = editions[0]?.id ?? '';
        this.cdr.detectChanges();
      },
      error: () => undefined
    });

    const id = this.route.snapshot.paramMap.get('id');
    if (!id) return;

    this.editingId = id;
    this.loading.set(true);
    this.speakersService.getById(id).subscribe({
      next: (speaker) => {
        this.loading.set(false);
        if (!speaker) return;
        this.editionId = speaker.editionId;
        this.name = speaker.name;
        this.roleFr = speaker.roleFr;
        this.roleEn = speaker.roleEn;
        this.descriptionFr = speaker.descriptionFr;
        this.descriptionEn = speaker.descriptionEn;
        this.imageUrl = speaker.imageUrl;
        this.isVisible = speaker.isVisible;
        this.status = speaker.status;
        this.displayOrder = speaker.displayOrder;
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
      const uploaded = await firstValueFrom(this.mediaService.upload('speakers', this.selectedFile));
      this.imageUrl = uploaded.url;
    } catch {
      throw new Error(this.translate.instant('AdminArticles.UploadFailed'));
    }
  }

  private hasRequiredContent(): boolean {
    return !!(this.editionId && this.name.trim() && this.roleFr.trim() && this.roleEn.trim() && this.descriptionFr.trim() && this.descriptionEn.trim());
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

  save(): void {
    if (this.busy()) return;
    if (!this.hasRequiredContent() || (!this.imageUrl && !this.selectedFile)) {
      this.snackbar.error(this.translate.instant('AdminSpeakers.MissingFields'));
      return;
    }

    this.busy.set(true);

    this.uploadSelectedImage()
      .then(() => {
        const payload = {
          editionId: this.editionId,
          name: this.name.trim(),
          roleFr: this.roleFr.trim(),
          roleEn: this.roleEn.trim(),
          descriptionFr: this.descriptionFr.trim(),
          descriptionEn: this.descriptionEn.trim(),
          imageUrl: this.imageUrl,
          isVisible: this.isVisible,
          status: this.status,
          displayOrder: this.displayOrder
        };

        const request$ = this.editingId
          ? this.speakersService.update(this.editingId, payload)
          : this.speakersService.create(payload);

        request$.subscribe({
          next: () => this.router.navigate(['/admin/intervenants']),
          error: (err) => {
            this.busy.set(false);
            this.snackbar.error(err?.error?.error ?? this.translate.instant('AdminSpeakers.SaveFailed'));
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
      message: this.translate.instant('AdminSpeakers.ConfirmDelete'),
      confirmLabel: this.translate.instant('Common.Delete'),
      variant: 'danger'
    });
    if (!confirmed) return;

    this.busy.set(true);
    this.speakersService.delete(this.editingId).subscribe({
      next: () => this.router.navigate(['/admin/intervenants']),
      error: (err) => {
        this.busy.set(false);
        this.snackbar.error(err?.error?.error ?? this.translate.instant('AdminSpeakers.DeleteFailed'));
      }
    });
  }
}
