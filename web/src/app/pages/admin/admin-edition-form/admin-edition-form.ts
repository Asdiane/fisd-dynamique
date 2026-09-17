import { ChangeDetectorRef, Component, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { AdminPageHeaderComponent } from '../../../shared/admin/admin-page-header/admin-page-header';
import { SnackbarService } from '../../../shared/admin/snackbar/snackbar.service';
import { AdminHelpTipComponent } from '../../../shared/admin/admin-help-tip/admin-help-tip';
import { FormStepsComponent, FormStep } from '../../../shared/admin/form-steps/form-steps';
import { AppSelectComponent, AppSelectOption } from '../../../shared/admin/app-select/app-select';
import { ConfirmDialogService } from '../../../shared/admin/confirm-dialog/confirm-dialog.service';
import { EditionsService } from '../../../core/editions.service';
import { ContentStatus } from '../../../core/content.models';

@Component({
  selector: 'app-admin-edition-form',
  standalone: true,
  imports: [FormsModule, RouterLink, TranslatePipe, AdminPageHeaderComponent, AdminHelpTipComponent, FormStepsComponent, AppSelectComponent],
  templateUrl: './admin-edition-form.html'
})
export class AdminEditionFormComponent implements OnInit {
  editingId: string | null = null;
  loading = signal(false);
  busy = signal(false);
  currentStep = signal(0);

  get steps(): FormStep[] {
    return [
      { label: this.translate.instant('AdminEditions.StepIdentity') },
      { label: this.translate.instant('AdminEditions.StepDatesLocation') },
      { label: this.translate.instant('AdminEngagementActions.StepPublish') }
    ];
  }

  activeLang = signal<'fr' | 'en'>('fr');
  year = new Date().getFullYear();
  badgeFr = '';
  badgeEn = '';
  titleFr = '';
  titleEn = '';
  textFr = '';
  textEn = '';
  startDate = '';
  endDate = '';
  locationLabelFr = '';
  locationLabelEn = '';
  ticketingUrl = '';
  isVisible = true;
  status: ContentStatus = ContentStatus.Draft;

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
    private editionsService: EditionsService,
    private confirmDialog: ConfirmDialogService,
    private translate: TranslateService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (!id) return;

    this.editingId = id;
    this.loading.set(true);
    this.editionsService.getById(id).subscribe({
      next: (edition) => {
        this.loading.set(false);
        if (!edition) return;
        this.year = edition.year;
        this.badgeFr = edition.badgeFr;
        this.badgeEn = edition.badgeEn;
        this.titleFr = edition.titleFr;
        this.titleEn = edition.titleEn;
        this.textFr = edition.textFr;
        this.textEn = edition.textEn;
        this.startDate = edition.startDate ? edition.startDate.substring(0, 10) : '';
        this.endDate = edition.endDate ? edition.endDate.substring(0, 10) : '';
        this.locationLabelFr = edition.locationLabelFr ?? '';
        this.locationLabelEn = edition.locationLabelEn ?? '';
        this.ticketingUrl = edition.ticketingUrl ?? '';
        this.isVisible = edition.isVisible;
        this.status = edition.status;
        this.cdr.detectChanges();
      },
      error: () => this.loading.set(false)
    });
  }

  private hasRequiredContent(): boolean {
    return !!(this.year && this.badgeFr.trim() && this.badgeEn.trim() && this.titleFr.trim() && this.titleEn.trim() && this.textFr.trim() && this.textEn.trim());
  }

  nextStep(): void {
    if (this.currentStep() === 0 && !this.hasRequiredContent()) {
      this.snackbar.error(this.translate.instant('AdminEngagementActions.StepMissingFields'));
      return;
    }
    this.currentStep.update((step) => step + 1);
  }

  previousStep(): void {
    this.currentStep.update((step) => Math.max(0, step - 1));
  }

  save(): void {
    if (this.busy()) return;
    if (!this.hasRequiredContent()) {
      this.snackbar.error(this.translate.instant('AdminEditions.MissingFields'));
      return;
    }

    this.busy.set(true);

    const payload = {
      year: this.year,
      badgeFr: this.badgeFr.trim(),
      badgeEn: this.badgeEn.trim(),
      titleFr: this.titleFr.trim(),
      titleEn: this.titleEn.trim(),
      textFr: this.textFr.trim(),
      textEn: this.textEn.trim(),
      startDate: this.startDate || null,
      endDate: this.endDate || null,
      locationLabelFr: this.locationLabelFr.trim() || null,
      locationLabelEn: this.locationLabelEn.trim() || null,
      ticketingUrl: this.ticketingUrl.trim() || null,
      isVisible: this.isVisible,
      status: this.status
    };

    const request$ = this.editingId ? this.editionsService.update(this.editingId, payload) : this.editionsService.create(payload);

    request$.subscribe({
      next: () => this.router.navigate(['/admin/editions']),
      error: (err) => {
        this.busy.set(false);
        this.snackbar.error(err?.error?.error ?? this.translate.instant('AdminEditions.SaveFailed'));
      }
    });
  }

  async delete(): Promise<void> {
    if (!this.editingId || this.busy()) return;

    const confirmed = await this.confirmDialog.confirm({
      title: this.translate.instant('Common.ConfirmDeleteTitle'),
      message: this.translate.instant('AdminEditions.ConfirmDelete'),
      confirmLabel: this.translate.instant('Common.Delete'),
      variant: 'danger'
    });
    if (!confirmed) return;

    this.busy.set(true);
    this.editionsService.delete(this.editingId).subscribe({
      next: () => this.router.navigate(['/admin/editions']),
      error: (err) => {
        this.busy.set(false);
        this.snackbar.error(err?.error?.error ?? this.translate.instant('AdminEditions.DeleteFailed'));
      }
    });
  }
}
