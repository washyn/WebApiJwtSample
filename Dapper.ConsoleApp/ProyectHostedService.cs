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

        // var oldWay = new OldWay().GetUsers();
        // _logger.LogInformation("OldWay count: {@Users}", oldWay);
        // var newWay = _newWay.GetUsers();
        // _logger.LogInformation("NewWay count: {@Users}", newWay);


        // PASO 1: Old Way vs New Way
        _logger.LogInformation("--- PASO 1: Old Way vs New Way ---");
        var paso1 = new Paso1_OldVsNewWay();
        var oldUsers = paso1.GetUsers_OldWay();
        var newUsers = paso1.GetUsers_NewWay();
        _logger.LogInformation("OldWay count: {Old}, NewWay count: {New}", oldUsers.Count, newUsers.Count);

        // // PASO 2: Query con Parámetros
        _logger.LogInformation("--- PASO 2: Query con Parámetros ---");
        var paso2 = new Paso2_QueryConParametros();
        var unUsuario = paso2.GetUserById(oldUsers.FirstOrDefault()?.Id ?? "");
        var usersFiltrados = paso2.GetUsersConVariosFiltros(
            lockoutEnabled: false,
            minFailedAttempts: 0,
            emailLike: "@");
        _logger.LogInformation("GetUserById: {@Nombre}, Filtrados: {Cant}", unUsuario?.UserName, usersFiltrados.Count);

        // // PASO 3: Tipos de Query
        _logger.LogInformation("--- PASO 3: Tipos de Query ---");
        var paso3 = new Paso3_TiposDeQuery();
        var qfd = paso3.QueryFirstOrDefault_UsuarioPorUserName("NO_EXISTE_XXX");
        _logger.LogInformation("QueryFirstOrDefault (no existe): {@EsNull}", qfd == null ? "NULL (OK)" : "Encontrado");
        var qs = paso3.QuerySingle_UsuarioPorId(oldUsers.First().Id);
        _logger.LogInformation("QuerySingle (existe 1): {@U}", qs.UserName);

        // // PASO 4: Execute + ExecuteScalar
        _logger.LogInformation("--- PASO 4: Execute + ExecuteScalar ---");
        var paso4 = new Paso4_ExecuteScalar();
        var totalUsuarios = paso4.ContarUsuariosTotales();
        _logger.LogInformation("ExecuteScalar - Total usuarios: {T}", totalUsuarios);

        // // var confirmados = paso4.ConfirmarEmail(oldUsers.First().Id);
        // // _logger.LogInformation("Execute - Confirmados: {N}", confirmados);

        // // PASO 5: Async / Await
        _logger.LogInformation("--- PASO 5: Async / Await ---");
        var paso5 = new Paso5_AsyncAwait();
        var usersAsync = await paso5.GetUsuariosAsync();
        _logger.LogInformation("QueryAsync - Users: {U}", usersAsync.Count);
        // var (cntU, cntR, cntC) = await paso5.ContarTodoEnParaleloAsync();
        // _logger.LogInformation("WhenAll paralelo - U={U} R={R} C={C}", cntU, cntR, cntC);

        // PASO 6: DynamicParameters + IN Clauses
        _logger.LogInformation("--- PASO 6: DynamicParameters + IN Clauses ---");
        var paso6 = new Paso6_DynamicParamsInClause();
        var busqueda = paso6.BuscarUsuarios(emailConfirmed: true, userName: null, email: null);
        _logger.LogInformation("DynamicParams (emailConfirmed=true): {Count}", busqueda.Count);
        var ids = oldUsers.Take(2).Select(u => u.Id).ToList();
        var usersIn = paso6.ObtenerUsuariosPorIds(ids);
        _logger.LogInformation("IN clause - Encontrados: {C}/{Ids}", usersIn.Count, ids.Count);

        // PASO 7: QueryMultiple
        _logger.LogInformation("--- PASO 7: QueryMultiple ---");
        var paso7 = new Paso7_QueryMultiple();
        var (users7, roles7, totU7, totR7) = paso7.ObtenerUsuariosYRolesEnUnViaje();
        _logger.LogInformation("1 viaje a BD: U={U} R={R} Totals={TU}/{TR}",
            users7.Count, roles7.Count, totU7, totR7);
        var dash = paso7.ObtenerDashboard();
        _logger.LogInformation("Dashboard (6 queries en 1 viaje): {@Dash}", dash);

        // PASO 8: Multi-Mapping
        _logger.LogInformation("--- PASO 8: Multi-Mapping ---");
        var paso8 = new Paso8_MultiMapping();
        var flat = paso8.ObtenerUsuariosConRoles_Simple();
        _logger.LogInformation("Flat (User+Role): {Filas}", flat.Count);
        var usuariosConRoles = paso8.ObtenerUsuariosConRoles_Diccionario();
        _logger.LogInformation("Padre->Hijos (User->List<Role>): {Users}", usuariosConRoles.Count);

        // // PASO 9: Transacciones
        _logger.LogInformation("--- PASO 9: Transacciones ---");
        var paso9 = new Paso9_Transacciones();
        _logger.LogInformation("Métodos listos: CrearRolConClaims + ReemplazarRolesDeUsuario");
        _logger.LogInformation("(Ejemplos de escritura comentados para no afectar la BD)");
        // var rolNuevo = new AspNetRole
        // {
        //     Id = Guid.NewGuid().ToString(),
        //     Name = "Demo-Rol",
        //     NormalizedName = "DEMO-ROL",
        //     ConcurrencyStamp = Guid.NewGuid().ToString()
        // };
        // var claims = new List<AspNetRoleClaim>
        // {
        //     new() { ClaimType = "Permission", ClaimValue = "Demo.Read" },
        //     new() { ClaimType = "Permission", ClaimValue = "Demo.Write" }
        // };
        // var ok = paso9.CrearRolConClaims(rolNuevo, claims);
        // _logger.LogInformation("Rol creado con claims: {R}", ok);


        await Task.CompletedTask;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await _abpApplication.ShutdownAsync();
    }
}
