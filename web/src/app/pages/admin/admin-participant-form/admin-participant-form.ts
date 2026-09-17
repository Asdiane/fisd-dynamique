import { ChangeDetectorRef, Component, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { AdminPageHeaderComponent } from '../../../shared/admin/admin-page-header/admin-page-header';
import { SnackbarService } from '../../../shared/admin/snackbar/snackbar.service';
import { ParticipantsService } from '../../../core/participants.service';
import { ConfirmDialogService } from '../../../shared/admin/confirm-dialog/confirm-dialog.service';
import { AppSelectComponent, AppSelectOption } from '../../../shared/admin/app-select/app-select';
import { ContentStatus } from '../../../core/content.models';

@Component({
  selector: 'app-admin-participant-form',
  standalone: true,
  imports: [FormsModule, RouterLink, TranslatePipe, AdminPageHeaderComponent, AppSelectComponent],
  templateUrl: './admin-participant-form.html'
})
export class AdminParticipantFormComponent implements OnInit {
  editingId: string | null = null;
  loading = signal(false);
  busy = signal(false);

  activeLang = signal<'fr' | 'en'>('fr');
  nameFr = '';
  nameEn = '';
  descriptionFr = '';
  descriptionEn = '';
  isVisible = true;
  status: ContentStatus = ContentStatus.Draft;
  displayOrder = 1;

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
    private participantsService: ParticipantsService,
    private confirmDialog: ConfirmDialogService,
    private translate: TranslateService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (!id) return;

    this.editingId = id;
    this.loading.set(true);
    this.participantsService.getById(id).subscribe({
      next: (participant) => {
        this.loading.set(false);
        if (!participant) return;
        this.nameFr = participant.nameFr;
        this.nameEn = participant.nameEn;
        this.descriptionFr = participant.descriptionFr;
        this.descriptionEn = participant.descriptionEn;
        this.isVisible = participant.isVisible;
        this.status = participant.status;
        this.displayOrder = participant.displayOrder;
        this.cdr.detectChanges();
      },
      error: () => this.loading.set(false)
    });
  }

  save(): void {
    if (this.busy()) return;
    if (!this.nameFr.trim() || !this.nameEn.trim() || !this.descriptionFr.trim() || !this.descriptionEn.trim()) {
      this.snackbar.error(this.translate.instant('AdminParticipants.MissingFields'));
      return;
    }

    this.busy.set(true);

    const payload = {
      nameFr: this.nameFr.trim(),
      nameEn: this.nameEn.trim(),
      descriptionFr: this.descriptionFr.trim(),
      descriptionEn: this.descriptionEn.trim(),
      isVisible: this.isVisible,
      status: this.status,
      displayOrder: this.displayOrder
    };

    const request$ = this.editingId
      ? this.participantsService.update(this.editingId, payload)
      : this.participantsService.create(payload);

    request$.subscribe({
      next: () => this.router.navigate(['/admin/participants']),
      error: (err) => {
        this.busy.set(false);
        this.snackbar.error(err?.error?.error ?? this.translate.instant('AdminParticipants.SaveFailed'));
      }
    });
  }

  async delete(): Promise<void> {
    if (!this.editingId || this.busy()) return;

    const confirmed = await this.confirmDialog.confirm({
      title: this.translate.instant('Common.ConfirmDeleteTitle'),
      message: this.translate.instant('AdminParticipants.ConfirmDelete'),
      confirmLabel: this.translate.instant('Common.Delete'),
      variant: 'danger'
    });
    if (!confirmed) return;

    this.busy.set(true);
    this.participantsService.delete(this.editingId).subscribe({
      next: () => this.router.navigate(['/admin/participants']),
      error: (err) => {
        this.busy.set(false);
        this.snackbar.error(err?.error?.error ?? this.translate.instant('AdminParticipants.DeleteFailed'));
      }
    });
  }
}
