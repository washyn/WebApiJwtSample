import { Injectable } from '@angular/core';
import { AbpWindowService } from '@abp/ng.core';
import { IMessageService, INotifyService } from './interfaces';

@Injectable({
  providedIn: 'root',
})
export class AbpUtilService {
  constructor(
    public notify: INotifyService,
    public window: AbpWindowService,
    public message: IMessageService
  ) {}
}
