import { Component, ElementRef, EventEmitter, HostListener, Input, OnDestroy, Output, ViewChild, forwardRef } from '@angular/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';

export interface AppSelectOption {
  value: unknown;
  label: string;
}

const DEFAULT_TRIGGER_CLASS = 'w-full rounded-lg border border-slate-400 bg-white p-3 text-ink';

let activeSelect: AppSelectComponent | null = null;

@Component({
  selector: 'app-select',
  standalone: true,
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => AppSelectComponent),
      multi: true
    }
  ],
  host: { class: 'block' },
  templateUrl: './app-select.html'
})
export class AppSelectComponent implements ControlValueAccessor, OnDestroy {
  @Input() options: AppSelectOption[] = [];
  @Input() triggerClass = DEFAULT_TRIGGER_CLASS;
  @Input() placeholder = '';
  @Output() valueChange = new EventEmitter<unknown>();

  @ViewChild('trigger') triggerRef?: ElementRef<HTMLButtonElement>;

  value: unknown = '';
  disabled = false;
  open = false;
  panelStyle: { top: string; left: string; width: string; maxHeight: string } = { top: '0px', left: '0px', width: '0px', maxHeight: '256px' };

  private onChange: (value: unknown) => void = () => {};
  private onTouched: () => void = () => {};

  private static readonly MIN_PANEL_WIDTH = 160;

  ngOnDestroy(): void {
    if (activeSelect === this) activeSelect = null;
  }

  get selectedLabel(): string {
    return this.options.find((o) => o.value === this.value)?.label ?? '';
  }

  writeValue(value: unknown): void {
    this.value = value;
  }

  registerOnChange(fn: (value: unknown) => void): void {
    this.onChange = fn;
  }

  registerOnTouched(fn: () => void): void {
    this.onTouched = fn;
  }

  setDisabledState(isDisabled: boolean): void {
    this.disabled = isDisabled;
  }

  toggle(): void {
    if (this.disabled) return;
    if (this.open) {
      this.close();
      return;
    }
    if (activeSelect && activeSelect !== this) activeSelect.close();
    activeSelect = this;
    this.open = true;
    this.computePanelPosition();
  }

  select(option: AppSelectOption): void {
    this.value = option.value;
    this.onChange(option.value);
    this.valueChange.emit(option.value);
    this.close();
  }

  private close(): void {
    if (activeSelect === this) activeSelect = null;
    if (!this.open) return;
    this.open = false;
    this.onTouched();
  }

  private computePanelPosition(): void {
    const button = this.triggerRef?.nativeElement;
    if (!button) return;
    const rect = button.getBoundingClientRect();
    const gap = 4;
    const rowHeight = 40;
    const visibleRows = 6;
    const spaceBelow = window.innerHeight - rect.bottom - gap - 8;
    const maxHeight = Math.max(rowHeight * 2, Math.min(rowHeight * visibleRows, spaceBelow));
    const width = Math.max(rect.width, AppSelectComponent.MIN_PANEL_WIDTH);
    this.panelStyle = {
      top: `${rect.bottom + gap}px`,
      left: `${Math.min(rect.left, window.innerWidth - width - 8)}px`,
      width: `${width}px`,
      maxHeight: `${maxHeight}px`
    };
  }

  @HostListener('document:click', ['$event'])
  onDocumentClick(event: MouseEvent): void {
    const target = event.target as Node;
    const clickedTrigger = this.triggerRef?.nativeElement.contains(target);
    const clickedPanel = !!(target as HTMLElement).closest?.('.app-select-panel');
    if (this.open && !clickedTrigger && !clickedPanel) {
      this.close();
    }
  }

  @HostListener('window:scroll')
  @HostListener('window:resize')
  onViewportChange(): void {
    this.close();
  }
}
