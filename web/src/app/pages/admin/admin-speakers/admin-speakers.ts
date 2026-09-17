import { Component, OnInit, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { AdminPageHeaderComponent } from '../../../shared/admin/admin-page-header/admin-page-header';
import { SnackbarService } from '../../../shared/admin/snackbar/snackbar.service';
import { AdminInfoCardComponent } from '../../../shared/admin/admin-info-card/admin-info-card';
import { ConfirmDialogService } from '../../../shared/admin/confirm-dialog/confirm-dialog.service';
import { SpeakersService } from '../../../core/speakers.service';
import { EditionsService } from '../../../core/editions.service';
import { AdminSpeaker, AdminEdition } from '../../../core/content.models';
import { NoDataComponent } from '../../../shared/admin/nodata/nodata';

@Component({
  selector: 'app-admin-speakers',
  standalone: true,
  imports: [RouterLink, TranslatePipe, AdminPageHeaderComponent, AdminInfoCardComponent, NoDataComponent],
  templateUrl: './admin-speakers.html'
})
export class AdminSpeakersComponent implements OnInit {
  speakers = signal<AdminSpeaker[]>([]);
  editions = signal<AdminEdition[]>([]);
  loading = signal(true);

  constructor(
    private snackbar: SnackbarService,
    private speakersService: SpeakersService,
    private editionsService: EditionsService,
    private confirmDialog: ConfirmDialogService,
    private translate: TranslateService
  ) {}

  ngOnInit(): void {
    this.editionsService.getAll().subscribe({ next: (editions) => this.editions.set(editions), error: () => undefined });
    this.refresh();
  }

  editionLabel(editionId: string): string {
    const edition = this.editions().find((e) => e.id === editionId);
    return edition ? edition.badgeFr : editionId;
  }

  private refresh(): void {
    this.loading.set(true);
    this.speakersService.getAll().subscribe({
      next: (speakers) => {
        this.speakers.set(speakers);
        this.loading.set(false);
      },
      error: () => this.loading.set(false)
    });
  }

  async deleteSpeaker(speaker: AdminSpeaker): Promise<void> {
    const confirmed = await this.confirmDialog.confirm({
      title: this.translate.instant('Common.ConfirmDeleteTitle'),
      message: this.translate.instant('AdminSpeakers.ConfirmDelete'),
      confirmLabel: this.translate.instant('Common.Delete'),
      variant: 'danger'
    });
    if (!confirmed) return;

    this.speakersService.delete(speaker.id).subscribe({
      next: () => {
        this.snackbar.success(this.translate.instant('AdminSpeakers.DeleteSuccess'));
        this.refresh();
      },
      error: (err) => this.snackbar.error(err?.error?.error ?? this.translate.instant('AdminSpeakers.DeleteFailed'))
    });
  }
}
