import { Pipe, type PipeTransform } from '@angular/core';

@Pipe({
  name: 'appTruncatePostfix',
  pure: true,
})
export class TruncateStringWithPostfix implements PipeTransform {
  transform(str: string, maxLength: number, postfix?: string): string {
    postfix = postfix || '...';
    if (!str || !str.length || str.length <= maxLength) {
      return str;
    }
    if (maxLength <= postfix.length) {
      return postfix.substr(0, maxLength);
    }
    return str.substr(0, maxLength - postfix.length) + postfix;
  }
}
