import { Component, OnInit, signal } from '@angular/core';
import { DatePipe } from '@angular/common';
import { RouterLink } from '@angular/router';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { AdminPageHeaderComponent } from '../../../shared/admin/admin-page-header/admin-page-header';
import { SnackbarService } from '../../../shared/admin/snackbar/snackbar.service';
import { SystemHealthService, SystemHealth } from '../../../core/system-health.service';

@Component({
  selector: 'app-admin-system-health',
  standalone: true,
  imports: [DatePipe, RouterLink, TranslatePipe, AdminPageHeaderComponent],
  templateUrl: './admin-system-health.html'
})
export class AdminSystemHealthComponent implements OnInit {
  health = signal<SystemHealth | null>(null);
  loading = signal(true);

  constructor(
    private systemHealthService: SystemHealthService,
    private snackbar: SnackbarService,
    private translate: TranslateService
  ) {}

  ngOnInit(): void {
    this.refresh();
  }

  refresh(): void {
    this.loading.set(true);
    this.systemHealthService.get().subscribe({
      next: (health) => {
        this.health.set(health);
        this.loading.set(false);
      },
      error: (err) => {
        this.loading.set(false);
        this.snackbar.error(err?.error?.error ?? this.translate.instant('AdminSystemHealth.LoadFailed'));
      }
    });
  }
}
