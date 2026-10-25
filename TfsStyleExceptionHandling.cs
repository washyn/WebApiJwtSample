using System.Net;

using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.Options;

namespace AKDEMIC.API;

public static class TfsStyleExceptionHandlingExtensions
{
    public static IServiceCollection AddTfsStyleExceptionHandling(
        this IServiceCollection services,
        Action<ApiExceptionMappingOptions>? configure = null)
    {
        if (configure != null)
        {
            services.Configure(configure);
        }

        services.AddExceptionHandler<TfsStyleExceptionHandler>();
        services.AddSingleton<ApiExceptionMapping>(sp =>
        {
            var options = sp.GetRequiredService<IOptions<ApiExceptionMappingOptions>>().Value;
            var mapping = new ApiExceptionMapping();

            foreach (var kvp in DefaultExceptionMappings.BaseMappings)
            {
                mapping.AddStatusCode(kvp.Key, kvp.Value);
            }

            foreach (var kvp in options.AdditionalStatusMappings)
            {
                mapping.AddStatusCode(kvp.Key, kvp.Value);
            }

            foreach (var kvp in options.ExceptionTranslations)
            {
                mapping.AddTranslation(kvp.Key, kvp.Value);
            }

            options.ConfigureMapping?.Invoke(mapping);
            return mapping;
        });

        return services;
    }
}

public class ApiExceptionMappingOptions
{
    public IDictionary<Type, HttpStatusCode> AdditionalStatusMappings { get; } =
        new Dictionary<Type, HttpStatusCode>();

    public IDictionary<Type, Type> ExceptionTranslations { get; } =
        new Dictionary<Type, Type>();

    public Action<ApiExceptionMapping>? ConfigureMapping { get; set; }
}

public static class DefaultExceptionMappings
{
    public static readonly IReadOnlyDictionary<Type, HttpStatusCode> BaseMappings =
        new Dictionary<Type, HttpStatusCode>
        {
            { typeof(UnauthorizedAccessException), HttpStatusCode.Forbidden },
            { typeof(ArgumentException), HttpStatusCode.BadRequest },
            { typeof(ArgumentNullException), HttpStatusCode.BadRequest },
            { typeof(ArgumentOutOfRangeException), HttpStatusCode.BadRequest },
            { typeof(InvalidOperationException), HttpStatusCode.BadRequest },
            { typeof(NotImplementedException), HttpStatusCode.NotImplemented },
            { typeof(KeyNotFoundException), HttpStatusCode.NotFound },
            { typeof(OperationCanceledException), HttpStatusCode.ServiceUnavailable },
            { typeof(TimeoutException), HttpStatusCode.GatewayTimeout },
        };

    public static void AddOrUpdate(IDictionary<Type, HttpStatusCode> target)
    {
        foreach (var kvp in BaseMappings)
        {
            target[kvp.Key] = kvp.Value;
        }
    }
}

public class ApiExceptionMapping
{
    private readonly Dictionary<string, HttpStatusCode> _statusCodeMapping = new();
    private readonly Dictionary<string, Type> _translatedExceptions = new();

    public void AddStatusCode<TException>(HttpStatusCode responseStatus)
        where TException : Exception
    {
        AddStatusCode(typeof(TException), responseStatus);
    }

    public void AddStatusCode(Type exceptionType, HttpStatusCode responseStatus)
    {
        if (exceptionType == null) throw new ArgumentNullException(nameof(exceptionType));
        if (!typeof(Exception).IsAssignableFrom(exceptionType))
            throw new ArgumentException($"Type {exceptionType.FullName} must derive from Exception.", nameof(exceptionType));
        AddStatusCode(exceptionType.FullName ?? exceptionType.Name, responseStatus);
    }

    public void AddStatusCode(string exceptionTypeName, HttpStatusCode responseStatus)
    {
        if (string.IsNullOrWhiteSpace(exceptionTypeName))
            throw new ArgumentException("Exception type name cannot be empty.", nameof(exceptionTypeName));
        _statusCodeMapping[exceptionTypeName] = responseStatus;
    }

