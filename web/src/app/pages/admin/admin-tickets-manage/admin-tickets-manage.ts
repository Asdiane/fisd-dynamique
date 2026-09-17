import { Component, OnInit, signal } from '@angular/core';
import { DatePipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { AdminPageHeaderComponent } from '../../../shared/admin/admin-page-header/admin-page-header';
import { NoDataComponent } from '../../../shared/admin/nodata/nodata';
import { SnackbarService } from '../../../shared/admin/snackbar/snackbar.service';
import { ConfirmDialogService } from '../../../shared/admin/confirm-dialog/confirm-dialog.service';
import { TicketsService } from '../../../core/tickets.service';
import { Ticket, TicketStatus } from '../../../core/content.models';

interface TicketDraft {
  status: TicketStatus;
  resolutionNote: string;
}

@Component({
  selector: 'app-admin-tickets-manage',
  standalone: true,
  imports: [FormsModule, RouterLink, DatePipe, TranslatePipe, AdminPageHeaderComponent, NoDataComponent],
  templateUrl: './admin-tickets-manage.html'
})
export class AdminTicketsManageComponent implements OnInit {
  tickets = signal<Ticket[]>([]);
  loading = signal(true);
  savingId = signal<string | null>(null);
  drafts = new Map<string, TicketDraft>();

  constructor(
    private snackbar: SnackbarService,
    private ticketsService: TicketsService,
    private confirmDialog: ConfirmDialogService,
    private translate: TranslateService
  ) {}

  ngOnInit(): void {
    this.refresh();
  }

  private refresh(): void {
    this.loading.set(true);
    this.ticketsService.getAll().subscribe({
      next: (tickets) => {
        this.tickets.set(tickets);
        this.drafts.clear();
        for (const ticket of tickets) {
          this.drafts.set(ticket.id, { status: ticket.status, resolutionNote: ticket.resolutionNote ?? '' });
        }
        this.loading.set(false);
      },
      error: () => this.loading.set(false)
    });
  }

  draftFor(ticket: Ticket): TicketDraft {
    return this.drafts.get(ticket.id)!;
  }

  save(ticket: Ticket): void {
    if (this.savingId()) return;
    const draft = this.draftFor(ticket);

    this.savingId.set(ticket.id);
    this.ticketsService.updateStatus(ticket.id, draft.status, draft.resolutionNote.trim()).subscribe({
      next: () => {
        this.savingId.set(null);
        this.snackbar.success(this.translate.instant('AdminTicketsManage.SaveSuccess'));
        this.refresh();
      },
      error: (err) => {
        this.savingId.set(null);
        this.snackbar.error(err?.error?.error ?? this.translate.instant('AdminTicketsManage.SaveFailed'));
      }
    });
  }

  async deleteTicket(ticket: Ticket): Promise<void> {
    const confirmed = await this.confirmDialog.confirm({
      title: this.translate.instant('Common.ConfirmDeleteTitle'),
      message: this.translate.instant('AdminTicketsManage.ConfirmDelete'),
      confirmLabel: this.translate.instant('Common.Delete'),
      variant: 'danger'
    });
    if (!confirmed) return;

    this.ticketsService.delete(ticket.id).subscribe({
      next: () => {
        this.snackbar.success(this.translate.instant('AdminTicketsManage.DeleteSuccess'));
        this.refresh();
      },
      error: (err) => this.snackbar.error(err?.error?.error ?? this.translate.instant('AdminTicketsManage.DeleteFailed'))
    });
  }

  categoryLabel(category: string): string {
    switch (category) {
      case 'bug':
        return this.translate.instant('AdminTickets.CategoryBug');
      case 'improvement':
        return this.translate.instant('AdminTickets.CategoryImprovement');
      default:
        return this.translate.instant('AdminTickets.CategoryOther');
    }
  }
}
