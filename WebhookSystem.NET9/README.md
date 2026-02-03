# WebhookSystem.NET9

Este proyecto es una implementación robusta de un sistema de gestión y envío de Webhooks, construido sobre **.NET 9**. Permite a los clientes suscribirse a eventos, recibir notificaciones HTTP seguras y gestionar el ciclo de vida de las entregas con políticas de reintento.

## 📋 Características Principales

- **Gestión de Suscripciones**: API REST completa para crear, leer, actualizar y eliminar suscripciones a webhooks.
- **Seguridad HMAC**: Firma de payloads utilizando HMAC-SHA256 para verificar la integridad y autenticidad de los mensajes (`X-Webhook-Signature`).
- **Resiliencia y Reintentos**: Implementación de políticas de reintento con *Exponential Backoff* para entregas fallidas.
- **Historial de Entregas**: Registro detallado de cada intento de envío, incluyendo códigos de estado, tiempos de respuesta y cuerpos de respuesta.
- **Middleware de Autenticación**: Middleware dedicado para validar firmas HMAC en endpoints receptores.
- **Background Jobs**: Integración con **Hangfire** para procesamiento en segundo plano (configurado en la infraestructura).
- **Health Checks**: Monitoreo de salud del sistema y base de datos.

## 🏗 Arquitectura

El proyecto sigue una arquitectura limpia y modular basada en **Minimal APIs** de .NET 9.

### Tecnologías Clave

- **Framework**: .NET 9
- **Base de Datos**: SQL Server (Entity Framework Core 9)
- **HTTP Client**: `Microsoft.Extensions.Http.Resilience` para clientes HTTP robustos.
- **Logging**: Serilog.
- **Background Processing**: Hangfire.
- **Documentación**: Swagger / OpenAPI.

### Estructura del Proyecto

- **`Endpoints/`**: Definición de las rutas de la API (Minimal APIs).
  - `WebhookEndpoints.cs`: Gestión de suscripciones y disparo de eventos.
  - `WebhookReceiverEndpoints.cs`: Endpoints de ejemplo para recibir y validar webhooks.
- **`Services/`**: Lógica de negocio.
  - `WebhookSender.cs`: Encargado de construir y enviar las peticiones HTTP, manejando firmas y reintentos.
  - `WebhookService.cs`: Orquestador de lógica de suscripciones y eventos.
  - `HmacAuthenticationService.cs`: Generación y validación de firmas criptográficas.
- **`Models/`**: Entidades del dominio (`WebhookSubscription`, `WebhookEvent`, `WebhookDelivery`).
- **`Data/`**: Contexto de base de datos (`WebhookDbContext`).
- **`Middleware/`**: Componentes del pipeline HTTP (`WebhookAuthenticationMiddleware`).

### Flujo de Datos

1.  **Suscripción**: Un cliente registra una URL (endpoint) y los eventos que desea escuchar. El sistema genera un `Secret` único para esa suscripción.
2.  **Disparo de Evento (Trigger)**: Se recibe un evento (ej. `order.created`) a través de la API.
3.  **Procesamiento**: El sistema busca las suscripciones activas para ese evento.
4.  **Envío (Dispatch)**:
    - Se genera el payload JSON.
    - Se firma el payload con el `Secret` de la suscripción (HMAC-SHA256).
    - Se envía la petición HTTP POST al cliente.
    - Si falla, se programa un reintento basado en la configuración (`MaxRetries`, `RetryDelay`).

## 🚀 Cómo Ejecutar

### Prerrequisitos

- .NET 9 SDK
- SQL Server (LocalDB o instancia completa)

### Pasos

1.  **Configuración**:
    Asegúrate de que la cadena de conexión en `appsettings.json` apunte a tu instancia de SQL Server:
    ```json
    "ConnectionStrings": {
      "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=WebhookSystem;Trusted_Connection=True;MultipleActiveResultSets=true"
    }
    ```

2.  **Ejecutar la aplicación**:
    ```bash
    dotnet run
    ```
    El sistema aplicará automáticamente las migraciones de base de datos al iniciar en entorno de desarrollo.

3.  **Explorar la API**:
    Abre tu navegador en `https://localhost:7084/swagger` (o el puerto configurado) para ver la documentación interactiva.

## 🛡 Seguridad

El sistema utiliza un esquema de firma estándar para garantizar que los webhooks provienen de una fuente confiable.

Los headers incluidos en cada petición son:
- `X-Webhook-Signature`: `sha256=<signature>`
- `X-Webhook-Timestamp`: Timestamp del envío.
- `X-Webhook-Id`: Identificador único del envío.
