import { CanDeactivateFn } from '@angular/router';

export interface ComponentWithUnsavedChanges {
  hasUnsavedChanges(): boolean;
  confirmDiscardMessage(): string;
}

export const unsavedChangesGuard: CanDeactivateFn<ComponentWithUnsavedChanges> = (component) => {
  return !component.hasUnsavedChanges() || confirm(component.confirmDiscardMessage());
};
