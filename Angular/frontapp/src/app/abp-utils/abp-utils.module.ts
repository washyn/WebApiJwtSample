import { NgModule } from '@angular/core';
import { IMessageService, INotifyService } from './interfaces';
import { ConsoleMessageService } from './console-message.service';
import { ConsoleNotifyService } from './console-notify.service';

@NgModule({
  declarations: [],
  imports: [],
  exports: [],
  providers: [
    {
      provide: INotifyService,
      useClass: ConsoleNotifyService,
    },
    {
      provide: IMessageService,
      useClass: ConsoleMessageService,
    },
  ],
})
export class AbpUtilsModule {}
