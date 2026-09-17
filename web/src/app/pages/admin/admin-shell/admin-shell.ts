import { Component, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { NavigationEnd, Router, RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { filter } from 'rxjs';
import { TranslatePipe } from '@ngx-translate/core';
import { AuthService } from '../../../core/auth.service';
import { ConfirmDialogComponent } from '../../../shared/admin/confirm-dialog/confirm-dialog';
import { SnackbarComponent } from '../../../shared/admin/snackbar/snackbar';

type NavIcon =
  | 'articles'
  | 'contacts'
  | 'testimonials'
  | 'souvenirs'
  | 'program'
  | 'users'
  | 'slides'
  | 'pillars'
  | 'participants'
  | 'speakers'
  | 'partners'
  | 'engagement'
  | 'media'
  | 'security'
  | 'errorLog'
  | 'auditLog'
  | 'systemHealth'
  | 'help'
  | 'tickets'
  | 'emailHistory';
type GroupIcon = 'home-content' | 'publications' | 'resources';

interface ShellNavItem {
  labelKey: string;
  route: string;
  icon: NavIcon;
}

interface ShellNavGroup {
  labelKey: string;
  icon: GroupIcon;
  items: ShellNavItem[];
}

@Component({
  selector: 'app-admin-shell',
  standalone: true,
  imports: [
    CommonModule,
    RouterLink,
    RouterLinkActive,
    RouterOutlet,
    TranslatePipe,
    ConfirmDialogComponent,
    SnackbarComponent,
  ],
  templateUrl: './admin-shell.html',
})
export class AdminShellComponent {
  // Grouped so the sidebar reads as a handful of sections instead of one long flat list -
  // each section folds/unfolds, and opens by default if it contains the current route.
  readonly navGroups: ShellNavGroup[] = [
    {
      labelKey: 'AdminNav.GroupHomeContent',
      icon: 'home-content',
      items: [
        { labelKey: 'AdminNav.Editorial', route: '/admin/pages', icon: 'slides' },
        { labelKey: 'AdminNav.Slides', route: '/admin/diaporama', icon: 'slides' },
        { labelKey: 'AdminNav.Pillars', route: '/admin/piliers', icon: 'pillars' },
        { labelKey: 'AdminNav.Participants', route: '/admin/participants', icon: 'participants' },
        { labelKey: 'AdminNav.Speakers', route: '/admin/intervenants', icon: 'speakers' },
        { labelKey: 'AdminNav.Partners', route: '/admin/partenaires', icon: 'partners' },
        { labelKey: 'AdminNav.EngagementActions', route: '/admin/engagement', icon: 'engagement' },
      ],
    },
    {
      labelKey: 'AdminNav.GroupPublications',
      icon: 'publications',
      items: [
        { labelKey: 'AdminNav.Articles', route: '/admin/articles', icon: 'articles' },
        { labelKey: 'AdminNav.Testimonials', route: '/admin/temoignages', icon: 'testimonials' },
        { labelKey: 'AdminNav.Souvenirs', route: '/admin/souvenirs', icon: 'souvenirs' },
        { labelKey: 'AdminNav.Program', route: '/admin/editions', icon: 'program' },
      ],
    },
    {
      labelKey: 'AdminNav.GroupResources',
      icon: 'resources',
      items: [
        { labelKey: 'AdminNav.Contacts', route: '/admin/contacts', icon: 'contacts' },
        { labelKey: 'AdminNav.Media', route: '/admin/media', icon: 'media' },
      ],
    },
  ];

  readonly usersItem: ShellNavItem = {
    labelKey: 'AdminNav.Users',
    route: '/admin/users',
    icon: 'users',
  };
  readonly securityItem: ShellNavItem = {
    labelKey: 'AdminNav.Security',
    route: '/admin/securite',
    icon: 'security',
  };
  readonly helpItem: ShellNavItem = {
    labelKey: 'AdminNav.Help',
    route: '/admin/help',
    icon: 'help',
  };
  readonly ticketsItem: ShellNavItem = {
    labelKey: 'AdminNav.Tickets',
    route: '/admin/tickets',
    icon: 'tickets',
  };
  readonly errorLogsItem: ShellNavItem = {
    labelKey: 'AdminNav.ErrorLogs',
    route: '/admin/journal-erreurs',
    icon: 'errorLog',
  };
  readonly auditLogsItem: ShellNavItem = {
    labelKey: 'AdminNav.AuditLogs',
    route: '/admin/journal-audit',
    icon: 'auditLog',
  };
  readonly emailHistoryItem: ShellNavItem = {
    labelKey: 'AdminNav.EmailHistory',
    route: '/admin/email-history',
    icon: 'emailHistory',
  };
  readonly systemHealthItem: ShellNavItem = {
    labelKey: 'AdminNav.SystemHealth',
    route: '/admin/sante-systeme',
    icon: 'systemHealth',
  };

  menuOpen = signal(false);
  // Accordion - only one group is expanded at a time, matching the public header's nav dropdowns.
  private expandedGroup = signal<string | null>(null);

  constructor(
    private authService: AuthService,
    private router: Router,
  ) {
    this.expandGroupForCurrentRoute(this.router.url);
    this.router.events
      .pipe(filter((event): event is NavigationEnd => event instanceof NavigationEnd))
      .subscribe((event) => {
        this.expandGroupForCurrentRoute(event.urlAfterRedirects);
      });
  }

  private expandGroupForCurrentRoute(url: string): void {
    const group = this.navGroups.find((g) => g.items.some((item) => url.startsWith(item.route)));
    if (group) {
      this.expandedGroup.set(group.labelKey);
    }
  }

  isGroupExpanded(group: ShellNavGroup): boolean {
    return this.expandedGroup() === group.labelKey;
  }

  toggleGroup(group: ShellNavGroup): void {
    this.expandedGroup.update((current) => (current === group.labelKey ? null : group.labelKey));
  }

  get isSuperAdmin(): boolean {
    return this.authService.isSuperAdmin();
  }

  get isPlatformAdmin(): boolean {
    return this.authService.isPlatformAdmin();
  }

  get currentEmail(): string | null {
    return this.authService.currentEmail();
  }

  closeMenu(): void {
    this.menuOpen.set(false);
  }

  logout(): void {
    this.authService.signOut().subscribe(() => this.router.navigateByUrl('/admin'));
  }
}