    public void AddTranslation<TSourceException, TTargetException>()
        where TSourceException : Exception
        where TTargetException : Exception
    {
        AddTranslation(typeof(TSourceException), typeof(TTargetException));
    }

    public void AddTranslation(Type sourceExceptionType, Type targetExceptionType)
    {
        if (sourceExceptionType == null) throw new ArgumentNullException(nameof(sourceExceptionType));
        if (targetExceptionType == null) throw new ArgumentNullException(nameof(targetExceptionType));
        if (!typeof(Exception).IsAssignableFrom(sourceExceptionType))
            throw new ArgumentException($"Type {sourceExceptionType.FullName} must derive from Exception.", nameof(sourceExceptionType));
        if (!typeof(Exception).IsAssignableFrom(targetExceptionType))
            throw new ArgumentException($"Type {targetExceptionType.FullName} must derive from Exception.", nameof(targetExceptionType));
        AddTranslation(sourceExceptionType.FullName ?? sourceExceptionType.Name, targetExceptionType);
    }

    public void AddTranslation(string sourceExceptionType, Type targetExceptionType)
    {
        if (string.IsNullOrWhiteSpace(sourceExceptionType))
            throw new ArgumentException("Exception type name cannot be empty.", nameof(sourceExceptionType));
        if (targetExceptionType == null) throw new ArgumentNullException(nameof(targetExceptionType));
        _translatedExceptions[sourceExceptionType] = targetExceptionType;
    }

    public HttpStatusCode? GetStatusCode(Type exceptionType)
    {
        if (exceptionType == null) return null;
        if (_statusCodeMapping.TryGetValue(exceptionType.FullName ?? exceptionType.Name, out var value))
        {
            return value;
        }
        return null;
    }

    public Exception TranslateException(Exception exception)
    {
        if (exception == null) return null!;
        if (_translatedExceptions.TryGetValue(exception.GetType().FullName ?? exception.GetType().Name, out var targetType))
        {
            try
            {
                return (Exception)Activator.CreateInstance(targetType, exception.Message, exception)!;
            }
            catch
            {
                try
                {
                    return (Exception)Activator.CreateInstance(targetType, exception.Message)!;
                }
                catch
                {
                    return exception;
                }
            }
        }
        return exception;
    }
}

public abstract class ServiceException : Exception
{
    protected ServiceException()
    {
    }

    protected ServiceException(string message)
        : base(message)
    {
    }

    protected ServiceException(string message, HttpStatusCode httpStatusCode)
        : this(message, httpStatusCode, 0)
    {
    }

    protected ServiceException(string message, HttpStatusCode httpStatusCode, int errorCode)
        : base(message)
    {
        HttpStatusCode = httpStatusCode;
        ErrorCode = errorCode;
    }

    protected ServiceException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    protected ServiceException(string message, HttpStatusCode httpStatusCode, Exception innerException)
        : this(message, httpStatusCode, 0, innerException)
    {
    }

    protected ServiceException(string message, HttpStatusCode httpStatusCode, int errorCode, Exception innerException)
        : base(message, innerException)
    {
        HttpStatusCode = httpStatusCode;
        ErrorCode = errorCode;
    }

    public HttpStatusCode HttpStatusCode { get; protected set; } = HttpStatusCode.InternalServerError;
    public int ErrorCode { get; protected set; }
}

public class TfsStyleExceptionHandler : IExceptionHandler
{
    private readonly ILogger<TfsStyleExceptionHandler> _logger;
    private readonly ApiExceptionMapping _globalMapping;
    private readonly IServiceProvider _serviceProvider;

