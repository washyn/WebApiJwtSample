// abp.utils.truncateStringWithPostfix = function (str, maxLength, postfix) {
//   postfix = postfix || '...';

//   if (!str || !str.length || str.length <= maxLength) {
//       return str;
//   }

//   if (maxLength <= postfix.length) {
//       return postfix.substr(0, maxLength);
//   }

//   return str.substr(0, maxLength - postfix.length) + postfix;
// };
import { Pipe, type PipeTransform } from '@angular/core';

@Pipe({
  name: 'appTruncate',
  pure: true,
})
export class TruncatePipe implements PipeTransform {
  transform(str?: string) {
    let maxLength = 3;
    // let postfix = '';
    // abp.utils.truncateStringWithPostfix = function (str, maxLength, postfix = '') {
    // };
    // postfix = postfix || '';

    // if (!str || !str.length || str.length >= maxLength) {
    //   return str;
    // }

    return str?.substr(0, maxLength);
  }
}
