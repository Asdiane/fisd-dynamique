import { Component, OnInit, signal } from '@angular/core';
import { DatePipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { AdminPageHeaderComponent } from '../../../shared/admin/admin-page-header/admin-page-header';
import { SnackbarService } from '../../../shared/admin/snackbar/snackbar.service';
import { AdminInfoCardComponent } from '../../../shared/admin/admin-info-card/admin-info-card';
import { AppSelectComponent, AppSelectOption } from '../../../shared/admin/app-select/app-select';
import { ConfirmDialogService } from '../../../shared/admin/confirm-dialog/confirm-dialog.service';
import { AdminUsersService, PendingAdminInvitation } from '../../../core/admin-users.service';
import { AuthService } from '../../../core/auth.service';
import { AdminRole, AdminUser } from '../../../core/content.models';
import { NoDataComponent } from '../../../shared/admin/nodata/nodata';

@Component({
  selector: 'app-admin-users',
  standalone: true,
  imports: [FormsModule, RouterLink, DatePipe, TranslatePipe, AdminPageHeaderComponent, AdminInfoCardComponent, AppSelectComponent, NoDataComponent],
  templateUrl: './admin-users.html'
})
export class AdminUsersComponent implements OnInit {
  readonly AdminRole = AdminRole;
  users = signal<AdminUser[]>([]);
  loading = signal(true);
  pendingInvitations = signal<PendingAdminInvitation[]>([]);

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

  get isPlatformAdmin(): boolean {
    return this.authService.isPlatformAdmin();
  }

  constructor(
    private snackbar: SnackbarService,
    private adminUsersService: AdminUsersService,
    private authService: AuthService,
    private confirmDialog: ConfirmDialogService,
    private translate: TranslateService
  ) {}

  ngOnInit(): void {
    this.refresh();
    this.refreshPendingInvitations();
  }

  get currentUserEmail(): string | null {
    return this.authService.currentEmail();
  }

  canActOn(user: AdminUser): boolean {
    return user.role !== AdminRole.PlatformAdmin || this.isPlatformAdmin;
  }

  roleLabel(role: AdminRole): string {
    if (role === AdminRole.PlatformAdmin) return this.translate.instant('AdminUsers.RolePlatformAdmin');
    if (role === AdminRole.SuperAdmin) return this.translate.instant('AdminUsers.RoleSuperAdmin');
    return this.translate.instant('AdminUsers.RoleEditor');
  }

  private refresh(): void {
    this.loading.set(true);
    this.adminUsersService.getAll().subscribe({
      next: (users) => {
        this.users.set(users);
        this.loading.set(false);
      },
      error: () => this.loading.set(false)
    });
  }

  private refreshPendingInvitations(): void {
    this.adminUsersService.getPendingInvitations().subscribe({
      next: (invitations) => this.pendingInvitations.set(invitations)
    });
  }

  async cancelInvitation(invitation: PendingAdminInvitation): Promise<void> {
    const confirmed = await this.confirmDialog.confirm({
      title: this.translate.instant('AdminUsers.ConfirmCancelInviteTitle'),
      message: this.translate.instant('AdminUsers.ConfirmCancelInvite', { email: invitation.email }),
      confirmLabel: this.translate.instant('Common.Delete'),
      variant: 'danger'
    });
    if (!confirmed) return;

    this.adminUsersService.cancelInvitation(invitation.id).subscribe({
      next: () => {
        this.snackbar.success(this.translate.instant('AdminUsers.CancelInviteSuccess'));
        this.refreshPendingInvitations();
      },
      error: (err) => this.snackbar.error(err?.error?.error ?? this.translate.instant('AdminUsers.CancelInviteFailed'))
    });
  }

  changeRole(user: AdminUser, role: AdminRole): void {
    if (role === user.role) return;

    this.adminUsersService.updateRole(user.id, role).subscribe({
      next: () => {
        this.snackbar.success(this.translate.instant('AdminUsers.RoleUpdateSuccess'));
        this.refresh();
      },
      error: (err) => this.snackbar.error(err?.error?.error ?? this.translate.instant('AdminUsers.RoleUpdateFailed'))
    });
  }

  toggleActive(user: AdminUser): void {
    this.adminUsersService.setActive(user.id, !user.isActive).subscribe({
      next: () => {
        this.snackbar.success(
          this.translate.instant(user.isActive ? 'AdminUsers.DeactivateSuccess' : 'AdminUsers.ActivateSuccess')
        );
        this.refresh();
      },
      error: (err) => this.snackbar.error(err?.error?.error ?? this.translate.instant('AdminUsers.StatusUpdateFailed'))
    });
  }

  async resetTwoFactor(user: AdminUser): Promise<void> {
    const confirmed = await this.confirmDialog.confirm({
      title: this.translate.instant('AdminUsers.ConfirmResetTwoFactorTitle'),
      message: this.translate.instant('AdminUsers.ConfirmResetTwoFactor'),
      confirmLabel: this.translate.instant('AdminUsers.ResetTwoFactorButton'),
      variant: 'danger'
    });
    if (!confirmed) return;

    this.adminUsersService.resetTwoFactor(user.id).subscribe({
      next: () => {
        this.snackbar.success(this.translate.instant('AdminUsers.ResetTwoFactorSuccess'));
        this.refresh();
      },
      error: (err) => this.snackbar.error(err?.error?.error ?? this.translate.instant('AdminUsers.ResetTwoFactorFailed'))
    });
  }

  async deleteUser(user: AdminUser): Promise<void> {
    const confirmed = await this.confirmDialog.confirm({
      title: this.translate.instant('Common.ConfirmDeleteTitle'),
      message: this.translate.instant('AdminUsers.ConfirmDelete'),
      confirmLabel: this.translate.instant('Common.Delete'),
      variant: 'danger'
    });
    if (!confirmed) return;

    this.adminUsersService.delete(user.id).subscribe({
      next: () => {
        this.snackbar.success(this.translate.instant('AdminUsers.DeleteSuccess'));
        this.refresh();
      },
      error: (err) => this.snackbar.error(err?.error?.error ?? this.translate.instant('AdminUsers.DeleteFailed'))
    });
  }
}
