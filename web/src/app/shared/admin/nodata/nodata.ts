import { Component, input } from '@angular/core';
import { TranslatePipe } from '@ngx-translate/core';

@Component({
  selector: 'app-nodata',
  standalone: true,
  imports: [TranslatePipe],
  templateUrl: './nodata.html'
})
export class NoDataComponent {
  title = input('Common.NoDataTitle');
  hint = input('');
  variant = input<'empty' | 'filtered'>('empty');
}
