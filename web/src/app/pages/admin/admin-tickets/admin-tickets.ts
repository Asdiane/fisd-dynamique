import { Component, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { AdminPageHeaderComponent } from '../../../shared/admin/admin-page-header/admin-page-header';
import { NoDataComponent } from '../../../shared/admin/nodata/nodata';
import { SnackbarService } from '../../../shared/admin/snackbar/snackbar.service';
import { TicketsService } from '../../../core/tickets.service';
import { Ticket, TicketCategory, TicketStatus } from '../../../core/content.models';
import { AuthService } from '../../../core/auth.service';

@Component({
  selector: 'app-admin-tickets',
  standalone: true,
  imports: [FormsModule, RouterLink, TranslatePipe, AdminPageHeaderComponent, NoDataComponent],
  templateUrl: './admin-tickets.html'
})
export class AdminTicketsComponent implements OnInit {
  tickets = signal<Ticket[]>([]);
  loading = signal(true);
  busy = signal(false);

  category: TicketCategory = TicketCategory.Bug;
  title = '';
  description = '';
  selectedFile: File | null = null;

  constructor(
    private snackbar: SnackbarService,
    private ticketsService: TicketsService,
    private translate: TranslateService,
    private authService: AuthService
  ) {}

  get isPlatformAdmin(): boolean {
    return this.authService.isPlatformAdmin();
  }

  ngOnInit(): void {
    this.refresh();
  }

  private refresh(): void {
    this.loading.set(true);
    this.ticketsService.getMine().subscribe({
      next: (tickets) => {
        this.tickets.set(tickets);
        this.loading.set(false);
      },
      error: () => this.loading.set(false)
    });
  }

  onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    this.selectedFile = input.files?.[0] ?? null;
  }

  clearSelectedFile(): void {
    this.selectedFile = null;
  }

  submit(): void {
    if (this.busy()) return;
    if (!this.title.trim() || !this.description.trim()) {
      this.snackbar.error(this.translate.instant('AdminTickets.MissingFields'));
      return;
    }

    this.busy.set(true);
    const file = this.selectedFile;
    this.ticketsService.create({ category: this.category, title: this.title.trim(), description: this.description.trim() }).subscribe({
      next: (ticket) => {
        this.title = '';
        this.description = '';
        this.selectedFile = null;

        if (!file) {
          this.busy.set(false);
          this.snackbar.success(this.translate.instant('AdminTickets.SubmitSuccess'));
          this.refresh();
          return;
        }

        this.ticketsService.uploadAttachment(ticket.id, file).subscribe({
          next: () => {
            this.busy.set(false);
            this.snackbar.success(this.translate.instant('AdminTickets.SubmitSuccess'));
            this.refresh();
          },
          error: (err) => {
            this.busy.set(false);
            // The ticket itself was created successfully - only the attachment failed.
            this.snackbar.error(err?.error?.error ?? this.translate.instant('AdminTickets.AttachmentFailed'));
            this.refresh();
          }
        });
      },
      error: (err) => {
        this.busy.set(false);
        this.snackbar.error(err?.error?.error ?? this.translate.instant('AdminTickets.SubmitFailed'));
      }
    });
  }

  formatFileSize(bytes: number): string {
    if (bytes < 1024) return `${bytes} o`;
    if (bytes < 1024 * 1024) return `${(bytes / 1024).toFixed(0)} Ko`;
    return `${(bytes / (1024 * 1024)).toFixed(1)} Mo`;
  }

  statusBadgeClass(status: TicketStatus): string {
    switch (status) {
      case TicketStatus.Resolved:
        return 'bg-[#ecfdf3] text-[#087443]';
      case TicketStatus.InProgress:
        return 'bg-[#fffaeb] text-[#b54708]';
      case TicketStatus.Rejected:
        return 'bg-[#fef3f2] text-[#b42318]';
      default:
        return 'bg-surface text-muted';
    }
  }

  statusLabel(status: TicketStatus): string {
    switch (status) {
      case TicketStatus.InProgress:
        return this.translate.instant('AdminTickets.StatusInProgress');
      case TicketStatus.Resolved:
        return this.translate.instant('AdminTickets.StatusResolved');
      case TicketStatus.Rejected:
        return this.translate.instant('AdminTickets.StatusRejected');
      default:
        return this.translate.instant('AdminTickets.StatusOpen');
    }
  }
}
