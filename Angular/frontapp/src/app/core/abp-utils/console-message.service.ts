import { Injectable } from '@angular/core';
import { IMessageService } from './interfaces';

@Injectable()
export class ConsoleMessageService implements IMessageService {
  info(message: string, title?: string, options?: any) {
    console.group('console message service');
    console.log(message, title);
    alert(message);
    console.groupEnd();
  }
  success(message: string, title?: string, options?: any) {
    console.group('console message service');
    console.info(message, title);
    alert(message);
    console.groupEnd();
  }
  warn(message: string, title?: string, options?: any) {
    console.group('console message service');
    console.warn(message, title);
    alert(message);
    console.groupEnd();
  }
  error(message: string, title?: string, options?: any) {
    console.group('console message service');
    console.error(message, title);
    alert(message);
    console.groupEnd();
  }
  confirm(
    message: string,
    title?: string,
    callback?: (isConfirmed: boolean, isCancelled?: boolean) => void,
    options?: any
  ) {
    console.group('console message service');
    console.info(message, title);
    let res = confirm(message);
    if (callback) {
      callback(res);
    }
    console.groupEnd();
  }
}
