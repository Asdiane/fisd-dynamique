import { Component } from '@angular/core';
import { TranslatePipe } from '@ngx-translate/core';
import { ConfirmDialogService } from './confirm-dialog.service';

@Component({
  selector: 'app-confirm-dialog',
  standalone: true,
  imports: [TranslatePipe],
  templateUrl: './confirm-dialog.html'
})
export class ConfirmDialogComponent {
  constructor(public dialog: ConfirmDialogService) {}
}
