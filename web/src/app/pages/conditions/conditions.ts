import { Component, OnInit, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { TranslatePipe } from '@ngx-translate/core';
import { PageHeroComponent } from '../../shared/page-hero/page-hero';
import { ScrollRevealDirective } from '../../shared/directives/scroll-reveal.directive';
import { CollapsibleSectionComponent } from '../../shared/collapsible-section/collapsible-section';
import { ZEFFY_URL } from '../../core/content/site-content';
import { EditionsService } from '../../core/editions.service';

@Component({
  selector: 'app-conditions',
  standalone: true,
  imports: [RouterLink, TranslatePipe, PageHeroComponent, ScrollRevealDirective, CollapsibleSectionComponent],
  templateUrl: './conditions.html'
})
export class ConditionsComponent implements OnInit {
  zeffyUrl = signal(ZEFFY_URL);

  constructor(private editionsService: EditionsService) {}

  ngOnInit(): void {
    this.editionsService.resolveCurrentTicketingUrl(ZEFFY_URL).subscribe((url) => this.zeffyUrl.set(url));
  }
}
