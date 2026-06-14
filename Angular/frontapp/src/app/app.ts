import { ApplicationConfigurationDto, ConfigStateService } from '@abp/ng.core';
import { JsonPipe } from '@angular/common';
import { Component, inject, OnInit, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, JsonPipe],
  templateUrl: './app.html',
  styleUrl: './app.css',
})
export class App implements OnInit {
  protected readonly title = signal('frontapp');
  appConfig: ApplicationConfigurationDto = {} as ApplicationConfigurationDto;
  ngOnInit(): void {
    this.appConfig = this.configState.getAll();
  }

  protected readonly configState = inject(ConfigStateService);


}
// TODO: add all comon components
// pipes, directives, services, etc.
// add validations and test custom message
// primero las librerias requeridas obligatorias
// - Valdiation de formularios
// - Loader, top bar, etc.
// "@ngx-validate/core": "^0.2.0",