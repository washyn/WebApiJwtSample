# Single Sign-On (SSO) con Shared Cookies y ASP.NET Core Identity

Ejemplo funcional de SSO entre dos aplicaciones ASP.NET Core, donde una administra usuarios con Identity y la otra reutiliza la sesión sin necesidad de implementar Identity.

---

## 1. Arquitectura General

```
┌──────────────────────────────────────────────────────────────┐
│                     Browser (Usuario)                        │
│  Cookie del navegador: .SingleSignOn.SharedCookie (mismo     │
│  nombre y mismas claves de cifrado para ambos dominios)      │
└────────────┬───────────────────────────────┬─────────────────┘
             │                               │
             ▼                               ▼
┌────────────────────────┐       ┌────────────────────────────┐
│  WebAppIdentity        │       │  WebAppClient              │
│  (Servidor SSO)        │       │  (Aplicación sin Identity) │
│  ● ASP.NET Core Identity│      │  ● Solo lee la cookie      │
│  ● Admin users/roles   │       │  ● Reenvía Login al servidor│
│  ● Emite cookie SSO    │       │  ● [Authorize] funciona    │
└────────────────────────┘       └────────────────────────────┘
          HTTPS                           HTTPS
   https://localhost:7050           https://localhost:7064
```

---

## 2. ¿Cómo funciona? (Shared Cookies + Data Protection API)

El mecanismo se basa en **dos pilares indispensables**:

### 2.1 Data Protection API (misma clave de cifrado)

ASP.NET Core cifra el contenido de la cookie de autenticación. Si dos aplicaciones quieren leer la misma cookie, ambas deben usar **la misma clave maestra** y el **mismo ApplicationName**.

- Ambas apps configuran `AddDataProtection()` apuntando a **la misma carpeta física** (`SharedKeys/`)
- Ambas usan `SetApplicationName("SingleSignOnSharedApp")`
- Las claves XML se guardan en disco y las lee cada app al arrancar

> ⚠️ En Producción: sustituye `PersistKeysToFileSystem` por `PersistKeysToAzureBlobStorage`, `PersistKeysToDbContext`, `PersistKeysToRegistry` o un Key Vault.

### 2.2 Cookie de autenticación idéntica

Las dos aplicaciones configuran su **Cookie Authentication** con los mismos valores:

| Propiedad                     | Valor                               | Propósito                                        |
|-------------------------------|-------------------------------------|--------------------------------------------------|
| `Cookie.Name`                 | `.SingleSignOn.SharedCookie`        | Nombre físico de la cookie en el navegador       |
| `Cookie.SameSite`             | `Lax`                               | Permite cross-site en redirects                  |
| `AuthenticationScheme`        | `Identity.Application`              | Debe coincidir con el que Identity usa por defecto |
| `ReturnUrlParameter`          | `returnUrl`                         | Query string para volver a la app origen         |
| `ExpireTimeSpan`              | 60 minutos                          | Duración de la sesión                            |
| `SlidingExpiration`           | `true`                              | Renueva sesión con actividad                     |

> Al compartir nombre, claves y scheme, la cookie escrita por WebAppIdentity es descifrada y reconocida por WebAppClient sin hacer ninguna petición al servidor.

### 2.3 Redirección Login / Logout

- **WebAppClient NO tiene páginas de Login**. Si un usuario no autenticado accede a una ruta `[Authorize]`, el middleware redirige a `/Account/SsoLogin`.
- **SsoLogin** no muestra UI: arma la URL de WebAppIdentity `/Identity/Account/Login?returnUrl=https://localhost:7064/...` y redirige.
- Identity hace el Login, y su `returnUrl` devuelve al usuario a WebAppClient con la cookie ya emitida.
- **SsoLogout** hace `SignOutAsync("Identity.Application")` y luego redirige al Logout de Identity para cerrar sesión en ambos lados.

---

## 3. Puertos y URLs por defecto

| Aplicación      | URL HTTPS                 | URL HTTP           | Rol                     |
|-----------------|---------------------------|--------------------|-------------------------|
| WebAppIdentity  | `https://localhost:7050`  | `http://localhost:5035` | Servidor de Identidad (SSO) |
| WebAppClient    | `https://localhost:7064`  | `http://localhost:5227` | Cliente SSO             |

Para cambiar las URLs edita:
- `WebAppIdentity/Properties/launchSettings.json`
- `WebAppClient/Properties/launchSettings.json`
- `WebAppClient/appsettings.json` → sección `Sso`

---

## 4. Cómo ejecutar y probar el SSO

### 4.1 Requisitos previos

- .NET 8.0 SDK
- Navegador web (Chrome, Edge, Firefox)
- Ambas aplicaciones deben correr en HTTPS (recomendado para SameSite y cookies seguras)

### 4.2 Paso a paso

1. **Abre DOS terminales** (una por cada proyecto)

