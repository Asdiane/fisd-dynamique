import { Component, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { AdminPageHeaderComponent } from '../../../shared/admin/admin-page-header/admin-page-header';
import { SnackbarService } from '../../../shared/admin/snackbar/snackbar.service';
import { AdminInfoCardComponent } from '../../../shared/admin/admin-info-card/admin-info-card';
import { SouvenirsService } from '../../../core/souvenirs.service';
import { Souvenir, SouvenirPhoto } from '../../../core/content.models';
import { NoDataComponent } from '../../../shared/admin/nodata/nodata';

@Component({
  selector: 'app-admin-souvenir-detail',
  standalone: true,
  imports: [FormsModule, RouterLink, TranslatePipe, AdminPageHeaderComponent, AdminInfoCardComponent, NoDataComponent],
  templateUrl: './admin-souvenir-detail.html'
})
export class AdminSouvenirDetailComponent implements OnInit {
  souvenir = signal<Souvenir | null>(null);
  loading = signal(true);

  photos = signal<SouvenirPhoto[]>([]);
  photosLoading = signal(false);
  newPhotoImageUrl = '';
  newPhotoCaption = '';
  newPhotoDisplayOrder = 1;

  constructor(
    private snackbar: SnackbarService,
    private route: ActivatedRoute,
    private souvenirsService: SouvenirsService,
    private translate: TranslateService
  ) {}

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (!id) {
      this.loading.set(false);
      return;
    }

    this.souvenirsService.getById(id).subscribe({
      next: (souvenir) => {
        this.souvenir.set(souvenir ?? null);
        this.loading.set(false);
        if (souvenir) this.loadPhotos(souvenir.id);
      },
      error: () => this.loading.set(false)
    });
  }

  private loadPhotos(souvenirId: string): void {
    this.photosLoading.set(true);
    this.souvenirsService.getPhotos(souvenirId).subscribe({
      next: (photos) => {
        this.photos.set(photos);
        this.photosLoading.set(false);
        this.newPhotoDisplayOrder = photos.length + 1;
      },
      error: () => this.photosLoading.set(false)
    });
  }

  addPhoto(): void {
    const souvenir = this.souvenir();
    if (!souvenir || !this.newPhotoImageUrl.trim()) {
      this.snackbar.error(this.translate.instant('AdminSouvenirs.MissingPhotoUrl'));
      return;
    }

    this.souvenirsService
      .addPhoto(souvenir.id, {
        imageUrl: this.newPhotoImageUrl.trim(),
        caption: this.newPhotoCaption.trim() || null,
        displayOrder: this.newPhotoDisplayOrder
      })
      .subscribe({
        next: (photo) => {
          this.photos.update((photos) => [...photos, photo]);
          this.newPhotoImageUrl = '';
          this.newPhotoCaption = '';
          this.newPhotoDisplayOrder = this.photos().length + 1;
        },
        error: (err) => this.snackbar.error(err?.error?.error ?? this.translate.instant('AdminSouvenirs.PhotoAddFailed'))
      });
  }

  deletePhoto(photo: SouvenirPhoto): void {
    const souvenir = this.souvenir();
    if (!souvenir) return;

    this.souvenirsService.deletePhoto(souvenir.id, photo.id).subscribe({
      next: () => this.photos.update((photos) => photos.filter((p) => p.id !== photo.id)),
      error: (err) => this.snackbar.error(err?.error?.error ?? this.translate.instant('AdminSouvenirs.PhotoDeleteFailed'))
    });
  }
}
