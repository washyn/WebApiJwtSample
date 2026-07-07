import { Pipe, type PipeTransform } from '@angular/core';

@Pipe({
  name: 'appTruncate',
  pure: true,
})
export class TruncatePipe implements PipeTransform {
  transform(str?: string) {
    let maxLength = 3;
    return str?.substr(0, maxLength);
  }
}