2. **Terminal 1 - WebAppIdentity (servidor SSO)**:
   ```bash
   cd WebAppIdentity
   dotnet ef database update   # Crea el SQLite MyDatabase.db
   dotnet run --launch-profile https
   ```

3. **Terminal 2 - WebAppClient (cliente SSO)**:
   ```bash
   cd WebAppClient
   dotnet run --launch-profile https
   ```

4. **Prueba de flujo SSO**:
   - Abre el navegador en **https://localhost:7064** (WebAppClient)
   - Haz clic en **Privacy** (página protegida con `[Authorize]`) → serás redirigido a `.../Account/SsoLogin`
   - `SsoLogin` te envía automáticamente a **WebAppIdentity → Login**
   - Si no tienes usuario, entra primero en **Register** (`/Identity/Account/Register`) y crea uno (no requiere confirmación de email)
   - Inicia sesión en WebAppIdentity
   - Automáticamente volverás a **WebAppClient** con sesión iniciada
   - Verifica en el navbar: aparecerá "Hello <tuusuario>!" y el botón **Logout**
   - Haz clic en **Privacy** → ahora te deja entrar y muestra tus claims
   - En otra pestaña abre **https://localhost:7050** (WebAppIdentity) → verás que tu sesión también está activa allí
   - Haz **Logout** en cualquiera de las dos apps → ambas perderán la sesión a la vez ✅

---

## 5. Resumen de los cambios clave en cada proyecto

### 5.1 WebAppIdentity (el que ya tiene Identity)

