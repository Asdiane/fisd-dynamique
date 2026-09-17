import { ChangeDetectorRef, Component, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { AdminPageHeaderComponent } from '../../../shared/admin/admin-page-header/admin-page-header';
import { SnackbarService } from '../../../shared/admin/snackbar/snackbar.service';
import { AdminHelpTipComponent } from '../../../shared/admin/admin-help-tip/admin-help-tip';
import { AppSelectComponent, AppSelectOption } from '../../../shared/admin/app-select/app-select';
import { ConfirmDialogService } from '../../../shared/admin/confirm-dialog/confirm-dialog.service';
import { SouvenirsService } from '../../../core/souvenirs.service';
import { SouvenirStatus } from '../../../core/content.models';

@Component({
  selector: 'app-admin-souvenir-form',
  standalone: true,
  imports: [FormsModule, RouterLink, TranslatePipe, AdminPageHeaderComponent, AdminHelpTipComponent, AppSelectComponent],
  templateUrl: './admin-souvenir-form.html'
})
export class AdminSouvenirFormComponent implements OnInit {
  editingId: string | null = null;
  loading = signal(false);
  busy = signal(false);

  year = new Date().getFullYear();
  title = '';
  description = '';
  status: SouvenirStatus = SouvenirStatus.Published;

  get statusOptions(): AppSelectOption[] {
    return [
      { value: SouvenirStatus.Published, label: this.translate.instant('AdminSouvenirs.StatusPublished') },
      { value: SouvenirStatus.ComingSoon, label: this.translate.instant('AdminSouvenirs.StatusComingSoon') }
    ];
  }

  constructor(
    private snackbar: SnackbarService,
    private route: ActivatedRoute,
    private router: Router,
    private souvenirsService: SouvenirsService,
    private confirmDialog: ConfirmDialogService,
    private translate: TranslateService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (!id) return;

    this.editingId = id;
    this.loading.set(true);
    this.souvenirsService.getById(id).subscribe({
      next: (souvenir) => {
        this.loading.set(false);
        if (!souvenir) return;
        this.year = souvenir.year;
        this.title = souvenir.title;
        this.description = souvenir.description ?? '';
        this.status = souvenir.status;
        this.cdr.detectChanges();
      },
      error: () => this.loading.set(false)
    });
  }

  save(): void {
    if (this.busy()) return;
    if (!this.title.trim() || !this.year) {
      this.snackbar.error(this.translate.instant('AdminSouvenirs.MissingFields'));
      return;
    }

    this.busy.set(true);

    const payload = {
      year: this.year,
      title: this.title.trim(),
      description: this.description.trim() || null,
      status: this.status
    };

    const request$ = this.editingId ? this.souvenirsService.update(this.editingId, payload) : this.souvenirsService.create(payload);

    request$.subscribe({
      next: () => this.router.navigate(['/admin/souvenirs']),
      error: (err) => {
        this.busy.set(false);
        this.snackbar.error(err?.error?.error ?? this.translate.instant('AdminSouvenirs.SaveFailed'));
      }
    });
  }

  async delete(): Promise<void> {
    if (!this.editingId || this.busy()) return;

    const confirmed = await this.confirmDialog.confirm({
      title: this.translate.instant('Common.ConfirmDeleteTitle'),
      message: this.translate.instant('AdminSouvenirs.ConfirmDelete'),
      confirmLabel: this.translate.instant('Common.Delete'),
      variant: 'danger'
    });
    if (!confirmed) return;

    this.busy.set(true);
    this.souvenirsService.delete(this.editingId).subscribe({
      next: () => this.router.navigate(['/admin/souvenirs']),
      error: (err) => {
        this.busy.set(false);
        this.snackbar.error(err?.error?.error ?? this.translate.instant('AdminSouvenirs.DeleteFailed'));
      }
    });
  }
}
