import { AbpValidators, ApplicationConfigurationDto, ConfigStateService } from '@abp/ng.core';
import { JsonPipe } from '@angular/common';
import { Component, inject, OnInit, signal } from '@angular/core';
import {
  FormBuilder,
  FormControl,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { RouterOutlet } from '@angular/router';
import { NgxValidateCoreModule } from '@ngx-validate/core';
import { LoaderBarComponent } from './shared';
import { ErrorSampleService } from './proxy/web-app/controllers';
import { LoadingDirective } from './loading.directive';
import { LangComponent } from './lang-component';
import { SpinerAbpComponent } from './abpspinner';
@Component({
  selector: 'app-root',
  // imports: [RouterOutlet, JsonPipe, ReactiveFormsModule, NgxValidateCoreModule.forRoot()],
  imports: [
    RouterOutlet,
    JsonPipe,
    ReactiveFormsModule,
    NgxValidateCoreModule,
    LoaderBarComponent,
    LangComponent,
    SpinerAbpComponent,
  ],
  templateUrl: './app.html',
  styleUrl: './app.css',
})
export class App implements OnInit {
  protected readonly title = signal('frontapp');
  appConfig: ApplicationConfigurationDto = {} as ApplicationConfigurationDto;
  formExample: FormGroup = new FormGroup({});
  protected readonly configState = inject(ConfigStateService);
  public exampleService = inject(ErrorSampleService);
  public formBuilder = inject(FormBuilder);

  ngOnInit(): void {
    this.appConfig = this.configState.getAll();

    this.formExample = this.formBuilder.group<{
      filter: FormControl<string | null>;
    }>({
      // NOTA: todos los validadores de abp se deben usar con la invocacacion de funciones ()
      // can be use anything is indipendent why is configured abp DEFAULT_VALIDATION_BLUEPRINTS
      // filter: new FormControl<string>('', [
      //   AbpValidators.required(),
      //   AbpValidators.stringLength({
      //     maximumLength: 3,
      //   }),
      // ]),
      // NOTE: shoud be use angular standar validations same as default theme abp
      filter: new FormControl<string>('', [
        Validators.required,
        Validators.maxLength(10),
        // Validators.email,
        AbpValidators.emailAddress(),
      ]),
    });

    this.exampleService.largeRequest().subscribe((res) => {
      console.log('res end request');
    });
  }

  callLargeRequest() {
    this.exampleService.largeRequest().subscribe((res) => {
      console.log('res end request');
    });

    // this.exampleService.largeRequest().subscribe((res) => {});
  }

  save() {
    // validate form before save
    // if (!this.form.valid || this.modalBusy) return;
    if (this.formExample.invalid) return;
    console.log(this.formExample.value);
  }
}
// DONE: add example of multilanguage
// DONE: pipes,
// TODO: add all comon components, primero las librerias requeridas obligatorias
// TODO: directives.
// TODO: services, etc.
// TODO: improve with gpt and remove modularity.
// TODO: add som einterceptor for request display and another for inject jwt token
//--
// TODO: add common services for notifications, and confirmation.
// TODO: add interceptors for request and response.
// TODO: add message error localization.
// DONE: add abp-core, abp-utils and common.

// DONE: add abp loader by default
// DONE: Customize title page, is localization, hard code specicfic lolcalization find documentetation of abp and find key, implemetned in pagos.unaj.edu.pe
// DONE: add validations and test custom message
// DONE: Valdiation de formularios "@ngx-validate/core": "^0.2.0",
// DONE: Loader, etc.

// cutomizar logs for valirtion message for test only
// add config fronted
