// namespace log {
//   enum levels {
//     DEBUG,
//     INFO,
//     WARN,
//     ERROR,
//     FATAL,
//   }
//   let level: levels;
//   function log(logObject?: any, logLevel?: levels): void;
//   function debug(logObject?: any): void;
//   function info(logObject?: any): void;
//   function warn(logObject?: any): void;
//   function error(logObject?: any): void;
//   function fatal(logObject?: any): void;
// }

// namespace notify {
//   function info(message: string, title?: string, options?: any): void;
//   function success(message: string, title?: string, options?: any): void;
//   function warn(message: string, title?: string, options?: any): void;
//   function error(message: string, title?: string, options?: any): void;
// }

// namespace message {
//   //TODO: these methods return jQuery.Promise instead of any. fix it.
//   function info(message: string, title?: string, options?: any): any;
//   function success(message: string, title?: string, options?: any): any;
//   function warn(message: string, title?: string, options?: any): any;
//   function error(message: string, title?: string, options?: any): any;
//   function confirm(
//     message: string,
//     title?: string,
//     callback?: (isConfirmed: boolean, isCancelled?: boolean) => void,
//     options?: any
//   ): any;
// }

// namespace ui {
//   function block(elm?: any): void;
//   function unblock(elm?: any): void;
//   function setBusy(elm?: any, optionsOrPromise?: any): void;
//   function clearBusy(elm?: any): void;
// }

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

// Sloud be reusable
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
// Sloud be reusable
export abstract class INotifyService {
  abstract info(message: string, title?: string, options?: any): void;
  abstract success(message: string, title?: string, options?: any): void;
  abstract warn(message: string, title?: string, options?: any): void;
  abstract error(message: string, title?: string, options?: any): void;
}

export abstract class IUIService {
  abstract block(elm?: any): void;
  abstract unblock(elm?: any): void;
  abstract setBusy(elm?: any, optionsOrPromise?: any): void;
  abstract clearBusy(elm?: any): void;
}
