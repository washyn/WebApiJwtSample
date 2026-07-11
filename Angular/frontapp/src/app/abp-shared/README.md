# ABP Shared Utilities

Carpeta autocontenida para copiar a otro proyecto Angular con ABP.

## Uso standalone

```ts
import { provideAbpSharedUtilities, RoleDirective } from './abp-shared';

export const appConfig = {
  providers: [provideAbpSharedUtilities()],
};

@Component({
  standalone: true,
  imports: [RoleDirective],
})
export class ExampleComponent {}
```

## Uso con modulo

```ts
import { AbpSharedUtilitiesModule } from './abp-shared';

@NgModule({
  imports: [AbpSharedUtilitiesModule.forRoot()],
})
export class AppModule {}
```

## Componentes y pipes

- `LoaderBarComponent`
- `CustomValidationErrorComponent`
- `HttpErrorWrapperComponent`
- Pipes exportados desde `shared/index.ts`
- `RoleDirective`

## Servicios

- `RoleService`

## Ejemplo de directiva

```html
<button *appRole="'Admin'">Solo admins</button>
<button *appRole="'Admin || Manager'">Admins o managers</button>
<button *appRole="'Admin && Finance'">Admins de finanzas</button>
```
