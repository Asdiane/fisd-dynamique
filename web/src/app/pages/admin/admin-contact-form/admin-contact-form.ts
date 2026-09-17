import { ChangeDetectorRef, Component, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { AdminPageHeaderComponent } from '../../../shared/admin/admin-page-header/admin-page-header';
import { SnackbarService } from '../../../shared/admin/snackbar/snackbar.service';
import { AdminHelpTipComponent } from '../../../shared/admin/admin-help-tip/admin-help-tip';
import { AppSelectComponent, AppSelectOption } from '../../../shared/admin/app-select/app-select';
import { ConfirmDialogService } from '../../../shared/admin/confirm-dialog/confirm-dialog.service';
import { ContactsService } from '../../../core/contacts.service';
import { ContactType, ContentStatus } from '../../../core/content.models';

@Component({
  selector: 'app-admin-contact-form',
  standalone: true,
  imports: [FormsModule, RouterLink, TranslatePipe, AdminPageHeaderComponent, AdminHelpTipComponent, AppSelectComponent],
  templateUrl: './admin-contact-form.html'
})
export class AdminContactFormComponent implements OnInit {
  editingId: string | null = null;
  loading = signal(false);
  busy = signal(false);

  type: ContactType = ContactType.Email;
  label = '';
  value = '';
  href = '';
  isVisible = true;
  status: ContentStatus = ContentStatus.Draft;
  displayOrder = 1;

  get typeOptions(): AppSelectOption[] {
    return [
      { value: ContactType.Phone, label: this.translate.instant('AdminContacts.TypePhone') },
      { value: ContactType.WhatsApp, label: this.translate.instant('AdminContacts.TypeWhatsApp') },
      { value: ContactType.Email, label: this.translate.instant('AdminContacts.TypeEmail') },
      { value: ContactType.Address, label: this.translate.instant('AdminContacts.TypeAddress') },
      { value: ContactType.Social, label: this.translate.instant('AdminContacts.TypeSocial') }
    ];
  }

  get statusOptions(): AppSelectOption[] {
    return [
      { value: ContentStatus.Draft, label: this.translate.instant('Common.StatusDraft') },
      { value: ContentStatus.Published, label: this.translate.instant('Common.StatusPublished') }
    ];
  }

  constructor(
    private snackbar: SnackbarService,
    private route: ActivatedRoute,
    private router: Router,
    private contactsService: ContactsService,
    private confirmDialog: ConfirmDialogService,
    private translate: TranslateService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (!id) return;

    this.editingId = id;
    this.loading.set(true);
    this.contactsService.getById(id).subscribe({
      next: (contact) => {
        this.loading.set(false);
        if (!contact) return;
        this.type = contact.type;
        this.label = contact.label;
        this.value = contact.value;
        this.href = contact.href ?? '';
        this.isVisible = contact.isVisible;
        this.status = contact.status;
        this.displayOrder = contact.displayOrder;
        this.cdr.detectChanges();
      },
      error: () => this.loading.set(false)
    });
  }

  save(): void {
    if (this.busy()) return;
    if (!this.label.trim() || !this.value.trim()) {
      this.snackbar.error(this.translate.instant('AdminContacts.MissingFields'));
      return;
    }

    this.busy.set(true);

    const payload = {
      type: this.type,
      label: this.label.trim(),
      value: this.value.trim(),
      href: this.href.trim() || null,
      isVisible: this.isVisible,
      status: this.status,
      displayOrder: this.displayOrder
    };

    const request$ = this.editingId ? this.contactsService.update(this.editingId, payload) : this.contactsService.create(payload);

    request$.subscribe({
      next: () => this.router.navigate(['/admin/contacts']),
      error: (err) => {
        this.busy.set(false);
        this.snackbar.error(err?.error?.error ?? this.translate.instant('AdminContacts.SaveFailed'));
      }
    });
  }

  async delete(): Promise<void> {
    if (!this.editingId || this.busy()) return;

    const confirmed = await this.confirmDialog.confirm({
      title: this.translate.instant('Common.ConfirmDeleteTitle'),
      message: this.translate.instant('AdminContacts.ConfirmDelete'),
      confirmLabel: this.translate.instant('Common.Delete'),
      variant: 'danger'
    });
    if (!confirmed) return;

    this.busy.set(true);
    this.contactsService.delete(this.editingId).subscribe({
      next: () => this.router.navigate(['/admin/contacts']),
      error: (err) => {
        this.busy.set(false);
        this.snackbar.error(err?.error?.error ?? this.translate.instant('AdminContacts.DeleteFailed'));
      }
    });
  }
}
