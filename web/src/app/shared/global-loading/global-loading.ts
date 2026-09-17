import { Component, inject } from '@angular/core';
import { LoadingService } from '../../core/loading.service';

@Component({
  selector: 'app-global-loading',
  standalone: true,
  templateUrl: './global-loading.html'
})
export class GlobalLoadingComponent {
  private loadingService = inject(LoadingService);
  isLoading = this.loadingService.isLoading;
}
