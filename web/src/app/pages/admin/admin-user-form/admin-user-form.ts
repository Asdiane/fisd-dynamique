import { Component, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { AdminPageHeaderComponent } from '../../../shared/admin/admin-page-header/admin-page-header';
import { AdminHelpTipComponent } from '../../../shared/admin/admin-help-tip/admin-help-tip';
import { AppSelectComponent, AppSelectOption } from '../../../shared/admin/app-select/app-select';
import { SnackbarService } from '../../../shared/admin/snackbar/snackbar.service';
import { AdminUsersService } from '../../../core/admin-users.service';
import { AuthService } from '../../../core/auth.service';
import { AdminRole } from '../../../core/content.models';

@Component({
  selector: 'app-admin-user-form',
  standalone: true,
  imports: [FormsModule, RouterLink, TranslatePipe, AdminPageHeaderComponent, AdminHelpTipComponent, AppSelectComponent],
  templateUrl: './admin-user-form.html'
})
export class AdminUserFormComponent {
  busy = signal(false);

  email = '';
  role: AdminRole = AdminRole.Editor;

  get roleOptions(): AppSelectOption[] {
    const options: AppSelectOption[] = [
      { value: AdminRole.Editor, label: this.translate.instant('AdminUsers.RoleEditor') },
      { value: AdminRole.SuperAdmin, label: this.translate.instant('AdminUsers.RoleSuperAdmin') }
    ];
    if (this.authService.isPlatformAdmin()) {
      options.push({ value: AdminRole.PlatformAdmin, label: this.translate.instant('AdminUsers.RolePlatformAdmin') });
    }
    return options;
  }

  constructor(
    private router: Router,
    private adminUsersService: AdminUsersService,
    private authService: AuthService,
    private snackbar: SnackbarService,
    private translate: TranslateService
  ) {}

  inviteUser(): void {
    if (this.busy()) return;
    if (!this.email.trim()) {
      this.snackbar.error(this.translate.instant('AdminUsers.MissingFields'));
      return;
    }

    this.busy.set(true);

    this.adminUsersService.invite({ email: this.email.trim(), role: this.role }).subscribe({
      next: () => {
        this.snackbar.success(this.translate.instant('AdminUsers.InviteSentSuccess'));
        this.router.navigate(['/admin/users']);
      },
      error: (err) => {
        this.busy.set(false);
        this.snackbar.error(err?.error?.error ?? this.translate.instant('AdminUsers.InviteFailed'));
      }
    });
  }
}