    public TfsStyleExceptionHandler(
        ILogger<TfsStyleExceptionHandler> logger,
        ApiExceptionMapping globalMapping,
        IServiceProvider serviceProvider)
    {
        _logger = logger;
        _globalMapping = globalMapping;
        _serviceProvider = serviceProvider;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var endpoint = httpContext.GetEndpoint();
        ControllerBase? controllerInstance = null;
        ApiExceptionMapping? controllerMapping = null;

        if (endpoint?.Metadata.GetMetadata<ControllerActionDescriptor>() is { } descriptor)
        {
            var controllerType = descriptor.ControllerTypeInfo.AsType();
            if (typeof(ApiControllerBase).IsAssignableFrom(controllerType))
            {
                try
                {
                    controllerInstance = ActivatorUtilities.CreateInstance(_serviceProvider, controllerType) as ControllerBase;
                    if (controllerInstance is ApiControllerBase tfsController)
                    {
                        controllerMapping = tfsController.BuildExceptionMapping();
                    }
                }
                catch
                {
                    // si no se puede activar el controlador, usamos solo el mapping global
                }
            }
        }

        var translated = _globalMapping.TranslateException(exception);
        if (controllerMapping != null)
        {
            translated = controllerMapping.TranslateException(translated);
        }

        var statusCode = MapExceptionToStatusCode(translated, controllerMapping, controllerInstance as ApiControllerBase);

        LogException(translated, statusCode, httpContext);

        var problemDetails = new ProblemDetails
        {
            Status = (int)statusCode,
            Title = statusCode.ToString(),
            Detail = GetSafeExceptionMessage(translated, statusCode),
            Instance = httpContext.Request.Path
        };

        problemDetails.Extensions["traceId"] = httpContext.TraceIdentifier;
        problemDetails.Extensions["activityId"] = ActivityTraceId();

        if (translated is ServiceException svcEx)
        {
            problemDetails.Extensions["errorCode"] = svcEx.ErrorCode;
            problemDetails.Extensions["exceptionType"] = svcEx.GetType().Name;
        }

        if (statusCode >= HttpStatusCode.InternalServerError)
        {
            if (!string.IsNullOrWhiteSpace(translated.StackTrace) &&
                httpContext.RequestServices.GetService<IHostEnvironment>()?.IsDevelopment() == true)
            {
                problemDetails.Extensions["stackTrace"] = translated.StackTrace;
            }
        }

        httpContext.Response.StatusCode = (int)statusCode;
        httpContext.Response.ContentType = "application/problem+json";
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
        return true;
    }

    private HttpStatusCode MapExceptionToStatusCode(
        Exception exception,
        ApiExceptionMapping? controllerMapping,
        ApiControllerBase? controller)
    {
        HttpStatusCode? statusCode = null;

        if (controllerMapping != null)
        {
            statusCode = controllerMapping.GetStatusCode(exception.GetType());
        }

        if (statusCode == null)
        {
            statusCode = _globalMapping.GetStatusCode(exception.GetType());
        }

        if (statusCode == null)
        {
            if (exception is ServiceException svcEx)
            {
                statusCode = svcEx.HttpStatusCode;
            }
            else if (exception is OperationCanceledException)
            {
                statusCode = HttpStatusCode.ServiceUnavailable;
            }
            else
            {
                statusCode = HttpStatusCode.InternalServerError;
            }
        }

        if (controller != null)
        {
            try
            {
                statusCode = controller.MapException(exception, statusCode.Value);
            }
            catch
            {
                // ignore mapping failures in override
            }
        }

        return statusCode.Value;
    }

    private void LogException(Exception exception, HttpStatusCode statusCode, HttpContext httpContext)
    {
        var logLevel = statusCode >= HttpStatusCode.InternalServerError ? LogLevel.Error : LogLevel.Warning;

        if (exception is OperationCanceledException && statusCode == HttpStatusCode.ServiceUnavailable)
        {
            logLevel = LogLevel.Information;
        }

        using (_logger.BeginScope(new Dictionary<string, object>
               {
                   ["TraceId"] = httpContext.TraceIdentifier,
                   ["ActivityId"] = ActivityTraceId(),
                   ["Path"] = httpContext.Request.Path,
                   ["Method"] = httpContext.Request.Method,
                   ["StatusCode"] = (int)statusCode,
                   ["ExceptionType"] = exception.GetType().FullName ?? exception.GetType().Name
               }))
        {
            if (logLevel >= LogLevel.Error)
            {
                _logger.LogError(exception, "Unhandled exception: {Message}", exception.Message);
            }
            else if (logLevel >= LogLevel.Warning)
            {
                _logger.LogWarning(exception, "Handled exception with status {StatusCode}: {Message}",
                    (int)statusCode, exception.Message);
            }
            else
            {
                _logger.LogInformation("Request canceled or non-error exception: {Message}", exception.Message);
            }
        }
    }

