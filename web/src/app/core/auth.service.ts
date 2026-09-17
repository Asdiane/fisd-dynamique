import { computed, Injectable, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { catchError, map, Observable, of, shareReplay } from 'rxjs';
import { environment } from '../../environments/environment';
import { AdminRole } from './content.models';

export const AUTH_TOKEN_KEY = 'fisd_admin_token';

export interface TwoFactorChallenge {
  email: string;
  password: string;
  requiresSetup: boolean;
  secret?: string;
  qrUri?: string;
}

export interface SignInResult {
  error: string | null;
  isEditor: boolean;
  twoFactorChallenge?: TwoFactorChallenge;
}

interface LoginResponse {
  success: boolean;
  error?: string;
  token?: string;
  email?: string;
  role?: string;
  requiresTwoFactor?: boolean;
  requiresTwoFactorSetup?: boolean;
  twoFactorSecret?: string;
  twoFactorQrUri?: string;
}

interface MeResponse {
  id: string;
  email: string;
  role: string;
}

interface ResetPasswordResponse {
  success: boolean;
  error?: string;
}

export interface InvitationInfo {
  email: string;
  role: string;
  expiresAt: string;
}

export interface Passkey {
  id: string;
  deviceLabel: string;
  createdOn: string;
  lastUsedOn: string | null;
}

interface PasskeyOptionsResponse {
  challengeId: string;
  options: unknown;
}

interface PasskeyActionResponse {
  success: boolean;
  error?: string;
  passkey?: Passkey;
}

@Injectable({ providedIn: 'root' })
export class AuthService {
  isEditor = signal(false);
  role = signal<AdminRole | null>(null);
  currentEmail = signal<string | null>(null);
  currentUserId = signal<string | null>(null);
  isSuperAdmin = computed(() => this.role() === AdminRole.SuperAdmin || this.role() === AdminRole.PlatformAdmin);
  isPlatformAdmin = computed(() => this.role() === AdminRole.PlatformAdmin);

  private twoFactorChallenge: TwoFactorChallenge | null = null;
  // Every admin route guard calls checkAuth() on each navigation - without this cache that's a
  // fresh /api/auth/me round-trip (and a loading-spinner flicker) on every single click in the
  // admin menu. Cached for the session; cleared on sign-out or a failed check.
  private authCheck$: Observable<boolean> | null = null;

  constructor(private http: HttpClient) {
    this.checkAuth().subscribe();
  }

  getTwoFactorChallenge(): TwoFactorChallenge | null {
    return this.twoFactorChallenge;
  }

  // After accepting an invitation the new admin has a freshly-typed password but no session yet -
  // route them through the exact same mandatory 2FA setup screen a first-time login would, instead
  // of duplicating that flow.
  beginTwoFactorSetupFromInvitation(email: string, password: string, secret: string, qrUri: string): void {
    this.twoFactorChallenge = { email, password, requiresSetup: true, secret, qrUri };
  }

  clearTwoFactorChallenge(): void {
    this.twoFactorChallenge = null;
  }

  // Route guards must await the real check instead of reading the `isEditor` signal
  // synchronously - on a hard page load/refresh, the signal still holds its initial `false`
  // value until this HTTP call resolves, which would otherwise bounce a valid session through
  // the login page before the check catches up.
  checkAuth(): Observable<boolean> {
    if (!sessionStorage.getItem(AUTH_TOKEN_KEY)) {
      this.isEditor.set(false);
      this.role.set(null);
      this.currentEmail.set(null);
      this.currentUserId.set(null);
      this.authCheck$ = null;
      return of(false);
    }

    if (!this.authCheck$) {
      this.authCheck$ = this.http.get<MeResponse>(`${environment.apiUrl}/api/auth/me`).pipe(
        map((res) => {
          this.isEditor.set(true);
          this.role.set((res.role as AdminRole) ?? null);
          this.currentEmail.set(res.email ?? null);
          this.currentUserId.set(res.id ?? null);
          return true;
        }),
        catchError(() => {
          sessionStorage.removeItem(AUTH_TOKEN_KEY);
          this.isEditor.set(false);
          this.role.set(null);
          this.currentEmail.set(null);
          this.currentUserId.set(null);
          this.authCheck$ = null;
          return of(false);
        }),
        shareReplay(1)
      );
    }

    return this.authCheck$;
  }

  signIn(email: string, password: string): Observable<SignInResult> {
    return this.http.post<LoginResponse>(`${environment.apiUrl}/api/auth/login`, { email, password }).pipe(
      map((res) => {
        if (!res.success) {
          return { error: res.error ?? null, isEditor: false };
        }

        if (res.requiresTwoFactor || res.requiresTwoFactorSetup) {
          this.twoFactorChallenge = {
            email,
            password,
            requiresSetup: !!res.requiresTwoFactorSetup,
            secret: res.twoFactorSecret,
            qrUri: res.twoFactorQrUri
          };
          return { error: null, isEditor: false, twoFactorChallenge: this.twoFactorChallenge };
        }

        this.applySession(res);
        return { error: null, isEditor: true };
      }),
      // The API always answers 200 with a success/error envelope (see AuthController.Login), so
      // this branch only ever fires for a real transport failure (server down, CORS block, etc.),
      // never for bad credentials - the two are no longer confusable from the UI.
      catchError(() => {
        this.isEditor.set(false);
        this.role.set(null);
        this.currentEmail.set(null);
        return of({ error: 'Impossible de contacter le serveur. Vérifiez votre connexion.', isEditor: false });
      })
    );
  }

  enableTwoFactor(email: string, password: string, totpCode: string): Observable<SignInResult> {
    return this.completeTwoFactor(`${environment.apiUrl}/api/auth/2fa/enable`, email, password, totpCode);
  }

  verifyTwoFactor(email: string, password: string, totpCode: string): Observable<SignInResult> {
    return this.completeTwoFactor(`${environment.apiUrl}/api/auth/2fa/verify`, email, password, totpCode);
  }

  private completeTwoFactor(url: string, email: string, password: string, totpCode: string): Observable<SignInResult> {
    return this.http.post<LoginResponse>(url, { email, password, totpCode }).pipe(
      map((res) => {
        if (!res.success) {
          return { error: res.error ?? null, isEditor: false };
        }
        this.applySession(res);
        return { error: null, isEditor: true };
      }),
      catchError(() => of({ error: 'Impossible de contacter le serveur. Vérifiez votre connexion.', isEditor: false }))
    );
  }

  private applySession(res: LoginResponse): void {
    if (res.token) {
      sessionStorage.setItem(AUTH_TOKEN_KEY, res.token);
    }
    this.isEditor.set(true);
    this.role.set((res.role as AdminRole) ?? null);
    this.currentEmail.set(res.email ?? null);
    // The login response doesn't carry the admin's id - fetch /me once so
    // currentUserId is populated right away instead of only after a refresh.
    this.checkAuth().subscribe();
  }

  signOut(): Observable<void> {
    sessionStorage.removeItem(AUTH_TOKEN_KEY);
    this.isEditor.set(false);
    this.role.set(null);
    this.currentEmail.set(null);
    this.currentUserId.set(null);
    this.authCheck$ = null;
    return of(undefined);
  }

  requestPasswordReset(email: string): Observable<void> {
    return this.http.post<void>(`${environment.apiUrl}/api/auth/forgot-password`, { email }).pipe(catchError(() => of(undefined)));
  }

  resetPassword(token: string, newPassword: string): Observable<string | null> {
    return this.http.post<ResetPasswordResponse>(`${environment.apiUrl}/api/auth/reset-password`, { token, newPassword }).pipe(
      map((res) => (res.success ? null : res.error ?? 'Une erreur est survenue.')),
      catchError(() => of('Impossible de contacter le serveur. Vérifiez votre connexion.'))
    );
  }

  getInvitationInfo(token: string): Observable<InvitationInfo | null> {
    return this.http
      .get<InvitationInfo>(`${environment.apiUrl}/api/auth/invitations/${token}`)
      .pipe(catchError(() => of(null)));
  }

  acceptInvitation(token: string, password: string): Observable<{ error: string | null }> {
    return this.http.post<LoginResponse>(`${environment.apiUrl}/api/auth/invitations/accept`, { token, password }).pipe(
      map((res) => {
        if (!res.success) {
          return { error: res.error ?? 'Une erreur est survenue.' };
        }
        if (res.requiresTwoFactorSetup && res.email && res.twoFactorSecret && res.twoFactorQrUri) {
          this.beginTwoFactorSetupFromInvitation(res.email, password, res.twoFactorSecret, res.twoFactorQrUri);
        }
        return { error: null };
      }),
      catchError(() => of({ error: 'Impossible de contacter le serveur. Vérifiez votre connexion.' }))
    );
  }

  getPasskeyRegistrationOptions(): Observable<PasskeyOptionsResponse> {
    return this.http.post<PasskeyOptionsResponse>(`${environment.apiUrl}/api/auth/passkey/register/options`, {});
  }

  verifyPasskeyRegistration(challengeId: string, deviceLabel: string, attestationResponse: unknown): Observable<PasskeyActionResponse> {
    return this.http.post<PasskeyActionResponse>(`${environment.apiUrl}/api/auth/passkey/register/verify`, {
      challengeId,
      deviceLabel,
      attestationResponse
    });
  }

  listPasskeys(): Observable<Passkey[]> {
    return this.http.get<Passkey[]>(`${environment.apiUrl}/api/auth/passkey`);
  }

  deletePasskey(id: string): Observable<void> {
    return this.http.delete<void>(`${environment.apiUrl}/api/auth/passkey/${id}`);
  }

  getPasskeyLoginOptions(): Observable<PasskeyOptionsResponse> {
    return this.http.post<PasskeyOptionsResponse>(`${environment.apiUrl}/api/auth/passkey/login/options`, {});
  }

  verifyPasskeyLogin(challengeId: string, assertionResponse: unknown): Observable<SignInResult> {
    return this.http.post<LoginResponse>(`${environment.apiUrl}/api/auth/passkey/login/verify`, { challengeId, assertionResponse }).pipe(
      map((res) => {
        if (!res.success) {
          return { error: res.error ?? null, isEditor: false };
        }
        this.applySession(res);
        return { error: null, isEditor: true };
      }),
      catchError(() => of({ error: 'Impossible de contacter le serveur. Vérifiez votre connexion.', isEditor: false }))
    );
  }
}
