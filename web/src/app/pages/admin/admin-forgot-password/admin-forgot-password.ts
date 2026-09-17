import { Component, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { TranslatePipe } from '@ngx-translate/core';
import { AuthService } from '../../../core/auth.service';

@Component({
  selector: 'app-admin-forgot-password',
  standalone: true,
  imports: [FormsModule, RouterLink, TranslatePipe],
  templateUrl: './admin-forgot-password.html'
})
export class AdminForgotPasswordComponent {
  email = '';
  submitting = signal(false);
  submitted = signal(false);

  constructor(private authService: AuthService) {}

  onSubmit(): void {
    if (this.submitting() || !this.email.trim()) return;
    this.submitting.set(true);
    this.authService.requestPasswordReset(this.email.trim()).subscribe(() => {
      this.submitting.set(false);
      this.submitted.set(true);
    });
  }
}
