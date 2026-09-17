import { Component, ElementRef, HostListener, input, OnDestroy, ViewChild } from '@angular/core';

type HelpPosition = 'auto' | 'above' | 'below' | 'left' | 'right';

let activePopover: AdminHelpTipComponent | null = null;

@Component({
  selector: 'app-admin-help-tip',
  standalone: true,
  templateUrl: './admin-help-tip.html',
  styleUrl: './admin-help-tip.scss'
})
export class AdminHelpTipComponent implements OnDestroy {
  text = input.required<string>();
  position = input<HelpPosition>('auto');

  @ViewChild('trigger', { read: ElementRef }) triggerRef!: ElementRef;
  @ViewChild('popover', { read: ElementRef }) popoverRef?: ElementRef<HTMLElement>;

  open = false;
  popoverTop = 0;
  popoverLeft = 0;
  resolvedPosition: 'above' | 'below' | 'left' | 'right' = 'above';

  private scrollHandler = () => this.close();

  constructor(private el: ElementRef) {}

  ngOnDestroy(): void {
    if (activePopover === this) activePopover = null;
    this.removeScrollListener();
    const popoverEl = this.popoverRef?.nativeElement as (HTMLElement & { hidePopover?: () => void }) | undefined;
    popoverEl?.hidePopover?.();
    popoverEl?.remove();
  }

  toggle(event: MouseEvent): void {
    event.stopPropagation();
    this.open ? this.close() : this.openPopover();
  }

  @HostListener('document:click', ['$event'])
  onDocumentClick(event: MouseEvent): void {
    if (this.open && !this.el.nativeElement.contains(event.target)) {
      this.close();
    }
  }

  private openPopover(): void {
    if (activePopover && activePopover !== this) activePopover.close();
    activePopover = this;

    const rect = this.triggerRef.nativeElement.getBoundingClientRect();
    const pw = 240;
    const ph = 80;
    const gap = 8;
    const pad = 8;
    const vw = window.innerWidth;
    const vh = window.innerHeight;

    const space = { above: rect.top, below: vh - rect.bottom, left: rect.left, right: vw - rect.right };

    let pos = this.position() as 'above' | 'below' | 'left' | 'right';
    if (this.position() === 'auto' || !this.fits(pos, space, pw, ph, gap)) {
      pos = this.detectBestPosition(space, pw, ph, gap);
    }

    this.resolvedPosition = pos;
    this.calcCoords(rect, pos, pw, ph, gap, pad, vw, vh);

    this.open = true;
    window.addEventListener('scroll', this.scrollHandler, true);

    setTimeout(() => {
      if (this.open && this.popoverRef) {
        const popoverEl = this.popoverRef.nativeElement as HTMLElement & { showPopover?: () => void };
        document.body.appendChild(popoverEl);
        popoverEl.showPopover?.();
      }
    });
  }

  private detectBestPosition(space: Record<string, number>, pw: number, ph: number, gap: number): 'above' | 'below' | 'left' | 'right' {
    if (space['above'] >= ph + gap) return 'above';
    if (space['below'] >= ph + gap) return 'below';
    if (space['right'] >= pw + gap) return 'right';
    if (space['left'] >= pw + gap) return 'left';
    return space['above'] >= space['below'] ? 'above' : 'below';
  }

  private fits(pos: string, space: Record<string, number>, pw: number, ph: number, gap: number): boolean {
    if (pos === 'above' || pos === 'below') return space[pos] >= ph + gap;
    return space[pos] >= pw + gap;
  }

  private calcCoords(rect: DOMRect, pos: 'above' | 'below' | 'left' | 'right', pw: number, ph: number, gap: number, pad: number, vw: number, vh: number): void {
    const cx = rect.left + rect.width / 2;
    const cy = rect.top + rect.height / 2;
    let top = 0;
    let left = 0;

    switch (pos) {
      case 'above':
        top = rect.top - ph - gap;
        left = cx - pw / 2;
        break;
      case 'below':
        top = rect.bottom + gap;
        left = cx - pw / 2;
        break;
      case 'left':
        top = cy - ph / 2;
        left = rect.left - pw - gap;
        break;
      case 'right':
        top = cy - ph / 2;
        left = rect.right + gap;
        break;
    }

    if (left < pad) left = pad;
    if (left + pw > vw - pad) left = vw - pw - pad;
    if (top < pad) top = pad;
    if (top + ph > vh - pad) top = vh - ph - pad;

    this.popoverTop = top;
    this.popoverLeft = left;
  }

  private close(): void {
    if (activePopover === this) activePopover = null;
    this.open = false;
    this.removeScrollListener();
    const popoverEl = this.popoverRef?.nativeElement as (HTMLElement & { hidePopover?: () => void }) | undefined;
    popoverEl?.hidePopover?.();
    popoverEl?.remove();
  }

  private removeScrollListener(): void {
    window.removeEventListener('scroll', this.scrollHandler, true);
  }
}
