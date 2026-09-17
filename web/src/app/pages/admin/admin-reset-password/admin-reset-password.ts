import { Component, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { TranslatePipe } from '@ngx-translate/core';
import { AuthService } from '../../../core/auth.service';

@Component({
  selector: 'app-admin-reset-password',
  standalone: true,
  imports: [FormsModule, RouterLink, TranslatePipe],
  templateUrl: './admin-reset-password.html'
})
export class AdminResetPasswordComponent implements OnInit {
  token = signal<string | null>(null);
  password = '';
  hidePassword = signal(true);
  submitting = signal(false);
  success = signal(false);
  errorMessage = signal<string | null>(null);

  constructor(private route: ActivatedRoute, private authService: AuthService) {}

  ngOnInit(): void {
    this.token.set(this.route.snapshot.queryParamMap.get('token'));
  }

  onSubmit(): void {
    const token = this.token();
    if (this.submitting() || !token) return;
    this.submitting.set(true);
    this.errorMessage.set(null);

    this.authService.resetPassword(token, this.password).subscribe((error) => {
      this.submitting.set(false);
      if (error) {
        this.errorMessage.set(error);
        return;
      }
      this.success.set(true);
    });
  }
}
