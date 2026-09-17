import { Component, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { DatePipe } from '@angular/common';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { AdminPageHeaderComponent } from '../../../shared/admin/admin-page-header/admin-page-header';
import { AppSelectComponent, AppSelectOption } from '../../../shared/admin/app-select/app-select';
import { NoDataComponent } from '../../../shared/admin/nodata/nodata';
import { SnackbarService } from '../../../shared/admin/snackbar/snackbar.service';
import { EmailHistoryService, EmailHistoryEntry, EmailHistoryDetail } from '../../../core/email-history.service';

const PAGE_SIZE = 25;

@Component({
  selector: 'app-admin-email-history',
  standalone: true,
  imports: [FormsModule, DatePipe, TranslatePipe, AdminPageHeaderComponent, AppSelectComponent, NoDataComponent],
  templateUrl: './admin-email-history.html'
})
export class AdminEmailHistoryComponent implements OnInit {
  entries = signal<EmailHistoryEntry[]>([]);
  loading = signal(true);
  totalCount = signal(0);
  page = signal(1);
  status = '';
  expandedId = signal<number | null>(null);
  expandedDetail = signal<EmailHistoryDetail | null>(null);
  loadingDetail = signal(false);

  get statusOptions(): AppSelectOption[] {
    return [
      { value: '', label: this.translate.instant('AdminEmailHistory.AllStatuses') },
      { value: 'sent', label: this.translate.instant('AdminEmailHistory.StatusSent') },
      { value: 'failed', label: this.translate.instant('AdminEmailHistory.StatusFailed') }
    ];
  }

  get totalPages(): number {
    return Math.max(1, Math.ceil(this.totalCount() / PAGE_SIZE));
  }

  constructor(
    private emailHistoryService: EmailHistoryService,
    private snackbar: SnackbarService,
    private translate: TranslateService
  ) {}

  ngOnInit(): void {
    this.refresh();
  }

  onStatusChange(): void {
    this.page.set(1);
    this.refresh();
  }

  refresh(): void {
    this.loading.set(true);
    this.emailHistoryService.getAll({ status: this.status || undefined, page: this.page(), pageSize: PAGE_SIZE }).subscribe({
      next: (res) => {
        this.entries.set(res.items);
        this.totalCount.set(res.totalCount);
        this.loading.set(false);
      },
      error: (err) => {
        this.loading.set(false);
        this.snackbar.error(err?.error?.error ?? this.translate.instant('AdminEmailHistory.LoadFailed'));
      }
    });
  }

  goToPage(page: number): void {
    if (page < 1 || page > this.totalPages) return;
    this.page.set(page);
    this.refresh();
  }

  toggleDetail(entry: EmailHistoryEntry): void {
    if (this.expandedId() === entry.id) {
      this.expandedId.set(null);
      this.expandedDetail.set(null);
      return;
    }

    this.expandedId.set(entry.id);
    this.loadingDetail.set(true);
    this.emailHistoryService.getDetail(entry.id).subscribe({
      next: (detail) => {
        this.expandedDetail.set(detail);
        this.loadingDetail.set(false);
      },
      error: (err) => {
        this.loadingDetail.set(false);
        this.snackbar.error(err?.error?.error ?? this.translate.instant('AdminEmailHistory.LoadFailed'));
      }
    });
  }

  statusBadgeClass(sent: boolean): string {
    return sent ? 'bg-[#ecfdf3] text-[#087443]' : 'bg-[#fef3f2] text-[#b42318]';
  }
}
