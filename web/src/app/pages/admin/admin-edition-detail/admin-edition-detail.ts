import { Component, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { EditionsService } from '../../../core/editions.service';
import { AdminEdition, ProgramDay, ScheduleItem } from '../../../core/content.models';
import { AdminPageHeaderComponent } from '../../../shared/admin/admin-page-header/admin-page-header';
import { SnackbarService } from '../../../shared/admin/snackbar/snackbar.service';
import { AdminInfoCardComponent } from '../../../shared/admin/admin-info-card/admin-info-card';
import { ConfirmDialogService } from '../../../shared/admin/confirm-dialog/confirm-dialog.service';
import { NoDataComponent } from '../../../shared/admin/nodata/nodata';

@Component({
  selector: 'app-admin-edition-detail',
  standalone: true,
  imports: [FormsModule, RouterLink, TranslatePipe, AdminPageHeaderComponent, AdminInfoCardComponent, NoDataComponent],
  templateUrl: './admin-edition-detail.html'
})
export class AdminEditionDetailComponent implements OnInit {
  edition = signal<AdminEdition | null>(null);
  loading = signal(true);
  busy = signal(false);

  days = signal<ProgramDay[]>([]);
  daysLoading = signal(false);
  dayEditorOpen = signal(false);
  currentDay: ProgramDay | null = null;
  dayLabel = '';
  dayDateLabel = '';
  dayDisplayOrder = 1;

  selectedDay = signal<ProgramDay | null>(null);
  scheduleItems = signal<ScheduleItem[]>([]);
  scheduleLoading = signal(false);
  itemEditorOpen = signal(false);
  currentItem: ScheduleItem | null = null;
  itemTime = '';
  itemTitle = '';
  itemTag = '';
  itemDetail = '';
  itemLocation = '';
  itemDisplayOrder = 1;

  constructor(
    private snackbar: SnackbarService,
    private route: ActivatedRoute,
    private editionsService: EditionsService,
    private confirmDialog: ConfirmDialogService,
    private translate: TranslateService
  ) {}

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (!id) {
      this.loading.set(false);
      return;
    }

    this.editionsService.getById(id).subscribe({
      next: (edition) => {
        this.edition.set(edition ?? null);
        this.loading.set(false);
        if (edition) this.loadDays(edition.id);
      },
      error: () => this.loading.set(false)
    });
  }

  private loadDays(editionId: string): void {
    this.daysLoading.set(true);
    this.editionsService.getDays(editionId).subscribe({
      next: (days) => {
        this.days.set(days);
        this.daysLoading.set(false);
      },
      error: () => this.daysLoading.set(false)
    });
  }

  newDay(): void {
    this.currentDay = null;
    this.dayLabel = '';
    this.dayDateLabel = '';
    this.dayDisplayOrder = this.days().length + 1;
    this.dayEditorOpen.set(true);
  }

  editDay(day: ProgramDay): void {
    this.currentDay = day;
    this.dayLabel = day.label;
    this.dayDateLabel = day.dateLabel;
    this.dayDisplayOrder = day.displayOrder;
    this.dayEditorOpen.set(true);
  }

  cancelDayEdit(): void {
    this.dayEditorOpen.set(false);
  }

  saveDay(): void {
    const edition = this.edition();
    if (!edition || this.busy()) return;
    if (!this.dayLabel.trim() || !this.dayDateLabel.trim()) {
      this.snackbar.error(this.translate.instant('AdminEditions.MissingDayFields'));
      return;
    }

    this.busy.set(true);
    const payload = { label: this.dayLabel.trim(), dateLabel: this.dayDateLabel.trim(), displayOrder: this.dayDisplayOrder };
    const request$ = this.currentDay
      ? this.editionsService.updateDay(edition.id, this.currentDay.id, payload)
      : this.editionsService.addDay(edition.id, payload);

    request$.subscribe({
      next: (day) => {
        this.busy.set(false);
        this.dayEditorOpen.set(false);
        this.days.update((days) => (this.currentDay ? days.map((d) => (d.id === day.id ? day : d)) : [...days, day]));
        this.snackbar.success(this.translate.instant('AdminEditions.DaySaveSuccess'));
      },
      error: (err) => {
        this.busy.set(false);
        this.snackbar.error(err?.error?.error ?? this.translate.instant('AdminEditions.DaySaveFailed'));
      }
    });
  }

  async deleteDay(day: ProgramDay): Promise<void> {
    const edition = this.edition();
    if (!edition) return;
    const confirmed = await this.confirmDialog.confirm({
      title: this.translate.instant('Common.ConfirmDeleteTitle'),
      message: this.translate.instant('AdminEditions.ConfirmDeleteDay'),
      confirmLabel: this.translate.instant('Common.Delete'),
      variant: 'danger'
    });
    if (!confirmed) return;

    this.editionsService.deleteDay(edition.id, day.id).subscribe({
      next: () => {
        this.days.update((days) => days.filter((d) => d.id !== day.id));
        if (this.selectedDay()?.id === day.id) this.selectDay(null);
        this.snackbar.success(this.translate.instant('AdminEditions.DayDeleteSuccess'));
      },
      error: (err) => this.snackbar.error(err?.error?.error ?? this.translate.instant('AdminEditions.DayDeleteFailed'))
    });
  }

  selectDay(day: ProgramDay | null): void {
    this.selectedDay.set(day);
    this.itemEditorOpen.set(false);
    this.scheduleItems.set([]);
    if (!day) return;

    this.scheduleLoading.set(true);
    this.editionsService.getSchedule(day.id).subscribe({
      next: (items) => {
        this.scheduleItems.set(items);
        this.scheduleLoading.set(false);
      },
      error: () => this.scheduleLoading.set(false)
    });
  }

  newScheduleItem(): void {
    this.currentItem = null;
    this.itemTime = '';
    this.itemTitle = '';
    this.itemTag = '';
    this.itemDetail = '';
    this.itemLocation = '';
    this.itemDisplayOrder = this.scheduleItems().length + 1;
    this.itemEditorOpen.set(true);
  }

  editScheduleItem(item: ScheduleItem): void {
    this.currentItem = item;
    this.itemTime = item.time;
    this.itemTitle = item.title;
    this.itemTag = item.tag;
    this.itemDetail = item.detail;
    this.itemLocation = item.location;
    this.itemDisplayOrder = item.displayOrder;
    this.itemEditorOpen.set(true);
  }

  cancelItemEdit(): void {
    this.itemEditorOpen.set(false);
  }

  saveScheduleItem(): void {
    const day = this.selectedDay();
    if (!day || this.busy()) return;
    if (!this.itemTime.trim() || !this.itemTitle.trim()) {
      this.snackbar.error(this.translate.instant('AdminEditions.MissingScheduleFields'));
      return;
    }

    this.busy.set(true);
    const payload = {
      time: this.itemTime.trim(),
      title: this.itemTitle.trim(),
      tag: this.itemTag.trim(),
      detail: this.itemDetail.trim(),
      location: this.itemLocation.trim(),
      displayOrder: this.itemDisplayOrder
    };
    const request$ = this.currentItem
      ? this.editionsService.updateScheduleItem(day.id, this.currentItem.id, payload)
      : this.editionsService.addScheduleItem(day.id, payload);

    request$.subscribe({
      next: (item) => {
        this.busy.set(false);
        this.itemEditorOpen.set(false);
        this.scheduleItems.update((items) => (this.currentItem ? items.map((i) => (i.id === item.id ? item : i)) : [...items, item]));
        this.snackbar.success(this.translate.instant('AdminEditions.ScheduleSaveSuccess'));
      },
      error: (err) => {
        this.busy.set(false);
        this.snackbar.error(err?.error?.error ?? this.translate.instant('AdminEditions.ScheduleSaveFailed'));
      }
    });
  }

  async deleteScheduleItem(item: ScheduleItem): Promise<void> {
    const day = this.selectedDay();
    if (!day) return;
    const confirmed = await this.confirmDialog.confirm({
      title: this.translate.instant('Common.ConfirmDeleteTitle'),
      message: this.translate.instant('AdminEditions.ConfirmDeleteScheduleItem'),
      confirmLabel: this.translate.instant('Common.Delete'),
      variant: 'danger'
    });
    if (!confirmed) return;

    this.editionsService.deleteScheduleItem(day.id, item.id).subscribe({
      next: () => {
        this.scheduleItems.update((items) => items.filter((i) => i.id !== item.id));
        this.snackbar.success(this.translate.instant('AdminEditions.ScheduleDeleteSuccess'));
      },
      error: (err) => this.snackbar.error(err?.error?.error ?? this.translate.instant('AdminEditions.ScheduleDeleteFailed'))
    });
  }
}
