import { Provider } from '@angular/core';
import { IMessageService, INotifyService, IUIService } from './interfaces';
import { ConsoleMessageService } from './console-message.service';
import { ConsoleNotifyService } from './console-notify.service';
import { ConsoleUIService } from './console-ui.service';

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
    {
      provide: IUIService,
      useClass: ConsoleUIService,
    },
  ];
}
