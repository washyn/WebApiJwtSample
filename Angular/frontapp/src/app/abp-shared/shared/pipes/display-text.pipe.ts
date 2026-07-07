import { Pipe, type PipeTransform } from '@angular/core';

@Pipe({
  name: 'displayText',
  pure: true,
})
export class DisplayTextPipe implements PipeTransform {
  transform(value: string, maxLength?: number) {
    maxLength ??= 20;
    return this.cortarTexto(value, maxLength);
  }

  cortarTexto(elemento: string, longitudMaxima: number) {
    if (elemento.length > longitudMaxima) {
      elemento = elemento.substring(0, longitudMaxima - 3) + '...';
      return elemento;
    }
    return elemento;
  }
}
