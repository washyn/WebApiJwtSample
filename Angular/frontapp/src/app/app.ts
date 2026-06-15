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
@Component({
  selector: 'app-root',
  // imports: [RouterOutlet, JsonPipe, ReactiveFormsModule, NgxValidateCoreModule.forRoot()],
  imports: [RouterOutlet, JsonPipe, ReactiveFormsModule, NgxValidateCoreModule],
  templateUrl: './app.html',
  styleUrl: './app.css',
})
export class App implements OnInit {
  protected readonly title = signal('frontapp');
  appConfig: ApplicationConfigurationDto = {} as ApplicationConfigurationDto;
  formExample: FormGroup = new FormGroup({});
  protected readonly configState = inject(ConfigStateService);
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
      filter: new FormControl<string>('', [
        Validators.required,
        Validators.maxLength(10),
        // Validators.email,
        AbpValidators.emailAddress(),
      ]),
    });
  }

  save() {
    // validate form before save
    // if (!this.form.valid || this.modalBusy) return;
    if (this.formExample.invalid) return;
    console.log(this.formExample.value);
  }
}
// TODO: add all comon components
// pipes, directives, services, etc.
// add validations and test custom message
// primero las librerias requeridas obligatorias
// - Valdiation de formularios
// - Loader, top bar, etc.
// "@ngx-validate/core": "^0.2.0",

// cutomizar logs for valirtion message for test only
// add config fronted
