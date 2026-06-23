import { Pipe, type PipeTransform } from '@angular/core';

@Pipe({
  name: 'appBoolean',
  pure: true,
})
export class BooleanPipe implements PipeTransform {
  transform(value?: boolean) {
    switch (value) {
      case true:
        return 'Si';
      case false:
        return 'No';
      default:
        return '-';
    }
  }
}
