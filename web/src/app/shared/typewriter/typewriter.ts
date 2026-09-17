import { Component, OnDestroy, OnInit, input, signal } from '@angular/core';

const TYPE_MS_PER_CHAR = 55;
const DELETE_MS_PER_CHAR = 30;
const PAUSE_AFTER_TYPED_MS = 1800;
const PAUSE_AFTER_DELETED_MS = 300;

@Component({
  selector: 'app-typewriter',
  standalone: true,
  templateUrl: './typewriter.html',
  styleUrl: './typewriter.scss'
})
export class TypewriterComponent implements OnInit, OnDestroy {
  phrases = input.required<string[]>();
  loop = input(true);

  displayText = signal('');
  private phraseIndex = 0;
  private timer?: ReturnType<typeof setTimeout>;

  ngOnInit(): void {
    this.typePhrase();
  }

  ngOnDestroy(): void {
    clearTimeout(this.timer);
  }

  private typePhrase(): void {
    const phrase = this.phrases()[this.phraseIndex];
    let charCount = 0;

    const typeNextChar = () => {
      charCount++;
      this.displayText.set(phrase.slice(0, charCount));
      if (charCount < phrase.length) {
        this.timer = setTimeout(typeNextChar, TYPE_MS_PER_CHAR);
      } else if (this.loop()) {
        this.timer = setTimeout(() => this.deletePhrase(phrase), PAUSE_AFTER_TYPED_MS);
      }
    };

    typeNextChar();
  }

  private deletePhrase(phrase: string): void {
    let charCount = phrase.length;

    const deleteNextChar = () => {
      charCount--;
      this.displayText.set(phrase.slice(0, charCount));
      if (charCount > 0) {
        this.timer = setTimeout(deleteNextChar, DELETE_MS_PER_CHAR);
      } else {
        this.phraseIndex = (this.phraseIndex + 1) % this.phrases().length;
        this.timer = setTimeout(() => this.typePhrase(), PAUSE_AFTER_DELETED_MS);
      }
    };

    deleteNextChar();
  }
}
