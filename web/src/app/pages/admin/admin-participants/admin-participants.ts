import { Component, OnInit, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { AdminPageHeaderComponent } from '../../../shared/admin/admin-page-header/admin-page-header';
import { SnackbarService } from '../../../shared/admin/snackbar/snackbar.service';
import { AdminInfoCardComponent } from '../../../shared/admin/admin-info-card/admin-info-card';
import { ConfirmDialogService } from '../../../shared/admin/confirm-dialog/confirm-dialog.service';
import { ParticipantsService } from '../../../core/participants.service';
import { AdminParticipant } from '../../../core/content.models';
import { NoDataComponent } from '../../../shared/admin/nodata/nodata';

@Component({
  selector: 'app-admin-participants',
  standalone: true,
  imports: [RouterLink, TranslatePipe, AdminPageHeaderComponent, AdminInfoCardComponent, NoDataComponent],
  templateUrl: './admin-participants.html'
})
export class AdminParticipantsComponent implements OnInit {
  participants = signal<AdminParticipant[]>([]);
  loading = signal(true);

  constructor(
    private snackbar: SnackbarService,
    private participantsService: ParticipantsService,
    private confirmDialog: ConfirmDialogService,
    private translate: TranslateService
  ) {}

  ngOnInit(): void {
    this.refresh();
  }

  private refresh(): void {
    this.loading.set(true);
    this.participantsService.getAll().subscribe({
      next: (participants) => {
        this.participants.set(participants);
        this.loading.set(false);
      },
      error: () => this.loading.set(false)
    });
  }

  async deleteParticipant(participant: AdminParticipant): Promise<void> {
    const confirmed = await this.confirmDialog.confirm({
      title: this.translate.instant('Common.ConfirmDeleteTitle'),
      message: this.translate.instant('AdminParticipants.ConfirmDelete'),
      confirmLabel: this.translate.instant('Common.Delete'),
      variant: 'danger'
    });
    if (!confirmed) return;

    this.participantsService.delete(participant.id).subscribe({
      next: () => {
        this.snackbar.success(this.translate.instant('AdminParticipants.DeleteSuccess'));
        this.refresh();
      },
      error: (err) => this.snackbar.error(err?.error?.error ?? this.translate.instant('AdminParticipants.DeleteFailed'))
    });
  }
}
