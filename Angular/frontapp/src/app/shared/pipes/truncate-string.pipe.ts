import { Pipe, type PipeTransform } from '@angular/core';

@Pipe({
  name: 'appTruncateString',
  pure: true,
})
export class TruncateString implements PipeTransform {
  transform(str: string, maxLength: number): string {
    if (!str || !str.length || str.length <= maxLength) {
      return str;
    }
    return str.substr(0, maxLength);
  }
}
