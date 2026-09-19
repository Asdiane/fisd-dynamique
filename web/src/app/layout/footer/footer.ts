import { computed } from '@angular/core';
import { DomSanitizer } from '@angular/platform-browser';
import { EditorialService, DEFAULT_FOOTER_ITEMS } from '../../core/editorial.service';
import { Component, OnInit, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { TranslatePipe } from '@ngx-translate/core';
import { ZEFFY_URL } from '../../core/content/site-content';
import { EditionsService } from '../../core/editions.service';
import { ContactsService } from '../../core/contacts.service';
import { Contact, ContactType } from '../../core/content.models';

type SocialIcon = 'facebook' | 'x' | 'linkedin' | 'instagram' | 'youtube' | 'tiktok' | 'generic';

interface SocialLink {
  name: string;
  href: string;
  icon: SocialIcon;
}

interface ContactLink {
  label: string;
  href: string;
}

// Fallback shown until Contacts loads from the API - kept in sync with whatever is actually
// live today, same resilience pattern as the rest of the site (static default, API overrides).
const FALLBACK_SOCIAL_LINKS: SocialLink[] = [
  {
    name: 'Facebook',
    href: 'https://www.facebook.com/profile.php?id=61561514091346&mibextid=LQQJ4d',
    icon: 'facebook',
  },
  { name: 'X', href: 'https://x.com/Forumisd', icon: 'x' },
  { name: 'LinkedIn', href: 'https://www.linkedin.com/company/forumisd/', icon: 'linkedin' },
  {
    name: 'Instagram',
    href: 'https://www.instagram.com/forumisd?igsh=MTl6eHRrNzluN21leg==',
    icon: 'instagram',
  },
];
const FALLBACK_EMAIL: ContactLink = { label: 'info@fisd.ca', href: 'mailto:info@fisd.ca' };
const FALLBACK_PHONE: ContactLink = { label: '+1 514 974-1455', href: 'tel:+15149741455' };

function inferSocialIcon(href: string): SocialIcon {
  const url = href.toLowerCase();
  if (url.includes('facebook.com')) return 'facebook';
  if (url.includes('x.com') || url.includes('twitter.com')) return 'x';
  if (url.includes('linkedin.com')) return 'linkedin';
  if (url.includes('youtube.com') || url.includes('youtu.be')) return 'youtube';
  if (url.includes('tiktok.com')) return 'tiktok';
  if (url.includes('instagram.com')) return 'instagram';
  return 'generic';
}

@Component({
  selector: 'app-footer',
  standalone: true,
  imports: [RouterLink, TranslatePipe],
  templateUrl: './footer.html',
})
export class FooterComponent implements OnInit {
  readonly contactLinks = signal<ContactLink[]>([FALLBACK_EMAIL, FALLBACK_PHONE]);
  footerItems(group: string) { return (this.editorial.content().footerItems ?? DEFAULT_FOOTER_ITEMS).filter(item => item.visible && item.group === group); }
  readonly showFacebook = signal(false);
  readonly allSocialLinks = computed(() => {
    const links = [...this.socialLinks()];
    const settings = this.editorial.content();
    if (settings.tikTokUrl)
      links.push({ name: 'TikTok', href: settings.tikTokUrl, icon: 'tiktok' });
    if (settings.youTubeUrl)
      links.push({ name: 'YouTube', href: settings.youTubeUrl, icon: 'youtube' });
    return links.filter(
      (link, index) =>
        links.findIndex((x) => x.name.toLowerCase() === link.name.toLowerCase()) === index,
    );
  });
  readonly facebookUrl = computed(
    () =>
      this.socialLinks().find((s) => s.icon === 'facebook')?.href ?? FALLBACK_SOCIAL_LINKS[0].href,
  );
  readonly facebookEmbed = computed(() =>
    this.sanitizer.bypassSecurityTrustResourceUrl(
      'https://www.facebook.com/plugins/page.php?href=' +
        encodeURIComponent(this.facebookUrl()) +
        '&tabs=timeline&width=300&height=350&small_header=true&adapt_container_width=true&hide_cover=false&show_facepile=false',
    ),
  );
  currentYear = new Date().getFullYear();
  zeffyUrl = signal(ZEFFY_URL);
  socialLinks = signal<SocialLink[]>(FALLBACK_SOCIAL_LINKS);
  email = signal<ContactLink>(FALLBACK_EMAIL);
  phone = signal<ContactLink>(FALLBACK_PHONE);

  constructor(
    public editorial: EditorialService,
    private sanitizer: DomSanitizer,
    private editionsService: EditionsService,
    private contactsService: ContactsService,
  ) {}

  ngOnInit(): void {
    this.editionsService
      .resolveCurrentTicketingUrl(ZEFFY_URL)
      .subscribe((url) => this.zeffyUrl.set(url));

    // Static fallbacks above render immediately - the API only overrides them once loaded, and
    // any failure keeps whatever is already showing.
    this.contactsService.getVisible().subscribe({
      next: (contacts) => this.applyContacts(contacts),
      error: () => undefined,
    });
  }

  private applyContacts(contacts: Contact[]): void {
    const socials = contacts.filter((c) => c.type === ContactType.Social && c.href);
    {
      this.socialLinks.set(
        socials.map((c) => ({ name: c.label, href: c.href!, icon: inferSocialIcon(c.href!) })),
      );
    }

    this.contactLinks.set(contacts.filter(c => c.type !== ContactType.Social).map(c => ({
      label: c.value,
      href: c.href || (c.type === ContactType.Email ? 'mailto:' + c.value : c.type === ContactType.Phone ? 'tel:' + c.value : ''),
    })));
    const email = contacts.find((c) => c.type === ContactType.Email);
    if (email) {
      this.email.set({ label: email.value, href: email.href ?? `mailto:${email.value}` });
    }

    const phone = contacts.find((c) => c.type === ContactType.Phone);
    if (phone) {
      this.phone.set({ label: phone.value, href: phone.href ?? `tel:${phone.value}` });
    }
  }
}