    private static string GetSafeExceptionMessage(Exception ex, HttpStatusCode statusCode)
    {
        if (statusCode >= HttpStatusCode.InternalServerError)
        {
            return "An unexpected error occurred. Consult the application logs for more details.";
        }
        return ex.Message;
    }

    private static string ActivityTraceId()
    {
        return System.Diagnostics.Activity.Current?.Id ?? Guid.NewGuid().ToString("N");
    }
}

[ApiController]
[Route("api/[controller]")]
public abstract class ApiControllerBase : ControllerBase
{
    public virtual IDictionary<Type, HttpStatusCode> HttpExceptions
    {
        get
        {
            var dict = new Dictionary<Type, HttpStatusCode>();
            PopulateControllerHttpExceptions(dict);
            return dict;
        }
    }

    public virtual IDictionary<Type, Type> TranslatedExceptions
    {
        get
        {
            var dict = new Dictionary<Type, Type>();
            PopulateControllerTranslatedExceptions(dict);
            return dict;
        }
    }

    protected virtual void PopulateControllerHttpExceptions(IDictionary<Type, HttpStatusCode> mappings)
    {
    }

    protected virtual void PopulateControllerTranslatedExceptions(IDictionary<Type, Type> translations)
    {
    }

    protected virtual void InitializeExceptionMap(ApiExceptionMapping exceptionMap)
    {
    }

    public virtual HttpStatusCode MapException(Exception ex, HttpStatusCode defaultStatusCode)
    {
        if (ex == null) return defaultStatusCode;
        var mapping = BuildExceptionMapping();
        var statusCode = mapping.GetStatusCode(ex.GetType());
        if (statusCode != null) return statusCode.Value;
        if (ex is ServiceException svcEx) return svcEx.HttpStatusCode;
        return defaultStatusCode;
    }

    public virtual Exception TranslateException(Exception ex)
    {
        return BuildExceptionMapping().TranslateException(ex);
    }

    public ApiExceptionMapping BuildExceptionMapping()
    {
        var mapping = new ApiExceptionMapping();

        foreach (var kvp in DefaultExceptionMappings.BaseMappings)
        {
            mapping.AddStatusCode(kvp.Key, kvp.Value);
        }

        foreach (var kvp in HttpExceptions)
        {
            mapping.AddStatusCode(kvp.Key, kvp.Value);
        }

        foreach (var kvp in TranslatedExceptions)
        {
            mapping.AddTranslation(kvp.Key, kvp.Value);
        }

        InitializeExceptionMap(mapping);
        return mapping;
    }

    protected virtual bool ExemptFromGlobalExceptionFormatting => false;

    protected void AddDisposableResource(IDisposable resource)
    {
        if (HttpContext?.RequestServices.GetService<IScopedDisposableTracker>() is { } tracker)
        {
            tracker.Track(resource);
        }
        else
        {
            HttpContext?.Response.RegisterForDispose(resource);
        }
    }
}

public interface IScopedDisposableTracker
{
    void Track(IDisposable disposable);
}

internal class ScopedDisposableTracker : IScopedDisposableTracker, IDisposable
{
    private readonly List<IDisposable> _resources = new();

    public void Track(IDisposable disposable)
    {
        if (disposable != null) _resources.Add(disposable);
    }

    public void Dispose()
    {
        for (int i = _resources.Count - 1; i >= 0; i--)
        {
            try { _resources[i].Dispose(); } catch { }
        }
        _resources.Clear();
    }
}

