import { AfterViewInit, Component, ElementRef, ViewChild, effect, input } from '@angular/core';
import * as QRCode from 'qrcode';

// The square icon mark, not the wide wordmark - a wide logo forced into a square center
// overlay gets squished and unreadable.
const LOGO_URL = 'assets/logos/fisd-icon.png';

@Component({
  selector: 'app-qr-code',
  standalone: true,
  template: `<canvas #canvas></canvas>`
})
export class QrCodeComponent implements AfterViewInit {
  data = input<string>('');
  size = input(220);

  @ViewChild('canvas') private canvasRef!: ElementRef<HTMLCanvasElement>;
  private viewReady = false;

  constructor() {
    effect(() => {
      const value = this.data();
      if (this.viewReady && value) {
        this.render(value);
      }
    });
  }

  ngAfterViewInit(): void {
    this.viewReady = true;
    if (this.data()) {
      this.render(this.data());
    }
  }

  private render(value: string): void {
    const canvas = this.canvasRef.nativeElement;
    // High error correction leaves enough redundancy for the center third to be covered by the
    // logo without breaking scannability.
    QRCode.toCanvas(canvas, value, {
      width: this.size(),
      margin: 1,
      errorCorrectionLevel: 'H',
      color: { dark: '#0f172a', light: '#ffffff' }
    }).then(() => this.drawLogo(canvas));
  }

  private drawLogo(canvas: HTMLCanvasElement): void {
    const ctx = canvas.getContext('2d');
    if (!ctx) return;

    const logo = new Image();
    logo.onload = () => {
      const logoSize = canvas.width * 0.22;
      const x = (canvas.width - logoSize) / 2;
      const y = (canvas.height - logoSize) / 2;
      const padding = logoSize * 0.12;

      ctx.fillStyle = '#ffffff';
      ctx.fillRect(x - padding, y - padding, logoSize + padding * 2, logoSize + padding * 2);
      ctx.drawImage(logo, x, y, logoSize, logoSize);
    };
    logo.src = LOGO_URL;
  }
}
