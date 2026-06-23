import { NgModule, OnInit } from '@angular/core';
import { BooleanPipe } from './boolean.pipe';
import { DisplayTextPipe } from './display-text.pipe';
import { PrefixNamePipe } from './prefix-name.pipe';
import { PenCurrencyPipe } from './pen-currency.pipe';
import { TruncatePipe } from './truncate.pipe';
import { TruncateString } from './truncate-string.pipe';
import { TruncateStringWithPostfix } from './truncate-string-postfix.pipe';

@NgModule({
  imports: [],
  exports: [
    BooleanPipe,
    DisplayTextPipe,
    PrefixNamePipe,
    PenCurrencyPipe,
    TruncatePipe,
    TruncateString,
    TruncateStringWithPostfix,
  ],
  declarations: [
    BooleanPipe,
    DisplayTextPipe,
    PrefixNamePipe,
    PenCurrencyPipe,
    TruncatePipe,
    TruncateString,
    TruncateStringWithPostfix,
  ],
})
export class CommonPipesModule {}
