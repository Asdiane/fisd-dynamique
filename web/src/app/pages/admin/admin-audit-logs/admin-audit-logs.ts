import { Component, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { DatePipe } from '@angular/common';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { AdminPageHeaderComponent } from '../../../shared/admin/admin-page-header/admin-page-header';
import { AppSelectComponent, AppSelectOption } from '../../../shared/admin/app-select/app-select';
import { NoDataComponent } from '../../../shared/admin/nodata/nodata';
import { SnackbarService } from '../../../shared/admin/snackbar/snackbar.service';
import { AuditLogsService, AuditLog, AuditLogDetail } from '../../../core/audit-logs.service';

const PAGE_SIZE = 25;

@Component({
  selector: 'app-admin-audit-logs',
  standalone: true,
  imports: [FormsModule, DatePipe, TranslatePipe, AdminPageHeaderComponent, AppSelectComponent, NoDataComponent],
  templateUrl: './admin-audit-logs.html'
})
export class AdminAuditLogsComponent implements OnInit {
  logs = signal<AuditLog[]>([]);
  loading = signal(true);
  totalCount = signal(0);
  page = signal(1);
  action = '';
  expandedId = signal<number | null>(null);
  expandedDetail = signal<AuditLogDetail | null>(null);
  loadingDetail = signal(false);

  get actionOptions(): AppSelectOption[] {
    return [
      { value: '', label: this.translate.instant('AdminAuditLogs.AllActions') },
      { value: 'Added', label: this.translate.instant('AdminAuditLogs.ActionAdded') },
      { value: 'Modified', label: this.translate.instant('AdminAuditLogs.ActionModified') },
      { value: 'Deleted', label: this.translate.instant('AdminAuditLogs.ActionDeleted') }
    ];
  }

  get totalPages(): number {
    return Math.max(1, Math.ceil(this.totalCount() / PAGE_SIZE));
  }

  constructor(
    private auditLogsService: AuditLogsService,
    private snackbar: SnackbarService,
    private translate: TranslateService
  ) {}

  ngOnInit(): void {
    this.refresh();
  }

  onActionChange(): void {
    this.page.set(1);
    this.refresh();
  }

  refresh(): void {
    this.loading.set(true);
    this.auditLogsService.getAll({ action: this.action || undefined, page: this.page(), pageSize: PAGE_SIZE }).subscribe({
      next: (res) => {
        this.logs.set(res.items);
        this.totalCount.set(res.totalCount);
        this.loading.set(false);
      },
      error: (err) => {
        this.loading.set(false);
        this.snackbar.error(err?.error?.error ?? this.translate.instant('AdminAuditLogs.LoadFailed'));
      }
    });
  }

  goToPage(page: number): void {
    if (page < 1 || page > this.totalPages) return;
    this.page.set(page);
    this.refresh();
  }

  toggleDetail(log: AuditLog): void {
    if (this.expandedId() === log.id) {
      this.expandedId.set(null);
      this.expandedDetail.set(null);
      return;
    }

    this.expandedId.set(log.id);
    this.loadingDetail.set(true);
    this.auditLogsService.getDetail(log.id).subscribe({
      next: (detail) => {
        this.expandedDetail.set(detail);
        this.loadingDetail.set(false);
      },
      error: (err) => {
        this.loadingDetail.set(false);
        this.snackbar.error(err?.error?.error ?? this.translate.instant('AdminAuditLogs.LoadFailed'));
      }
    });
  }

  actionBadgeClass(action: string): string {
    switch (action) {
      case 'Added':
        return 'bg-[#ecfdf3] text-[#087443]';
      case 'Deleted':
        return 'bg-[#fef3f2] text-[#b42318]';
      default:
        return 'bg-primary-100 text-primary-700';
    }
  }

  formatJson(value: string | null | undefined): string {
    if (!value) return '';
    try {
      return JSON.stringify(JSON.parse(value), null, 2);
    } catch {
      return value;
    }
  }
}
