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
import { LangComponent } from './lang-component';
import { AbpUtilService } from './core/abp-utils/abp-util.service';
import { finalize } from 'rxjs';

@Component({
  selector: 'app-root',
  imports: [
    RouterOutlet,
    JsonPipe,
    ReactiveFormsModule,
    NgxValidateCoreModule,
    LoaderBarComponent,
    LangComponent,
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
  ////////////////////////////////////////
  error500() {
    this.exampleService.error500().subscribe((res) => {
      console.log('res 500');
    });
  }

  error401() {
    this.exampleService.error401().subscribe((res) => {
      console.log('res 401');
    });
  }
  error403() {
    this.exampleService.error403().subscribe((res) => {
      console.log('res 403');
    });
  }
  error40XXX() {
    this.exampleService.error40XXXByModelSample({}).subscribe((res) => {
      console.log('res 40XXX');
    });
  }
  error404() {
    this.exampleService.error404().subscribe((res) => {
      console.log('res 404');
    });
  }
  errorBusinessException() {
    this.exampleService.errorBusinessException().subscribe((res) => {
      console.log('res business exception');
    });
  }
  largeRequest() {
    this.exampleService.largeRequest().subscribe((res) => {
      console.log('res large request');
    });
  }

  largeRequestSecondExample() {
    this.util.ui.setBusy();
    this.exampleService
      .largeRequestSecondExample()
      .pipe(finalize(() => this.util.ui.clearBusy()))
      .subscribe((res) => {
        console.log('res large request');
      });
    // if requiered skip handle error, use skip handle error arg in reuest
    // and add two args in suscribe, next and error
  }

  requireAuth() {
    this.exampleService.requireAuth().subscribe((res) => {
      console.log('res require auth');
    });
  }
  error400() {
    this.exampleService.error400().subscribe((res) => {
      console.log('res 400');
    });
  }
  error501() {
    this.exampleService.error501().subscribe((res) => {
      console.log('res 501');
    });
  }

  /////////////////////////

  skipHandleError() {
    // this.exampleService
    //   .error500({
    //     skipHandleError: true,
    //   })
    //   .subscribe(
    //     (res) => {
    //       console.log('next when success');
    //     },
    //     (err) => {
    //       console.log('Custom handled error');
    //       console.log(err);
    //     }
    //   );
  }

  skipHandleErrorSuccess() {
    // this.exampleService
    //   .largeRequest({
    //     skipHandleError: true,
    //   })
    //   .subscribe(
    //     (res) => {
    //       console.log('habdkerd success');
    //     },
    //     (err) => {
    //       console.log('Custom handled error');
    //     }
    //   );
  }

  // https://github.com/abpframework/abp/issues/4560
  ////////////////////////////////////////
  save() {
    // validate form before save
    // if (!this.form.valid || this.modalBusy) return;
    if (this.formExample.invalid) return;
    console.log(this.formExample.value);
  }

  confirmExample() {
    this.util.message.confirm('Are you sure?', 'Confirm', (isConfirmed) => {
      console.log(isConfirmed);
    });
  }
}
// DONE: impkement bearer token in back... continue
// DONE: include spinner un abp utils, add angular global another library
// DONE: add interceptors for request and response... CONTINUE HERE
// DONE: Remove dep ngx-spinner, and angular animations package reference
// - add message error localization.
// TODO: improve with gpt and remove modularity.
// DONE: add example of multilanguage
// DONE: pipes,
// DONE: add all comon components, primero las librerias requeridas obligatorias
// DONE: directives.
// DONE: services, etc.
// TEST: add som einterceptor for request display and another for inject jwt token
// DONE: add common services for notifications, and confirmation.
// DONE: add abp-core, abp-utils and common.
// DONE: add abp loader by default
// DONE: Customize title page, is localization, hard code specicfic lolcalization find documentetation of abp and find key, implemetned in pagos.unaj.edu.pe
// DONE: add validations and test custom message
// DONE: Valdiation de formularios "@ngx-validate/core": "^0.2.0",
// DONE: Loader, etc.
// cutomizar logs for valirtion message for test only
// add config fronted
