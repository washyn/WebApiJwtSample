import { Injectable } from '@angular/core';
import { AbpWindowService } from '@abp/ng.core';
import { IMessageService, INotifyService, IUIService } from './interfaces';

@Injectable({
  providedIn: 'root',
})
export class AbpUtilService {
  constructor(
    public notify: INotifyService,
    public message: IMessageService,
    public ui: IUIService,
    public window: AbpWindowService,
  ) {}
}
