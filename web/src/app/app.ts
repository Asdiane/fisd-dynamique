import { Component, inject, signal } from '@angular/core';
import { NavigationEnd, Router, RouterOutlet } from '@angular/router';
import { filter } from 'rxjs';
import { HeaderComponent } from './layout/header/header';
import { FooterComponent } from './layout/footer/footer';
import { GlobalLoadingComponent } from './shared/global-loading/global-loading';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, HeaderComponent, FooterComponent, GlobalLoadingComponent],
  templateUrl: './app.html'
})
export class App {
  private router = inject(Router);

  // The editorial admin area is a standalone workspace with its own header/footer,
  // matching the original site where admin.html never included the public nav.
  isAdminRoute = signal(this.router.url.startsWith('/admin'));

  constructor() {
    this.router.events.pipe(filter((event): event is NavigationEnd => event instanceof NavigationEnd)).subscribe((event) => {
      this.isAdminRoute.set(event.urlAfterRedirects.startsWith('/admin'));
    });
  }
}
