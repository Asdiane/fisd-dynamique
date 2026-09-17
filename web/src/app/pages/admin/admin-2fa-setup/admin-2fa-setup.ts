import { Component, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { TranslatePipe } from '@ngx-translate/core';
import { AuthService } from '../../../core/auth.service';
import { QrCodeComponent } from '../../../shared/qr-code/qr-code';

@Component({
  selector: 'app-admin-2fa-setup',
  standalone: true,
  imports: [FormsModule, TranslatePipe, QrCodeComponent],
  templateUrl: './admin-2fa-setup.html'
})
export class Admin2faSetupComponent implements OnInit {
  code = '';
  errorMessage = signal<string | null>(null);
  submitting = signal(false);
  secret = '';
  qrUri = '';

  constructor(private authService: AuthService, private router: Router) {}

  ngOnInit(): void {
    const challenge = this.authService.getTwoFactorChallenge();
    if (!challenge || !challenge.requiresSetup) {
      this.router.navigateByUrl('/admin');
      return;
    }
    this.secret = challenge.secret ?? '';
    this.qrUri = challenge.qrUri ?? '';
  }

  onSubmit(): void {
    const challenge = this.authService.getTwoFactorChallenge();
    if (!challenge || this.submitting()) return;

    this.submitting.set(true);
    this.errorMessage.set(null);

    this.authService.enableTwoFactor(challenge.email, challenge.password, this.code.trim()).subscribe((result) => {
      this.submitting.set(false);
      if (result.error) {
        this.errorMessage.set(result.error);
        return;
      }
      this.authService.clearTwoFactorChallenge();
      this.router.navigateByUrl('/admin/articles');
    });
  }
}
