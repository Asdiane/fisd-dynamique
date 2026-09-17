import { Component, ElementRef, HostListener, OnInit, signal } from '@angular/core';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { TranslatePipe } from '@ngx-translate/core';
import { ZEFFY_URL } from '../../core/content/site-content';
import { EditionsService } from '../../core/editions.service';
import { LocaleService } from '../../core/locale.service';

interface NavLeaf {
  labelKey: string;
  label?: string;
  link?: string;
  fragment?: string;
  externalHref?: string;
}

interface NavGroup {
  id: string;
  labelKey: string;
  link?: string;
  children: NavLeaf[];
}

type NavEntry = NavLeaf | NavGroup;

function isGroup(entry: NavEntry): entry is NavGroup {
  return 'children' in entry;
}

@Component({
  selector: 'app-header',
  standalone: true,
  imports: [RouterLink, RouterLinkActive, TranslatePipe],
  templateUrl: './header.html',
})
export class HeaderComponent implements OnInit {
  readonly isGroup = isGroup;
  zeffyUrl = signal(ZEFFY_URL);

  navEntries = signal<NavEntry[]>([
    { labelKey: 'Nav.Home', link: '/' },
    {
      id: 'about',
      labelKey: 'Nav.About',
      link: '/',
      children: [
        { labelKey: 'Nav.AboutPresentation', link: '/', fragment: 'presentation' },
        { labelKey: 'Nav.AboutObjectives', link: '/', fragment: 'objectifs' },
      ],
    },
    {
      id: 'engage',
      labelKey: 'Nav.Engage',
      link: '/agir',
      children: [
        { labelKey: 'Nav.EngagePartner', link: '/agir', fragment: 'partenaire' },
        { labelKey: 'Nav.EngageStand', link: '/agir', fragment: 'stand' },
        { labelKey: 'Nav.EngageVolunteer', link: '/agir', fragment: 'benevole' },
        { labelKey: 'Nav.EngageDonate', externalHref: 'zeffy' },
      ],
    },
    { labelKey: 'Nav.Programming', link: '/programmation/2026' },
    {
      id: 'previous',
      labelKey: 'Nav.PreviousEdition',
      children: [
        { labelKey: 'Nav.PreviousEdition', link: '/galerie' },
        { labelKey: 'Nav.Report', link: '/rapport-synthese' },
      ],
    },
    { labelKey: 'Nav.News', link: '/actualites' },
    { labelKey: 'Nav.Contact', link: '/contact' },
  ]);

  menuOpen = signal(false);
  openGroupId = signal<string | null>(null);

  // Touch devices report mouse events too (a tap fires a synthetic mouseenter right before
  // click), so hover-to-open only activates for real pointers - on touch, the dropdown stays
  // click/tap-driven (the mobile accordion behavior).
  private readonly supportsHover =
    typeof window !== 'undefined' &&
    window.matchMedia('(hover: hover) and (pointer: fine)').matches;

  constructor(
    private elementRef: ElementRef<HTMLElement>,
    private editionsService: EditionsService,
    public localeService: LocaleService,
  ) {}

  ngOnInit(): void {
    this.editionsService
      .resolveCurrentTicketingUrl(ZEFFY_URL)
      .subscribe((url) => this.zeffyUrl.set(url));
  }

  @HostListener('document:click', ['$event'])
  onDocumentClick(event: MouseEvent): void {
    if (!this.elementRef.nativeElement.contains(event.target as Node)) {
      this.openGroupId.set(null);
    }
  }

  toggleMenu(): void {
    this.menuOpen.update((open) => !open);
    this.openGroupId.set(null);
  }

  closeMenu(): void {
    this.menuOpen.set(false);
    this.openGroupId.set(null);
  }

  toggleGroup(id: string): void {
    // On desktop the dropdown is already hover-driven - a click right after hover-open would
    // otherwise immediately toggle it shut again. Only let click drive it on touch devices.
    if (this.supportsHover) {
      return;
    }
    this.openGroupId.update((current) => (current === id ? null : id));
  }

  onGroupMouseEnter(id: string): void {
    if (this.supportsHover) {
      this.openGroupId.set(id);
    }
  }

  onGroupMouseLeave(): void {
    if (this.supportsHover) {
      this.openGroupId.set(null);
    }
  }
}
