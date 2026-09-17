import { ChangeDetectorRef, Component, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { AdminPageHeaderComponent } from '../../../shared/admin/admin-page-header/admin-page-header';
import { SnackbarService } from '../../../shared/admin/snackbar/snackbar.service';
import { AdminHelpTipComponent } from '../../../shared/admin/admin-help-tip/admin-help-tip';
import { ConfirmDialogService } from '../../../shared/admin/confirm-dialog/confirm-dialog.service';
import { firstValueFrom } from 'rxjs';
import { PillarsService } from '../../../core/pillars.service';
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
  selector: 'app-admin-pillar-form',
  standalone: true,
  imports: [FormsModule, RouterLink, TranslatePipe, AdminPageHeaderComponent, AdminHelpTipComponent, AppSelectComponent],
  templateUrl: './admin-pillar-form.html'
})
export class AdminPillarFormComponent implements OnInit {
  editingId: string | null = null;
  loading = signal(false);
  busy = signal(false);

  activeLang = signal<'fr' | 'en'>('fr');
  titleFr = '';
  titleEn = '';
  textFr = '';
  textEn = '';
  icon = '';
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
    private pillarsService: PillarsService,
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
    this.pillarsService.getById(id).subscribe({
      next: (pillar) => {
        this.loading.set(false);
        if (!pillar) return;
        this.titleFr = pillar.titleFr;
        this.titleEn = pillar.titleEn;
        this.textFr = pillar.textFr;
        this.textEn = pillar.textEn;
        this.icon = pillar.icon;
        this.imageUrl = pillar.imageUrl;
        this.isVisible = pillar.isVisible;
        this.status = pillar.status;
        this.displayOrder = pillar.displayOrder;
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
      const uploaded = await firstValueFrom(this.mediaService.upload('pillars', this.selectedFile));
      this.imageUrl = uploaded.url;
    } catch {
      throw new Error(this.translate.instant('AdminArticles.UploadFailed'));
    }
  }

  save(): void {
    if (this.busy()) return;
    if (!this.titleFr.trim() || !this.titleEn.trim() || !this.textFr.trim() || !this.textEn.trim() || !this.icon.trim() || (!this.imageUrl && !this.selectedFile)) {
      this.snackbar.error(this.translate.instant('AdminPillars.MissingFields'));
      return;
    }

    this.busy.set(true);

    this.uploadSelectedImage()
      .then(() => {
        const payload = {
          titleFr: this.titleFr.trim(),
          titleEn: this.titleEn.trim(),
          textFr: this.textFr.trim(),
          textEn: this.textEn.trim(),
          icon: this.icon.trim(),
          imageUrl: this.imageUrl,
          isVisible: this.isVisible,
          status: this.status,
          displayOrder: this.displayOrder
        };

        const request$ = this.editingId
          ? this.pillarsService.update(this.editingId, payload)
          : this.pillarsService.create(payload);

        request$.subscribe({
          next: () => this.router.navigate(['/admin/piliers']),
          error: (err) => {
            this.busy.set(false);
            this.snackbar.error(err?.error?.error ?? this.translate.instant('AdminPillars.SaveFailed'));
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
      message: this.translate.instant('AdminPillars.ConfirmDelete'),
      confirmLabel: this.translate.instant('Common.Delete'),
      variant: 'danger'
    });
    if (!confirmed) return;

    this.busy.set(true);
    this.pillarsService.delete(this.editingId).subscribe({
      next: () => this.router.navigate(['/admin/piliers']),
      error: (err) => {
        this.busy.set(false);
        this.snackbar.error(err?.error?.error ?? this.translate.instant('AdminPillars.DeleteFailed'));
      }
    });
  }
}
