import { Component, OnInit, signal } from '@angular/core';
import { DatePipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { browserSupportsWebAuthn, startRegistration } from '@simplewebauthn/browser';
import { AdminPageHeaderComponent } from '../../../shared/admin/admin-page-header/admin-page-header';
import { AdminInfoCardComponent } from '../../../shared/admin/admin-info-card/admin-info-card';
import { SnackbarService } from '../../../shared/admin/snackbar/snackbar.service';
import { ConfirmDialogService } from '../../../shared/admin/confirm-dialog/confirm-dialog.service';
import { AuthService, Passkey } from '../../../core/auth.service';
import { AdminUsersService } from '../../../core/admin-users.service';
import { NoDataComponent } from '../../../shared/admin/nodata/nodata';

@Component({
  selector: 'app-admin-security',
  standalone: true,
  imports: [FormsModule, DatePipe, TranslatePipe, AdminPageHeaderComponent, AdminInfoCardComponent, NoDataComponent],
  templateUrl: './admin-security.html'
})
export class AdminSecurityComponent implements OnInit {
  passkeys = signal<Passkey[]>([]);
  loading = signal(true);
  adding = signal(false);
  addingOpen = signal(false);
  resettingTwoFactor = signal(false);
  newLabel = '';
  readonly supportsPasskeys = browserSupportsWebAuthn();
  readonly isPlatformAdmin;

  constructor(
    private authService: AuthService,
    private adminUsersService: AdminUsersService,
    private snackbar: SnackbarService,
    private confirmDialog: ConfirmDialogService,
    private translate: TranslateService
  ) {
    this.isPlatformAdmin = this.authService.isPlatformAdmin;
  }

  ngOnInit(): void {
    this.refresh();
  }

  private refresh(): void {
    this.loading.set(true);
    this.authService.listPasskeys().subscribe({
      next: (passkeys) => {
        this.passkeys.set(passkeys);
        this.loading.set(false);
      },
      error: () => this.loading.set(false)
    });
  }

  addPasskey(): void {
    const label = this.newLabel.trim();
    if (!label || this.adding()) return;

    this.adding.set(true);
    this.authService.getPasskeyRegistrationOptions().subscribe({
      next: ({ challengeId, options }) => {
        startRegistration({ optionsJSON: options as Parameters<typeof startRegistration>[0]['optionsJSON'] })
          .then((attestationResponse) => {
            this.authService.verifyPasskeyRegistration(challengeId, label, attestationResponse).subscribe({
              next: (res) => {
                this.adding.set(false);
                if (!res.success) {
                  this.snackbar.error(res.error ?? this.translate.instant('AdminSecurity.AddFailed'));
                  return;
                }
                this.newLabel = '';
                this.addingOpen.set(false);
                this.snackbar.success(this.translate.instant('AdminSecurity.AddSuccess'));
                this.refresh();
              },
              error: (err) => {
                this.adding.set(false);
                this.snackbar.error(err?.error?.error ?? this.translate.instant('AdminSecurity.AddFailed'));
              }
            });
          })
          .catch(() => this.adding.set(false));
      },
      error: (err) => {
        this.adding.set(false);
        this.snackbar.error(err?.error?.error ?? this.translate.instant('AdminSecurity.AddFailed'));
      }
    });
  }

  async deletePasskey(passkey: Passkey): Promise<void> {
    const confirmed = await this.confirmDialog.confirm({
      title: this.translate.instant('AdminSecurity.ConfirmDeleteTitle'),
      message: this.translate.instant('AdminSecurity.ConfirmDelete', { label: passkey.deviceLabel }),
      confirmLabel: this.translate.instant('Common.Delete'),
      variant: 'danger'
    });
    if (!confirmed) return;

    this.authService.deletePasskey(passkey.id).subscribe({
      next: () => {
        this.snackbar.success(this.translate.instant('AdminSecurity.DeleteSuccess'));
        this.refresh();
      },
      error: (err) => this.snackbar.error(err?.error?.error ?? this.translate.instant('AdminSecurity.DeleteFailed'))
    });
  }

  async resetMyTwoFactor(): Promise<void> {
    const myId = this.authService.currentUserId();
    if (!myId || this.resettingTwoFactor()) return;

    const confirmed = await this.confirmDialog.confirm({
      title: this.translate.instant('AdminSecurity.ConfirmResetTwoFactorTitle'),
      message: this.translate.instant('AdminSecurity.ConfirmResetTwoFactor'),
      confirmLabel: this.translate.instant('AdminSecurity.ResetTwoFactorButton'),
      variant: 'danger'
    });
    if (!confirmed) return;

    this.resettingTwoFactor.set(true);
    this.adminUsersService.resetTwoFactor(myId).subscribe({
      next: () => {
        this.resettingTwoFactor.set(false);
        this.snackbar.success(this.translate.instant('AdminSecurity.ResetTwoFactorSuccess'));
      },
      error: (err) => {
        this.resettingTwoFactor.set(false);
        this.snackbar.error(err?.error?.error ?? this.translate.instant('AdminSecurity.ResetTwoFactorFailed'));
      }
    });
  }
}
