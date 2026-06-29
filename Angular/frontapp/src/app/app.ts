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
import { AbpUtilService } from './core/abp-utils/abp-util.service';
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
  public util = inject(AbpUtilService);

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
  }

  callLargeRequest() {
    this.exampleService.largeRequest().subscribe((res) => {
      console.log('res end request');
    });
    // NOTA: cuando hay 2 suscriptores aun mismo request no se llega a mostrar el loader
    // this.exampleService.largeRequest().subscribe((res) => {});
  }

  exampleCallError401() {
    // before
    this.exampleService
      .error401({
        skipHandleError: false, // not hadle error in interceptor
      })
      .subscribe(
        (res) => {
          console.log('res 401');
        },
        (err) => {
          console.log('error 401');
        }
      );
  }

  save() {
    // validate form before save
    // if (!this.form.valid || this.modalBusy) return;
    if (this.formExample.invalid) return;
    console.log(this.formExample.value);
  }

  confirmExample() {
    this.util.message.confirm('Are you sure?', 'Confirm', (isConfirmed) => {
      console.log('isConfirmed');
      console.log(isConfirmed);
    });
  }
}
// DONE: add example of multilanguage
// DONE: pipes,
// DONE: add all comon components, primero las librerias requeridas obligatorias
// DONE: directives.
// DONE: services, etc.
// TODO: improve with gpt and remove modularity.
// TODO: add som einterceptor for request display and another for inject jwt token
//--
// DONE: add common services for notifications, and confirmation.
// TODO: add interceptors for request and response.
// TODO: add message error localization.
// TODO: include spinner un abp utils
// DONE: add abp-core, abp-utils and common.

// DONE: add abp loader by default
// DONE: Customize title page, is localization, hard code specicfic lolcalization find documentetation of abp and find key, implemetned in pagos.unaj.edu.pe
// DONE: add validations and test custom message
// DONE: Valdiation de formularios "@ngx-validate/core": "^0.2.0",
// DONE: Loader, etc.

// cutomizar logs for valirtion message for test only
// add config fronted
