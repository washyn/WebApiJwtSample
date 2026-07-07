import { Injectable } from '@angular/core';
import { INotifyService } from './interfaces';

@Injectable()
export class ConsoleNotifyService implements INotifyService {
  info(message: string, title?: string, options?: any): void {
    console.group('console notify service');
    console.info(message, title);
    console.groupEnd();
  }
  success(message: string, title?: string, options?: any): void {
    console.group('console notify service');
    console.info(message, title);
    console.groupEnd();
  }
  warn(message: string, title?: string, options?: any): void {
    console.group('console notify service');
    console.info(message, title);
    console.groupEnd();
  }
  error(message: string, title?: string, options?: any): void {
    console.group('console notify service');
    console.info(message, title);
    console.groupEnd();
  }
}