En [Program.cs](file:///d:/Documentos/code/personal_proyects/WebApiJwtSample/SingleSignOn/WebAppIdentity/Program.cs):

```csharp
// 1) Data Protection COMPARTIDO (mismo directorio y mismo nombre de app)
var sharedKeysPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "SharedKeys");
builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(sharedKeysPath))
    .SetApplicationName("SingleSignOnSharedApp");

// 2) Cookie de Identity CONFIGURADA con nombre unificado
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.Name = ".SingleSignOn.SharedCookie";   // Mismo nombre que el cliente
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.LoginPath = "/Identity/Account/Login";
    options.LogoutPath = "/Identity/Account/Logout";
    options.ReturnUrlParameter = "returnUrl";
    options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
    options.SlidingExpiration = true;
});

// 3) Añadir UseAuthentication ANTES de UseAuthorization
app.UseAuthentication();
app.UseAuthorization();
```

### 5.2 WebAppClient (el que NO tiene Identity)

En [Program.cs](file:///d:/Documentos/code/personal_proyects/WebApiJwtSample/SingleSignOn/WebAppClient/Program.cs):

```csharp
// 1) MISMA Data Protection (IDÉNTICA al IdentityServer)
var sharedKeysPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "SharedKeys");
builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(sharedKeysPath))
    .SetApplicationName("SingleSignOnSharedApp");

// 2) Autenticación por cookies con el MISMO scheme y MISMO nombre de cookie que Identity
builder.Services.AddAuthentication("Identity.Application") // <-- Este es el scheme por defecto de Identity
    .AddCookie("Identity.Application", options =>
    {
        options.Cookie.Name = ".SingleSignOn.SharedCookie";
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.LoginPath  = "/Account/SsoLogin";   // Página local que REDIRIGE al servidor
        options.LogoutPath = "/Account/SsoLogout";  // Página local que cierra sesión + redirige
        options.ReturnUrlParameter = "returnUrl";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
        options.SlidingExpiration = true;
    });

// 3) Pipeline con autenticación
app.UseAuthentication();
app.UseAuthorization();
```

Páginas añadidas en WebAppClient:

| Archivo | Propósito |
|---------|-----------|
| [SsoLogin.cshtml.cs](file:///d:/Documentos/code/personal_proyects/WebApiJwtSample/SingleSignOn/WebAppClient/Pages/Account/SsoLogin.cshtml.cs) | Arma `returnUrl` absoluta y redirige al Login de WebAppIdentity |
| [SsoLogout.cshtml.cs](file:///d:/Documentos/code/personal_proyects/WebApiJwtSample/SingleSignOn/WebAppClient/Pages/Account/SsoLogout.cshtml.cs) | Hace `SignOutAsync` local y redirige al Logout de Identity |
| [_LoginPartial.cshtml](file:///d:/Documentos/code/personal_proyects/WebApiJwtSample/SingleSignOn/WebAppClient/Pages/Shared/_LoginPartial.cshtml) | Navbar: Hello user / Login(SSO) / Logout |
| [Privacy.cshtml.cs](file:///d:/Documentos/code/personal_proyects/WebApiJwtSample/SingleSignOn/WebAppClient/Pages/Privacy.cshtml.cs) | Ejemplo de página con `[Authorize]` |

---

## 6. Consideraciones para Producción

### ⚠️ Data Protection
- **NO USES** `PersistKeysToFileSystem` en múltiples servidores a menos que el directorio sea compartido (SMB/NFS). Mejor usa:
  ```csharp
  // Azure
  builder.Services.AddDataProtection()
      .PersistKeysToAzureBlobStorage(connString, container, "keys.xml")
      .ProtectKeysWithAzureKeyVault(keyVaultUri, clientId, clientSecret)
      .SetApplicationName("SingleSignOnSharedApp");

  // O EF Core
  builder.Services.AddDataProtection()
      .PersistKeysToDbContext<MyKeysContext>()
      .SetApplicationName("SingleSignOnSharedApp");
  ```

### 🔒 HTTPS obligatorio
- Activa `options.Cookie.SecurePolicy = CookieSecurePolicy.Always;` en ambos proyectos en Producción.

### 🌐 Cookies entre subdominios (ej: app1.example.com, app2.example.com)
- Añade en AMBOS ConfigureApplicationCookie:
  ```csharp
  options.Cookie.Domain = ".example.com"; // El punto inicial es IMPORTANTE
  ```
- **No necesitas** configurar dominio explícito si ambas apps están en el mismo host (mismo dominio, distinto puerto), como este ejemplo en `localhost`.

### 🔁 Alternativa más robusta (para dominios distintos / terceros)
Si las apps no comparten el dominio de nivel superior, Shared Cookies no es suficiente. Usa OAuth 2.0 / OpenID Connect:
- **Duende IdentityServer** (comercial para uso en producción)
- **OpenIddict** (gratis, OSS)
- **Auth0 / Entra ID / Okta** (IDPs externos gestionados)

### 🗄️ Base de datos de usuarios
WebAppClient **no toca la base de datos de Identity** ni necesita conocerla. Toda la información del usuario viene en los claims de la cookie compartida.

---

## 7. Solución de problemas comunes

| Síntoma | Posible causa | Solución |
|---------|---------------|----------|
| "Login funciona en Identity pero Client no me ve autenticado" | Data Protection desincronizado (distinto directorio o ApplicationName) | Verifica que ambos `Program.cs` usen exactamente la misma ruta `SharedKeys` y el mismo string en `SetApplicationName`. Borra `SharedKeys/*.xml` y reinicia AMBAS apps. |
| "Redirect después de login va a localhost:5035 en vez de 7064" | returnUrl mal formado | Asegúrate que en SsoLogin se concatene `Sso:ClientBaseUrl` al `returnUrl` antes de pasárselo a Identity. |
| "Cookie no se envía entre puertos/dominios" | SameSite / Secure / Domain | Revisa `SameSiteMode.Lax` (necesario para redirects GET). En Producción usa `CookieSecurePolicy.Always`. |
| "Cierro sesión en Client pero sigo conectado en Identity" | Te faltó redirigir al Logout remoto | SsoLogout.cs debe hacer SignOut LOCAL + Redirect a `/Identity/Account/Logout` REMOTO. |
| "InvalidOperationException: Scheme not found" | Esquema distinto | Ambos proyectos deben usar `Identity.Application` como default authentication scheme. |
| "Solicitud de regreso no es local (returnUrl externa)" | Identity bloquea returnUrl no locales | ASP.NET Core Identity por defecto sí permite returnUrls absolutos (se valida solo el esquema). Si aparece, verifica que la URL empiece por `https://` y coincide con el servidor de identidad. |
| `dotnet ef database update` falla | Instalar EF tools | `dotnet tool install --global dotnet-ef` |
| Borrado accidental de claves SharedKeys | Todos los usuarios pierden sesión | Se regeneran automáticamente al reiniciar las apps, pero la sesión de todos se invalida (se requiere volver a loguear). Guarda una copia de seguridad en producción. |

---

## 8. Estructura final del repositorio

```
SingleSignOn/
├── SharedKeys/                    <-- Creado al primer run (claves Data Protection XML)
├── WebAppIdentity/
│   ├── Program.cs                 <-- Configuración SSO + Identity
│   ├── Areas/Identity/Pages/      <-- Razor Pages de Identity (Login/Register/Logout)
│   └── Pages/Index.cshtml         <-- Nueva landing con estado SSO
└── WebAppClient/
    ├── Program.cs                 <-- AddAuthentication cookie + Data Protection
    ├── appsettings.json           <-- Sso:IdentityBaseUrl, Sso:ClientBaseUrl
    ├── Pages/
    │   ├── Index.cshtml           <-- Landing con botón Login SSO
    │   ├── Privacy.cshtml(.cs)    <-- Página [Authorize] de prueba
    │   ├── Shared/
    │   │   ├── _Layout.cshtml     <-- Incluye <partial name="_LoginPartial"/>
    │   │   └── _LoginPartial.cshtml
    │   └── Account/
    │       ├── SsoLogin.cshtml(.cs)
    │       └── SsoLogout.cshtml(.cs)
```
add sso with open id dict