import { Pipe, type PipeTransform } from '@angular/core';

@Pipe({
  name: 'appCurrency',
  pure: true,
})
export class PenCurrencyPipe implements PipeTransform {
  transform(value: number) {
    return new Intl.NumberFormat('es-pe', {
      style: 'currency',
      currency: 'PEN',
    }).format(value);
  }
}
