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

        #region old way

        // var oldWayData = new OldWay();
        // var oldWayUsers = oldWayData.GetUsers();
        // _logger.LogInformation("OldWay - Users: {@Users}", oldWayUsers);

        #endregion

        #region new way - básico (comparativa directa)

        // var newWayUsers = _newWay.GetUsers();
        // _logger.LogInformation("NewWay - Users: {@Users}", newWayUsers);

        #endregion

        // ====================================================================
        // CURSO DE DAPPER: 9 PASOS DE ENSEÑANZA (similar a NewWay - instancia directa)
        // ====================================================================

        #region Paso 1: Old Way vs New Way (reducción del ~80% de código)

        // Comparativa directa entre ADO.NET manual vs Dapper
        // var paso1 = new Paso1_OldVsNewWay();

        // Old Way (SqlCommand + SqlDataReader manual, ~40 líneas):
        // var oldUsers = paso1.GetUsers_OldWay();
        // _logger.LogInformation("Paso1 OldWay users count: {Count}", oldUsers.Count);

        // New Way (Dapper Query<T>, ~3 líneas):
        // var newUsers = paso1.GetUsers_NewWay();
        // _logger.LogInformation("Paso1 NewWay users count: {Count}", newUsers.Count);

        // Por Id:
        // var userId = "un-id-de-ejemplo";
        // var u1 = paso1.GetUserById_OldWay(userId);
        // var u2 = paso1.GetUserById_NewWay(userId);
        // _logger.LogInformation("Paso1 GetUserById Old: {@Old}, New: {@New}", u1, u2);

        #endregion

        #region Paso 2: Query con Parámetros (nunca concatenar strings = SQL Injection)

        // var paso2 = new Paso2_QueryConParametros();

        // - GetUsersByEmailConfirmed (parámetro booleano)
        // var confirmed = paso2.GetUsersByEmailConfirmed(true);
        // _logger.LogInformation("Paso2 EmailConfirmed=true count: {Count}", confirmed.Count);

        // - GetUserById (parámetro simple)
        // var userById = paso2.GetUserById("algun-id");
        // _logger.LogInformation("Paso2 GetUserById: {@User}", userById);

        // - Varios filtros + LIKE pattern
        // var usersFiltrados = paso2.GetUsersConVariosFiltros(
        //     lockoutEnabled: true,
        //     minFailedAttempts: 3,
        //     emailLike: "outlook.com"
        // );
        // _logger.LogInformation("Paso2 Filtrados (lockout+failed+email) count: {Count}", usersFiltrados.Count);

        // - Ejemplo MALO (concatenación) para comparar
        // var usersUnsafe = paso2.GetUsersByEmail_Unsafe("correo@dominio.com");
        // _logger.LogInformation("Paso2 Unsafe (NO USAR EN PROD): {Count}", usersUnsafe.Count);

        #endregion

        #region Paso 3: Tipos de Query (First, Single y sus OrDefault)

        // var paso3 = new Paso3_TiposDeQuery();

        // - QueryFirstOrDefault (más usado): devuelve null si no hay resultados
        // var firstOrDefault = paso3.QueryFirstOrDefault_UsuarioPorUserName("NO_EXISTE_XXX");
        // _logger.LogInformation("Paso3 QueryFirstOrDefault (no existe): {@U}", firstOrDefault);

        // - QuerySingleOrDefault: lanza EX si hay +1 (detecta duplicados en UNIQUE)
        // var singleOrDef = paso3.QuerySingleOrDefault_RolPorNombre("Administrator");
        // _logger.LogInformation("Paso3 QuerySingleOrDefault Rol: {@R}", singleOrDef);

        // - QueryFirst: EXCEPCIÓN si NO hay resultados
        // var qFirst = paso3.QueryFirst_UsuarioConEmailConfirmado();
        // _logger.LogInformation("Paso3 QueryFirst (email confirmado): {@U}", qFirst);

        // - QuerySingle: EXCEPCIÓN si hay 0 O MÁS DE 1 resultado (PK, Unique)
        // var qSingle = paso3.QuerySingle_UsuarioPorId("id-valido");
        // _logger.LogInformation("Paso3 QuerySingle por Id: {@U}", qSingle);

        #endregion

        #region Paso 4: Execute + ExecuteScalar (INSERT/UPDATE/DELETE y agregados)

        // var paso4 = new Paso4_ExecuteScalar();

        // -----------------------
        // Execute: INSERT
        // -----------------------
        // var nuevoRol = new AspNetRole
        // {
        //     Id = Guid.NewGuid().ToString(),
        //     Name = "Coordinador",
        //     NormalizedName = "COORDINADOR",
        //     ConcurrencyStamp = Guid.NewGuid().ToString()
        // };
        // var rowsInsert = paso4.InsertRole(nuevoRol);
        // _logger.LogInformation("Paso4 InsertRole rows: {Rows}", rowsInsert);

        // Insertar Claim y OBTENER EL NUEVO ID (SCOPE_IDENTITY)
        // var newClaimId = paso4.InsertUserClaimAndGetId(
        //     userId: "user-id-ejemplo",
        //     claimType: "Area",
        //     claimValue: "Ventas"
        // );
        // _logger.LogInformation("Paso4 InsertClaim new ID: {NewId}", newClaimId);

        // Batch: varios claims de un solo Execute
        // var claimsBatch = new List<AspNetUserClaim>
        // {
        //     new() { UserId = "uid1", ClaimType = "Pais", ClaimValue = "PE" },
        //     new() { UserId = "uid1", ClaimType = "Ciudad", ClaimValue = "Lima" }
        // };
        // var rowsBatch = paso4.InsertMultipleUserClaims(claimsBatch);
        // _logger.LogInformation("Paso4 InsertMultipleClaims rows: {Rows}", rowsBatch);

        // -----------------------
        // Execute: UPDATE
        // -----------------------
        // var rowsStamp = paso4.UpdateUserSecurityStamp(
        //     "user-id", Guid.NewGuid().ToString());
        // _logger.LogInformation("Paso4 UpdateSecurityStamp rows: {Rows}", rowsStamp);

        // UPDATE con operación matemática (incremento)
        // var rowsInc = paso4.IncrementarIntentosFallidos("user-id");
        // _logger.LogInformation("Paso4 IncrementarFailed rows: {Rows}", rowsInc);

        // Upsert de token (UPDATE or INSERT)
        // var rowsUpsert = paso4.UpsertUserToken(new AspNetUserToken
        // {
        //     UserId = "uid", LoginProvider = "Default",
        //     Name = "PasswordReset", Value = "abc-123"
        // });
        // _logger.LogInformation("Paso4 UpsertToken rows: {Rows}", rowsUpsert);

        // -----------------------
        // Execute: DELETE
        // -----------------------
        // var rowsDel = paso4.DeleteUserClaim(999);
        // _logger.LogInformation("Paso4 DeleteClaim rows: {Rows}", rowsDel);

        // -----------------------
        // ExecuteScalar: agregados
        // -----------------------
        // var totalU = paso4.ContarUsuariosTotales();
        // var totalR = paso4.ContarRolesTotales();
        // var totalUConf = paso4.ContarUsuariosConEmailConfirmado();
        // var maxLock = paso4.ObtenerMaxLockoutEnd();
        // var maxFailed = paso4.ObtenerMaximoIntentosFallidos();
        // _logger.LogInformation("Paso4 Escalares: U:{U}, R:{R}, UConf:{Uc}, MaxLock:{Ml}, MaxFailed:{Mf}",
        //     totalU, totalR, totalUConf, maxLock, maxFailed);

        #endregion

        #region Paso 5: Async / Await (QueryAsync, ExecuteAsync, ExecuteScalarAsync)

        // var paso5 = new Paso5_AsyncAwait();

        // QueryAsync
        // var usersAsync = await paso5.GetUsuariosAsync();
        // _logger.LogInformation("Paso5 GetUsuariosAsync count: {Count}", usersAsync.Count);

        // QueryFirstOrDefaultAsync
        // var userAsync = await paso5.GetUsuarioPorIdAsync("id-ejemplo");
        // _logger.LogInformation("Paso5 GetUsuarioPorIdAsync: {@U}", userAsync);

        // ExecuteAsync
        // var rowsAsync = await paso5.ActualizarSecurityStampAsync("id-ejemplo");
        // _logger.LogInformation("Paso5 ActualizarSecurityStampAsync rows: {Rows}", rowsAsync);

        // ExecuteScalarAsync
        // var totalAsync = await paso5.ContarUsuariosAsync();
        // _logger.LogInformation("Paso5 ContarUsuariosAsync: {Total}", totalAsync);

        // Parallel (Task.WhenAll)
        // var (users, roles, claims) = await paso5.ContarTodoEnParaleloAsync();
        // _logger.LogInformation("Paso5 WhenAll - Users:{U}, Roles:{R}, Claims:{C}",
        //     users, roles, claims);

        #endregion

        #region Paso 6: DynamicParameters (filtros opcionales) + IN Clauses

        // var paso6 = new Paso6_DynamicParamsInClause();

        // -----------------------
        // DynamicParameters: filtros opcionales (no sabes cuáles vendrán)
        // -----------------------
        // var buscados = paso6.BuscarUsuarios(
        //     userName: "admin",
        //     emailConfirmed: true,
        //     lockoutEnabled: false
        //     // email: null, minAccessFailedCount: null => NO se agregan al query
        // );
        // _logger.LogInformation("Paso6 BuscarUsuarios (DynamicParams) count: {Count}", buscados.Count);

        // -----------------------
        // DynamicParameters + OUTPUT
        // -----------------------
        // var loginEjemplo = new AspNetUserLogin
        // {
        //     LoginProvider = "GitHub",
        //     ProviderKey = "gh_abc123xyz",
        //     ProviderDisplayName = "GitHub",
        //     UserId = "user-id"
        // };
        // var fueInsertado = paso6.InsertarLoginExterno(loginEjemplo);
        // _logger.LogInformation("Paso6 InsertarLoginExterno OUTPUT: 0 o 1 = {Result}", fueInsertado);

        // -----------------------
        // IN Clauses - Dapper expande la lista automáticamente
        // -----------------------
        // var ids = new List<string> { "id-1", "id-2", "id-3" };
        // var usersIn = paso6.ObtenerUsuariosPorIds(ids);
        // _logger.LogInformation("Paso6 IN clause Users count: {Count}", usersIn.Count);

        // IN + DELETE
        // var claimIds = new List<int> { 100, 200, 300 };
        // var rowsDelIn = paso6.EliminarVariosClaims(claimIds);
        // _logger.LogInformation("Paso6 IN clause EliminarClaims rows: {Rows}", rowsDelIn);

        // IN + UPDATE
        // var rowsConfirm = paso6.ConfirmarEmailDeUsuarios(ids);
        // _logger.LogInformation("Paso6 IN ConfirmarEmails rows: {Rows}", rowsConfirm);

        #endregion

        #region Paso 7: QueryMultiple (Varios SELECT en 1 solo viaje / 1 sola conexión)

        // var paso7 = new Paso7_QueryMultiple();

        // -----------------------
        // Users + Roles + Counts (4 SELECT, 1 viaje)
        // -----------------------
        // var (users, roles, totalU, totalR) = paso7.ObtenerUsuariosYRolesEnUnViaje();
        // _logger.LogInformation("Paso7 RoundTrip: U:{U}, R:{R}, Totals U/T:{TU}/{TR}",
        //     users.Count, roles.Count, totalU, totalR);

        // -----------------------
        // Perfil COMPLETO de 1 Usuario: User + Roles + Claims + Logins + Tokens
        // -----------------------
        // var perfil = paso7.ObtenerPerfilCompletoUsuario("user-id-ejemplo");
        // _logger.LogInformation("Paso7 Profile User: {@User}", perfil.Usuario);
        // _logger.LogInformation("  Roles:{R}, Claims:{C}, Logins:{L}, Tokens:{T}",
        //     perfil.Roles.Count, perfil.Claims.Count, perfil.Logins.Count, perfil.Tokens.Count);

        // -----------------------
        // Dashboard resumido (6 counts)
        // -----------------------
        // var dash = paso7.ObtenerDashboard();
        // _logger.LogInformation("Paso7 Dashboard: {@Dash}", dash);

        // -----------------------
        // Async: QueryMultipleAsync
        // -----------------------
        // var (usrAsync, rolesAsync) = await paso7.ObtenerUsuariosYRolesAsync();
        // _logger.LogInformation("Paso7 Async: U:{U}, R:{R}", usrAsync.Count, rolesAsync.Count);

        #endregion

        #region Paso 8: Multi-Mapping (Materializar relaciones JOIN con splitOn)

        // var paso8 = new Paso8_MultiMapping();

        // -----------------------
        // DTO plano: Usuario + Rol = 1 fila por combinación
        // -----------------------
        // var usersRolesFlat = paso8.ObtenerUsuariosConRoles_Simple();
        // _logger.LogInformation("Paso8 Flat User-Role rows: {Rows}", usersRolesFlat.Count);

        // -----------------------
        // 1 a N: Usuario con Lista de Roles (Diccionario + Distinct)
        // -----------------------
        // var usuariosConRoles = paso8.ObtenerUsuariosConRoles_Diccionario();
        // _logger.LogInformation("Paso8 User->List<Role> distinct users: {Count}",
        //     usuariosConRoles.Count);

        // -----------------------
        // 1 a N: Rol con Lista de Claims
        // -----------------------
        // var rolesConClaims = paso8.ObtenerRolesConSusClaims();
        // _logger.LogInformation("Paso8 Role->List<Claim> roles count: {Count}",
        //     rolesConClaims.Count);

        // -----------------------
        // 1 a N: Usuario con Lista de Claims
        // -----------------------
        // var usersConClaims = paso8.ObtenerUsuariosConClaims();
        // _logger.LogInformation("Paso8 User->List<Claim> users: {Count}", usersConClaims.Count);

        // -----------------------
        // 3 NIVELES: User -> Roles -> RoleClaims (splitOn: "RoleId,Id")
        // -----------------------
        // var users3Niv = paso8.ObtenerUsuarios3Niveles();
        // _logger.LogInformation("Paso8 3 niveles: {U} users (1er user roles: {R})",
        //     users3Niv.Count, users3Niv.FirstOrDefault()?.Roles.Count ?? 0);

        // -----------------------
        // 1 a 1 inline: (Usuario, Claim) por cada fila
        // -----------------------
        // var claimsConUser = paso8.ObtenerClaimsConDetalleUsuario();
        // _logger.LogInformation("Paso8 1a1 inline Claim+User rows: {Rows}", claimsConUser.Count);

        #endregion

        #region Paso 9: Transacciones (todo o nada - BeginTransaction + Commit/Rollback)

        // var paso9 = new Paso9_Transacciones();

        // -----------------------
        // Tx 1: Crear Rol + N RoleClaims (si falla algo, NO se crea NADA)
        // -----------------------
        // var rolTx = new AspNetRole
        // {
        //     Id = Guid.NewGuid().ToString(),
        //     Name = "Auditor",
        //     NormalizedName = "AUDITOR",
        //     ConcurrencyStamp = Guid.NewGuid().ToString()
        // };
        // var claimsTx = new List<AspNetRoleClaim>
        // {
        //     new() { ClaimType = "Permission", ClaimValue = "Audit.Read" },
        //     new() { ClaimType = "Permission", ClaimValue = "Audit.Report" }
        // };
        // var txOk = paso9.CrearRolConClaims(rolTx, claimsTx);
        // _logger.LogInformation("Paso9 Tx CrearRolConClaims result: {Result}", txOk);

        // -----------------------
        // Tx 2: Reemplazar Roles de Usuario (DELETE ALL + INSERT ALL)
        // -----------------------
        // var nuevosRoles = new List<string> { "role-id-1", "role-id-2" };
        // var rowsReemplazo = paso9.ReemplazarRolesDeUsuario("user-id-ejemplo", nuevosRoles);
        // _logger.LogInformation("Paso9 Tx ReemplazarRoles - nuevos: {Count} roles", rowsReemplazo);

        // -----------------------
        // Tx 3: Reemplazar Claims de Usuario
        // -----------------------
        // var nuevosClaims = new List<AspNetUserClaim>
        // {
        //     new() { ClaimType = "FullName", ClaimValue = "Juan Perez" },
        //     new() { ClaimType = "TimeZone", ClaimValue = "America/Lima" },
        //     new() { ClaimType = "Locale", ClaimValue = "es-PE" }
        // };
        // paso9.ReemplazarClaimsDeUsuario("user-id", nuevosClaims);
        // _logger.LogInformation("Paso9 Tx ReemplazarClaims OK");

        // -----------------------
        // Tx 4: Transferir Rol entre 2 usuarios (atomicidad)
        // -----------------------
        // try {
        //     paso9.TransferirRol(
        //         sourceUserId: "user-origen",
        //         targetUserId: "user-destino",
        //         roleId: "rol-a-transferir"
        //     );
        //     _logger.LogInformation("Paso9 Tx TransferirRol OK");
        // } catch (Exception ex) {
        //     _logger.LogWarning(ex, "Paso9 TransferirRol falló (se hizo ROLLBACK)");
        // }

        // -----------------------
        // Tx 5: Async (BeginTransaction + ExecuteAsync)
        // -----------------------
        // var rolesAsync = new List<string> { "admin", "manager" };
        // await paso9.ReemplazarRolesAsync("user-id", rolesAsync);
        // _logger.LogInformation("Paso9 Tx Async ReemplazarRolesAsync OK");

        #endregion

        await Task.CompletedTask;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await _abpApplication.ShutdownAsync();
    }
}