/*
========================================================================
   GUÍA DE USO RÁPIDA (copiar en Program.cs / Startup.cs)
========================================================================

1) En Program.cs, antes de builder.Build():
--------------------------------------------------------
builder.Services.AddTfsStyleExceptionHandling(options =>
{
    // --- Mapeo GLOBAL estilo TFS s_httpExceptions ---
    options.AdditionalStatusMappings[typeof(MyCustomNotFoundException)] = HttpStatusCode.NotFound;
    options.AdditionalStatusMappings[typeof(MyCustomConflictException)] = HttpStatusCode.Conflict;
    options.AdditionalStatusMappings[typeof(MyThrottlingException)] = (HttpStatusCode)429;

    // --- Traducción de excepciones (transformación de tipos) ---
    options.ExceptionTranslations[typeof(System.Data.SqlClient.SqlException)] = typeof(DatabaseUnavailableException);

    // --- Hook de configuración final ---
    options.ConfigureMapping = mapping =>
    {
        mapping.AddStatusCode<InvalidTokenException>(HttpStatusCode.Unauthorized);
    };
});

// Opcional: si usas ApiControllerBase
builder.Services.AddScoped<IScopedDisposableTracker, ScopedDisposableTracker>();


2) También en Program.cs, DESPUÉS de builder.Build():
--------------------------------------------------------
var app = builder.Build();

// ⚠️ IMPORTANTE: UseExceptionHandler DEBE estar lo antes posible en el pipeline
app.UseExceptionHandler(_ => { });   // vacío: el IExceptionHandler registrado se encarga

app.UseHttpsRedirection();
// ... resto del pipeline (UseRouting, UseAuthorization, MapControllers, etc.)


3) Definir excepciones de negocio con ServiceException:
--------------------------------------------------------
public class ProductoNoEncontradoException : ServiceException
{
    public ProductoNoEncontradoException(Guid id)
        : base($"Producto {id} no encontrado", HttpStatusCode.NotFound, errorCode: 10001)
    {
    }
}

public class StockInsuficienteException : ServiceException
{
    public StockInsuficienteException(int requerido, int disponible)
        : base($"Stock insuficiente: requiere {requerido}, disponible {disponible}",
              HttpStatusCode.Conflict, errorCode: 10002)
    {
        Requerido = requerido;
        Disponible = disponible;
    }
    public int Requerido { get; }
    public int Disponible { get; }
}


4) Controlador heredando de ApiControllerBase con mapeo ESPECÍFICO:
--------------------------------------------------------
[Route("api/[controller]")]
public class ProductosController : ApiControllerBase
{
    private readonly IProductoService _service;

    public ProductosController(IProductoService service)
    {
        _service = service;
    }

    // Mapeo ESPECÍFICO de este controlador (suma al global)
    protected override void PopulateControllerHttpExceptions(IDictionary<Type, HttpStatusCode> mappings)
    {
        // este mapeo solo aplica a las acciones de ProductosController
        mappings[typeof(ProductoArchivadoException)] = HttpStatusCode.Gone; // 410
    }

    // Traducciones específicas del controlador
    protected override void PopulateControllerTranslatedExceptions(IDictionary<Type, Type> translations)
    {
        translations[typeof(InvalidOperationException)] = typeof(ProductoDomainException);
    }

    // Hook opcional para lógica compleja
    protected override void InitializeExceptionMap(ApiExceptionMapping exceptionMap)
    {
        exceptionMap.AddStatusCode<MiExcepcionRara>((HttpStatusCode)418); // I'm a teapot
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Obtener(Guid id)
    {
        // esta excepción usa su propio HttpStatusCode porque hereda de ServiceException
        var producto = await _service.Obtener(id); // → lanza ProductoNoEncontradoException
        return Ok(producto);
    }
}


5) Ejemplo de respuesta generada (PRODUCTION):
--------------------------------------------------------
HTTP 404 Not Found
Content-Type: application/problem+json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.4",
  "title": "NotFound",
  "status": 404,
  "detail": "Producto 123 no encontrado",
  "instance": "/api/Productos/123",
  "traceId": "0HXXXXXXX-XXXX",
  "activityId": "b2f8a7c9d4e5...",
  "errorCode": 10001,
  "exceptionType": "ProductoNoEncontradoException"
}


6) Ejemplo de respuesta 500 en DEVELOPMENT (incluye stack trace):
--------------------------------------------------------
{
  "status": 500,
  "title": "InternalServerError",
  "detail": "An unexpected error occurred. Consult the application logs for more details.",
  "traceId": "...",
  "activityId": "...",
  "stackTrace": "  en MiProyecto.X() ...\r\n  en ..."
}

========================================================================
*/
