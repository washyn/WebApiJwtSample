import { Component, OnInit } from '@angular/core';
import { LoadingDirective } from './loading.directive';

@Component({
  selector: 'abp-spinner',
  imports: [LoadingDirective],
  template: `
    <div class="abp-spinner" [abpLoading]="isLoading" [abpLoadingDelay]="1000">
      Lorem ipsum dolor sit amet consectetur adipisicing elit. Vero est, culpa quasi dolorum quidem
      tempora minima! Nesciunt omnis impedit et enim, expedita autem est voluptas atque nihil
      doloremque hic corporis!
    </div>
    <button (click)="ejecutarCarga()">Simular Carga</button>
  `,
})
export class SpinerAbpComponent implements OnInit {
  isLoading = false;
  ngOnInit(): void {}
  ejecutarCarga() {
    this.isLoading = true;
    setTimeout(() => (this.isLoading = false), 2000);
  }
}
