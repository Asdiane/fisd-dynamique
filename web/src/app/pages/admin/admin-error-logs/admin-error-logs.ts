import { Component, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { DatePipe } from '@angular/common';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { AdminPageHeaderComponent } from '../../../shared/admin/admin-page-header/admin-page-header';
import { AppSelectComponent, AppSelectOption } from '../../../shared/admin/app-select/app-select';
import { NoDataComponent } from '../../../shared/admin/nodata/nodata';
import { SnackbarService } from '../../../shared/admin/snackbar/snackbar.service';
import { ErrorLogsService, ErrorLog, ErrorLogDetail } from '../../../core/error-logs.service';

const PAGE_SIZE = 25;

@Component({
  selector: 'app-admin-error-logs',
  standalone: true,
  imports: [FormsModule, DatePipe, TranslatePipe, AdminPageHeaderComponent, AppSelectComponent, NoDataComponent],
  templateUrl: './admin-error-logs.html'
})
export class AdminErrorLogsComponent implements OnInit {
  logs = signal<ErrorLog[]>([]);
  loading = signal(true);
  totalCount = signal(0);
  page = signal(1);
  level = '';
  expandedId = signal<number | null>(null);
  expandedDetail = signal<ErrorLogDetail | null>(null);
  loadingDetail = signal(false);

  get levelOptions(): AppSelectOption[] {
    return [
      { value: '', label: this.translate.instant('AdminErrorLogs.AllLevels') },
      { value: 'Warn', label: 'Warn' },
      { value: 'Error', label: 'Error' },
      { value: 'Fatal', label: 'Fatal' }
    ];
  }

  get totalPages(): number {
    return Math.max(1, Math.ceil(this.totalCount() / PAGE_SIZE));
  }

  constructor(
    private errorLogsService: ErrorLogsService,
    private snackbar: SnackbarService,
    private translate: TranslateService
  ) {}

  ngOnInit(): void {
    this.refresh();
  }

  onLevelChange(): void {
    this.page.set(1);
    this.refresh();
  }

  refresh(): void {
    this.loading.set(true);
    this.errorLogsService.getAll({ level: this.level || undefined, page: this.page(), pageSize: PAGE_SIZE }).subscribe({
      next: (res) => {
        this.logs.set(res.items);
        this.totalCount.set(res.totalCount);
        this.loading.set(false);
      },
      error: (err) => {
        this.loading.set(false);
        this.snackbar.error(err?.error?.error ?? this.translate.instant('AdminErrorLogs.LoadFailed'));
      }
    });
  }

  goToPage(page: number): void {
    if (page < 1 || page > this.totalPages) return;
    this.page.set(page);
    this.refresh();
  }

  toggleDetail(log: ErrorLog): void {
    if (this.expandedId() === log.id) {
      this.expandedId.set(null);
      this.expandedDetail.set(null);
      return;
    }

    this.expandedId.set(log.id);
    if (!log.hasException) {
      this.expandedDetail.set(null);
      return;
    }

    this.loadingDetail.set(true);
    this.errorLogsService.getDetail(log.id).subscribe({
      next: (detail) => {
        this.expandedDetail.set(detail);
        this.loadingDetail.set(false);
      },
      error: (err) => {
        this.loadingDetail.set(false);
        this.snackbar.error(err?.error?.error ?? this.translate.instant('AdminErrorLogs.LoadFailed'));
      }
    });
  }

  levelBadgeClass(level: string): string {
    switch (level) {
      case 'Fatal':
      case 'Error':
        return 'bg-[#fef3f2] text-[#b42318]';
      case 'Warn':
        return 'bg-[#fffaeb] text-[#b54708]';
      default:
        return 'bg-surface text-muted';
    }
  }
}
