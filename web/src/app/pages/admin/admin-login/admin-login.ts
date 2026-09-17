import { Component, OnDestroy, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { browserSupportsWebAuthn, startAuthentication } from '@simplewebauthn/browser';
import { AuthService } from '../../../core/auth.service';

const PANEL_PHOTO_INTERVAL_MS = 4200;

@Component({
  selector: 'app-admin-login',
  standalone: true,
  imports: [FormsModule, RouterLink, TranslatePipe],
  templateUrl: './admin-login.html'
})
export class AdminLoginComponent implements OnInit, OnDestroy {
  email = '';
  password = '';
  errorMessage = signal<string | null>(null);
  submitting = signal(false);
  hidePassword = signal(true);
  passkeySubmitting = signal(false);
  readonly supportsPasskeys = browserSupportsWebAuthn();

  readonly panelPhotos = [
    'assets/images/edition-2024/2.jpg',
    'assets/images/edition-2025/5.jpg',
    'assets/images/edition-2025/1.jpg',
    'assets/images/edition-2025/3.jpg'
  ];
  activePhoto = signal(0);
  private panelPhotoTimer?: ReturnType<typeof setInterval>;

  constructor(private authService: AuthService, private router: Router, private translate: TranslateService) {}

  ngOnInit(): void {
    if (this.authService.isEditor()) {
      this.router.navigateByUrl('/admin/articles');
    }

    this.panelPhotoTimer = setInterval(() => {
      this.activePhoto.update((i) => (i + 1) % this.panelPhotos.length);
    }, PANEL_PHOTO_INTERVAL_MS);
  }

  ngOnDestroy(): void {
    if (this.panelPhotoTimer) {
      clearInterval(this.panelPhotoTimer);
    }
  }

  onSubmit(): void {
    if (this.submitting()) return;
    this.submitting.set(true);
    this.errorMessage.set(null);

    this.authService.signIn(this.email.trim(), this.password).subscribe(({ error, isEditor, twoFactorChallenge }) => {
      this.submitting.set(false);
      if (error) {
        this.errorMessage.set(error);
        return;
      }
      if (twoFactorChallenge) {
        this.router.navigateByUrl(twoFactorChallenge.requiresSetup ? '/admin/2fa-setup' : '/admin/2fa-verify');
        return;
      }
      if (!isEditor) {
        this.errorMessage.set(this.translate.instant('AdminLogin.NotAuthorized'));
        this.authService.signOut().subscribe();
        return;
      }
      this.router.navigateByUrl('/admin/articles');
    });
  }

  signInWithPasskey(): void {
    if (this.passkeySubmitting()) return;
    this.passkeySubmitting.set(true);
    this.errorMessage.set(null);

    this.authService.getPasskeyLoginOptions().subscribe({
      next: ({ challengeId, options }) => {
        startAuthentication({ optionsJSON: options as Parameters<typeof startAuthentication>[0]['optionsJSON'] })
          .then((assertionResponse) => {
            this.authService.verifyPasskeyLogin(challengeId, assertionResponse).subscribe(({ error, isEditor }) => {
              this.passkeySubmitting.set(false);
              if (error || !isEditor) {
                this.errorMessage.set(error ?? this.translate.instant('AdminLogin.PasskeyFailed'));
                return;
              }
              this.router.navigateByUrl('/admin/articles');
            });
          })
          .catch((err) => {
            this.passkeySubmitting.set(false);
            if (err?.name !== 'NotAllowedError') {
              this.errorMessage.set(this.translate.instant('AdminLogin.PasskeyFailed'));
            }
          });
      },
      error: () => {
        this.passkeySubmitting.set(false);
        this.errorMessage.set(this.translate.instant('AdminLogin.PasskeyFailed'));
      }
    });
  }
}
