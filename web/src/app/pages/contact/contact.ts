import { Component, OnInit, signal } from '@angular/core';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { PageHeroComponent } from '../../shared/page-hero/page-hero';
import { ScrollRevealDirective } from '../../shared/directives/scroll-reveal.directive';
import { ContactsService } from '../../core/contacts.service';
import { Contact, ContactType, ContentStatus } from '../../core/content.models';

interface ContactLink {
  href: string;
  cta: string;
  external: boolean;
}

interface ContactGroup {
  title: string;
  value: string;
  links: ContactLink[];
}

const FALLBACK_CONTACTS: Contact[] = [
  { id: 'phone', type: ContactType.Phone, label: 'Téléphone', value: '+1 514 974 1455', href: 'tel:+15149741455', isVisible: true, status: ContentStatus.Published, displayOrder: 1 },
  { id: 'whatsapp', type: ContactType.WhatsApp, label: 'WhatsApp', value: '+1 514 974 1455', href: 'https://wa.me/15149741455', isVisible: true, status: ContentStatus.Published, displayOrder: 2 },
  { id: 'email', type: ContactType.Email, label: 'Courriel', value: 'info@fisd.ca', href: 'mailto:info@fisd.ca', isVisible: true, status: ContentStatus.Published, displayOrder: 3 }
];

@Component({
  selector: 'app-contact',
  standalone: true,
  imports: [TranslatePipe, PageHeroComponent, ScrollRevealDirective],
  templateUrl: './contact.html'
})
export class ContactComponent implements OnInit {
  groups = signal<ContactGroup[]>([]);

  constructor(private contactsService: ContactsService, private translate: TranslateService) {}

  ngOnInit(): void {
    this.contactsService.getVisible().subscribe({
      next: (contacts) => this.groups.set(this.buildGroups(contacts.length > 0 ? contacts : FALLBACK_CONTACTS)),
      error: () => this.groups.set(this.buildGroups(FALLBACK_CONTACTS))
    });
  }

  private ctaFor(type: ContactType): string {
    const keys: Record<string, string> = {
      [ContactType.Phone]: 'Contact.CtaPhone',
      [ContactType.WhatsApp]: 'Contact.CtaWhatsApp',
      [ContactType.Email]: 'Contact.CtaEmail'
    };
    return this.translate.instant(keys[type] ?? 'Contact.CtaGeneric');
  }

  private normalizeValue(rawValue: string): string {
    const digitsOnly = rawValue.replace(/\D/g, '');
    return digitsOnly === '15149741455' ? '+1 514 974 1455' : rawValue;
  }

  private buildGroups(contacts: Contact[]): ContactGroup[] {
    const groups: ContactGroup[] = [];

    for (const contact of contacts) {
      const value = this.normalizeValue((contact.value ?? '').trim());
      let group = groups.find((item) => item.value.toLowerCase() === value.toLowerCase());
      if (!group) {
        group = { value, title: contact.label, links: [] };
        groups.push(group);
      }

      const href = contact.href ?? '#';
      if (!group.links.some((link) => link.href === href)) {
        group.links.push({ href, cta: this.ctaFor(contact.type), external: contact.type === ContactType.WhatsApp });
      }

      const hasPhone = group.links.some((link) => link.href.startsWith('tel:'));
      const hasWhatsApp = group.links.some((link) => link.href.includes('wa.me/'));
      if (hasPhone && hasWhatsApp) {
        group.title = this.translate.instant('Contact.PhoneAndWhatsApp');
      }
    }

    return groups;
  }
}
