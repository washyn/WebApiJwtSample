import { Injectable } from '@angular/core';
import { IUIService } from './interfaces';

@Injectable()
export class ConsoleUIService implements IUIService {
  block(elm?: any): void {
    console.group('console ui service');
    console.info('block', elm);
    console.groupEnd();
  }

  unblock(elm?: any): void {
    console.group('console ui service');
    console.info('unblock', elm);
    console.groupEnd();
  }

  setBusy(elm?: any, optionsOrPromise?: any): void {
    console.group('console ui service');
    console.info('setBusy', elm, optionsOrPromise);
    console.groupEnd();
  }

  clearBusy(elm?: any): void {
    console.group('console ui service');
    console.info('clearBusy', elm);
    console.groupEnd();
  }
}
