using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using Volo.Abp;

namespace Dapper.ConsoleApp;

public class ProjectHostedService : IHostedService
{
    private readonly IAbpApplicationWithExternalServiceProvider _abpApplication;
    private readonly NewWay _newWay;
    private readonly ILogger<ProjectHostedService> _logger;

    public ProjectHostedService(
        IAbpApplicationWithExternalServiceProvider abpApplication,
        NewWay newWay,
        ILogger<ProjectHostedService> logger)
    {
        _abpApplication = abpApplication;
        _newWay = newWay;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("ProjectHostedService StartAsync");

        _logger.LogInformation("========================================");
        _logger.LogInformation("  CURSO DE DAPPER - 9 PASOS");
        _logger.LogInformation("========================================");

        // PASO 1: Old Way vs New Way
        _logger.LogInformation("--- PASO 1: Old Way vs New Way ---");
        var paso1 = new Paso1_OldVsNewWay();
        var oldUsers = paso1.GetUsers_OldWay();
        var newUsers = paso1.GetUsers_NewWay();
        _logger.LogInformation("OldWay count: {Old}, NewWay count: {New}", oldUsers.Count, newUsers.Count);

        // PASO 2: Query con Parámetros
        _logger.LogInformation("--- PASO 2: Query con Parámetros ---");
        var paso2 = new Paso2_QueryConParametros();
        var confirmedUsers = paso2.GetUsersByEmailConfirmed(true);
        _logger.LogInformation("Usuarios con email confirmado: {Count}", confirmedUsers.Count);
        var rolesStartA = paso2.GetRolesByNameStart("ADMIN");
        _logger.LogInformation("Roles que empiezan por ADMIN: {Count}", rolesStartA.Count);

        // PASO 3: Tipos de Query
        _logger.LogInformation("--- PASO 3: Tipos de Query ---");
        var paso3 = new Paso3_TiposDeQuery();
        var qfUser = paso3.QueryFirstOrDefault_UsuarioPorUserName("NO_EXISTE_XXX");
        _logger.LogInformation("QueryFirstOrDefault (no existe): {@User}", qfUser);
        var qfRole = paso3.QueryFirst_RolCualquiera();
        _logger.LogInformation("QueryFirst rol: {@R}", qfRole);

        // PASO 4: Execute + ExecuteScalar
        _logger.LogInformation("--- PASO 4: Execute + ExecuteScalar ---");
        var paso4 = new Paso4_ExecuteScalar();
        var totalU = paso4.ContarUsuariosTotales();
        var totalR = paso4.ContarRolesTotales();
        var totalUConf = paso4.ContarUsuariosConEmailConfirmado();
        var primerUser = paso4.ObtenerPrimerUserName();
        _logger.LogInformation("Escalares: Users={U}, Roles={R}, Conf={Uc}, PrimerUser={Pu}",
            totalU, totalR, totalUConf, primerUser);

        // PASO 5: Async / Await
        _logger.LogInformation("--- PASO 5: Async / Await ---");
        var paso5 = new Paso5_AsyncAwait();
        var usersAsync = await paso5.GetUsuariosAsync();
        var rolesAsync = await paso5.GetRolesAsync();
        _logger.LogInformation("Async: Users={U}, Roles={R}", usersAsync.Count, rolesAsync.Count);
        var (cntU, cntR, cntC) = await paso5.ContarTodoEnParaleloAsync();
        _logger.LogInformation("WhenAll: U={U} R={R} C={C}", cntU, cntR, cntC);

        // PASO 6: DynamicParameters + IN Clauses
        _logger.LogInformation("--- PASO 6: DynamicParameters + IN Clauses ---");
        var paso6 = new Paso6_DynamicParamsInClause();
        var buscados = paso6.BuscarUsuarios(
            userName: null,
            email: null,
            emailConfirmed: true,
            lockoutEnabled: null,
            minAccessFailedCount: null
        );
        _logger.LogInformation("BuscarUsuarios (DynamicParams): {Count}", buscados.Count);
        var idsEjemplo = new List<string> { "id-1", "id-2" };
        var usersIn = paso6.ObtenerUsuariosPorIds(idsEjemplo);
        _logger.LogInformation("IN clause (aunque no existan): {Count}", usersIn.Count);

        // PASO 7: QueryMultiple
        _logger.LogInformation("--- PASO 7: QueryMultiple ---");
        var paso7 = new Paso7_QueryMultiple();
        var (users7, roles7, totU7, totR7) = paso7.ObtenerUsuariosYRolesEnUnViaje();
        _logger.LogInformation("1 viaje: U={U} R={R} Totals={TU}/{TR}",
            users7.Count, roles7.Count, totU7, totR7);
        var dash = paso7.ObtenerDashboard();
        _logger.LogInformation("Dashboard: {@Dash}", dash);

        // PASO 8: Multi-Mapping
        _logger.LogInformation("--- PASO 8: Multi-Mapping ---");
        var paso8 = new Paso8_MultiMapping();
        var flat = paso8.ObtenerUsuariosConRoles_Simple();
        _logger.LogInformation("Flat User-Role: {Rows}", flat.Count);
        var usersConRoles = paso8.ObtenerUsuariosConRoles_Diccionario();
        _logger.LogInformation("User->List<Role>: {Count}", usersConRoles.Count);

        // PASO 9: Transacciones (solo ejemplo de estructura, sin ejecutar escritura)
        _logger.LogInformation("--- PASO 9: Transacciones ---");
        var paso9 = new Paso9_Transacciones();
        _logger.LogInformation("Paso9 listo: Transacciones con BeginTransaction + Commit/Rollback");
        _logger.LogInformation("  (Ejemplos de escritura comentados para no afectar la BD)");
        // var rolTx = new AspNetRole
        // {
        //     Id = Guid.NewGuid().ToString(),
        //     Name = "Auditor",
        //     NormalizedName = "AUDITOR",
        //     ConcurrencyStamp = Guid.NewGuid().ToString()
        // };
        // var claimsTx = new List<AspNetRoleClaim>
        // {
        //     new() { ClaimType = "Permission", ClaimValue = "Audit.Read" }
        // };
        // var txOk = paso9.CrearRolConClaims(rolTx, claimsTx);

        _logger.LogInformation("========================================");
        _logger.LogInformation("  FIN DE LA PRESENTACION (35 min aprox)");
        _logger.LogInformation("========================================");

        await Task.CompletedTask;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await _abpApplication.ShutdownAsync();
    }
}
