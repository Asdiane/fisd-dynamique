import { Component, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { AuthService, InvitationInfo } from '../../../core/auth.service';

@Component({
  selector: 'app-admin-accept-invitation',
  standalone: true,
  imports: [FormsModule, RouterLink, TranslatePipe],
  templateUrl: './admin-accept-invitation.html'
})
export class AdminAcceptInvitationComponent implements OnInit {
  token = signal<string | null>(null);
  invitation = signal<InvitationInfo | null>(null);
  loading = signal(true);
  password = '';
  confirmPassword = '';
  hidePassword = signal(true);
  submitting = signal(false);
  errorMessage = signal<string | null>(null);

  constructor(private route: ActivatedRoute, private router: Router, private authService: AuthService, private translate: TranslateService) {}

  ngOnInit(): void {
    const token = this.route.snapshot.queryParamMap.get('token');
    this.token.set(token);

    if (!token) {
      this.loading.set(false);
      return;
    }

    this.authService.getInvitationInfo(token).subscribe((info) => {
      this.invitation.set(info);
      this.loading.set(false);
    });
  }

  onSubmit(): void {
    const token = this.token();
    if (this.submitting() || !token) return;

    if (this.password !== this.confirmPassword) {
      this.errorMessage.set(this.translate.instant('AdminAcceptInvitation.PasswordMismatch'));
      return;
    }

    this.submitting.set(true);
    this.errorMessage.set(null);

    this.authService.acceptInvitation(token, this.password).subscribe(({ error }) => {
      this.submitting.set(false);
      if (error) {
        this.errorMessage.set(error);
        return;
      }
      this.router.navigateByUrl('/admin/2fa-setup');
    });
  }
}
