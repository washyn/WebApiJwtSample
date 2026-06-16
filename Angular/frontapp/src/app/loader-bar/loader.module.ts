import { NgModule } from '@angular/core';
import { LoaderBarComponent } from './loader-bar.component';
import { CommonModule } from '@angular/common';
import { HTTP_INTERCEPTORS } from '@angular/common/http';
import { OAuthApiInterceptor } from '../api.interceptor';

@NgModule({
  declarations: [LoaderBarComponent],
  imports: [CommonModule],
  exports: [LoaderBarComponent, CommonModule],
})
export class LoaderModule {}
