import { Component, input } from '@angular/core';

export interface FormStep {
  label: string;
}

@Component({
  selector: 'app-form-steps',
  standalone: true,
  templateUrl: './form-steps.html'
})
export class FormStepsComponent {
  steps = input.required<FormStep[]>();
  current = input(0);
}
