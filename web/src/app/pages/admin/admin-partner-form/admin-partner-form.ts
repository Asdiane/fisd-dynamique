import { ChangeDetectorRef, Component, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { firstValueFrom } from 'rxjs';
import { AdminPageHeaderComponent } from '../../../shared/admin/admin-page-header/admin-page-header';
import { SnackbarService } from '../../../shared/admin/snackbar/snackbar.service';
import { ConfirmDialogService } from '../../../shared/admin/confirm-dialog/confirm-dialog.service';
import { PartnersService } from '../../../core/partners.service';
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
  selector: 'app-admin-partner-form',
  standalone: true,
  imports: [FormsModule, RouterLink, TranslatePipe, AdminPageHeaderComponent, AppSelectComponent],
  templateUrl: './admin-partner-form.html'
})
export class AdminPartnerFormComponent implements OnInit {
  editingId: string | null = null;
  loading = signal(false);
  busy = signal(false);

  name = '';
  label = '';
  logoUrl = '';
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
    private partnersService: PartnersService,
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
    this.partnersService.getById(id).subscribe({
      next: (partner) => {
        this.loading.set(false);
        if (!partner) return;
        this.name = partner.name;
        this.label = partner.label ?? '';
        this.logoUrl = partner.logoUrl;
        this.isVisible = partner.isVisible;
        this.status = partner.status;
        this.displayOrder = partner.displayOrder;
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
    return this.localImagePreview || this.logoUrl;
  }

  private async uploadSelectedImage(): Promise<void> {
    if (!this.selectedFile) return;

    try {
      const uploaded = await firstValueFrom(this.mediaService.upload('partners', this.selectedFile));
      this.logoUrl = uploaded.url;
    } catch {
      throw new Error(this.translate.instant('AdminArticles.UploadFailed'));
    }
  }

  save(): void {
    if (this.busy()) return;
    if (!this.name.trim() || (!this.logoUrl && !this.selectedFile)) {
      this.snackbar.error(this.translate.instant('AdminPartners.MissingFields'));
      return;
    }

    this.busy.set(true);

    this.uploadSelectedImage()
      .then(() => {
        const payload = {
          name: this.name.trim(),
          label: this.label.trim() || null,
          logoUrl: this.logoUrl,
          isVisible: this.isVisible,
          status: this.status,
          displayOrder: this.displayOrder
        };

        const request$ = this.editingId
          ? this.partnersService.update(this.editingId, payload)
          : this.partnersService.create(payload);

        request$.subscribe({
          next: () => this.router.navigate(['/admin/partenaires']),
          error: (err) => {
            this.busy.set(false);
            this.snackbar.error(err?.error?.error ?? this.translate.instant('AdminPartners.SaveFailed'));
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
      message: this.translate.instant('AdminPartners.ConfirmDelete'),
      confirmLabel: this.translate.instant('Common.Delete'),
      variant: 'danger'
    });
    if (!confirmed) return;

    this.busy.set(true);
    this.partnersService.delete(this.editingId).subscribe({
      next: () => this.router.navigate(['/admin/partenaires']),
      error: (err) => {
        this.busy.set(false);
        this.snackbar.error(err?.error?.error ?? this.translate.instant('AdminPartners.DeleteFailed'));
      }
    });
  }
}
