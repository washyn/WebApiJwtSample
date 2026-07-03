import {
  ConfigStateService,
  LanguageInfo,
  LocalizationModule,
  LocalizationPipe,
  SessionStateService,
} from '@abp/ng.core';
import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { map, Observable } from 'rxjs';

@Component({
  selector: 'lang-component',
  imports: [CommonModule, LocalizationPipe],
  template: `
    <div>
      <div>
        {{ 'HelpDesk::Welcome' | abpLocalization }}
      </div>
      <div>for show language change reload page</div>
      <div>Default lang : {{ defaultLanguage$ | async }}</div>
      <div>
        langs

        <button
          *ngFor="let language of dropdownLanguages$ | async"
          (click)="onChangeLang(language.cultureName || '')"
        >
          {{ language.displayName }}
        </button>
      </div>
    </div>
  `,
})
export class LangComponent {
  languages$: Observable<LanguageInfo[]> = this.configState.getDeep$('localization.languages');

  get defaultLanguage$(): Observable<string> {
    return this.languages$.pipe(
      map(
        (languages) =>
          languages?.find((lang) => lang.cultureName === this.selectedLangCulture)?.displayName ||
          ''
      )
    );
  }

  get dropdownLanguages$(): Observable<LanguageInfo[]> {
    return this.languages$.pipe(
      map(
        (languages) =>
          languages?.filter((lang) => lang.cultureName !== this.selectedLangCulture) || []
      )
    );
  }

  get selectedLangCulture(): string {
    return this.sessionState.getLanguage();
  }

  constructor(private sessionState: SessionStateService, private configState: ConfigStateService) { }

  onChangeLang(cultureName: string) {
    this.sessionState.setLanguage(cultureName);
    // NOTE:aded only for test
    window.location.reload();
  }
}
