import { ModuleWithProviders, NgModule } from '@angular/core';
import { HttpErrorWrapperComponent } from './components/http-error-wrapper/http-error-wrapper.component';
import { AbpSharedUtilitiesOptions, getAbpSharedUtilityProviders } from './provide-abp-shared-utilities';
import {
  BooleanPipe,
  CustomValidationErrorComponent,
  DisplayTextPipe,
  LoaderBarComponent,
  PenCurrencyPipe,
  PrefixNamePipe,
  TruncatePipe,
  TruncateString,
  TruncateStringWithPostfix,
} from './shared';

@NgModule({
  imports: [
    HttpErrorWrapperComponent,
    LoaderBarComponent,
    CustomValidationErrorComponent,
    BooleanPipe,
    DisplayTextPipe,
    PenCurrencyPipe,
    PrefixNamePipe,
    TruncatePipe,
    TruncateString,
    TruncateStringWithPostfix,
  ],
  exports: [
    HttpErrorWrapperComponent,
    LoaderBarComponent,
    CustomValidationErrorComponent,
    BooleanPipe,
    DisplayTextPipe,
    PenCurrencyPipe,
    PrefixNamePipe,
    TruncatePipe,
    TruncateString,
    TruncateStringWithPostfix,
  ],
})
export class AbpSharedUtilitiesModule {
  static forRoot(
    options: AbpSharedUtilitiesOptions = {}
  ): ModuleWithProviders<AbpSharedUtilitiesModule> {
    return {
      ngModule: AbpSharedUtilitiesModule,
      providers: getAbpSharedUtilityProviders(options),
    };
  }
}
