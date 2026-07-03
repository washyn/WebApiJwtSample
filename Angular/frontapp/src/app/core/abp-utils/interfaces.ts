export enum levels {
  DEBUG,
  INFO,
  WARN,
  ERROR,
  FATAL,
}

export abstract class ILogService {
  abstract log(logObject?: any, logLevel?: levels): void;
  abstract debug(logObject?: any): void;
  abstract info(logObject?: any): void;
  abstract warn(logObject?: any): void;
  abstract error(logObject?: any): void;
  abstract fatal(logObject?: any): void;
}

export abstract class IMessageService {
  abstract info(message: string, title?: string, options?: any): any;
  abstract success(message: string, title?: string, options?: any): any;
  abstract warn(message: string, title?: string, options?: any): any;
  abstract error(message: string, title?: string, options?: any): any;
  abstract confirm(
    message: string,
    title?: string,
    callback?: (isConfirmed: boolean, isCancelled?: boolean) => void,
    options?: any
  ): any;
}

export abstract class INotifyService {
  abstract info(message: string, title?: string, options?: any): void;
  abstract success(message: string, title?: string, options?: any): void;
  abstract warn(message: string, title?: string, options?: any): void;
  abstract error(message: string, title?: string, options?: any): void;
}
// TODO: add extra service fake and remove ngx-spinner
export abstract class IUIService {
  abstract block(elm?: any): void;
  abstract unblock(elm?: any): void;
  abstract setBusy(elm?: any, optionsOrPromise?: any): void;
  abstract clearBusy(elm?: any): void;
}
