import { Provider } from '@angular/core';
import { IMessageService, INotifyService } from './interfaces';
import { ConsoleMessageService } from './console-message.service';
import { ConsoleNotifyService } from './console-notify.service';

export function provideAbpUtils(): Provider[] {
  return [
    {
      provide: INotifyService,
      useClass: ConsoleNotifyService,
    },
    {
      provide: IMessageService,
      useClass: ConsoleMessageService,
    },
  ];
}
