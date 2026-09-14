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
    private readonly DapperBasicExamples _dapperBasic;
    private readonly DapperAdvancedExamples _dapperAdvanced;
    private readonly DapperKeyPoints _dapperKeyPoints;
    private readonly ILogger<ProjectHostedService> _logger;

    public ProjectHostedService(
        IAbpApplicationWithExternalServiceProvider abpApplication,
        NewWay newWay,
        DapperBasicExamples dapperBasic,
        DapperAdvancedExamples dapperAdvanced,
        DapperKeyPoints dapperKeyPoints,
        ILogger<ProjectHostedService> logger)
    {
        _abpApplication = abpApplication;
        _newWay = newWay;
        _dapperBasic = dapperBasic;
        _dapperAdvanced = dapperAdvanced;
        _dapperKeyPoints = dapperKeyPoints;
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

        #region Dapper Basic - Query con parámetros

        // - GetUserById (QueryFirstOrDefault con parámetro simple)
        // var userById = _dapperBasic.GetUserById("id-de-ejemplo");
        // _logger.LogInformation("GetUserById: {@User}", userById);

        // - GetUsersByEmailConfirmed (filtro booleano simple)
        // var confirmedUsers = _dapperBasic.GetUsersByEmailConfirmed(true);
        // _logger.LogInformation("GetUsersByEmailConfirmed count: {Count}", confirmedUsers.Count);

        // - GetUsersWithLockoutEnabled (múltiples parámetros)
        // var lockedUsers = _dapperBasic.GetUsersWithLockoutEnabled(true, 5);
        // _logger.LogInformation("Users lockout+failed5: {Count}", lockedUsers.Count);

        // - GetUsersByEmailDomain (LIKE pattern)
        // var domainUsers = _dapperBasic.GetUsersByEmailDomain("gmail.com");
        // _logger.LogInformation("Users gmail domain: {Count}", domainUsers.Count);

        #endregion

        #region Dapper Basic - QueryFirst vs QuerySingle

        // - QueryFirstOrDefault: NO lanza si no hay resultados
        // var userFirst = _dapperBasic.GetUserByUserName_QueryFirstOrDefault("NO_EXISTE");
        // _logger.LogInformation("QueryFirstOrDefault (no existe): {@User}", userFirst);

        // - QuerySingleOrDefault: lanza EXCEPCIÓN si hay MÁS DE 1 resultado
        // var userSingle = _dapperBasic.GetUserById_QuerySingleOrDefault("id-unico");
        // _logger.LogInformation("QuerySingleOrDefault: {@User}", userSingle);

        // - GetRoleByName con QuerySingleOrDefault (para Roles)
        // var adminRole = _dapperBasic.GetRoleByName_QuerySingleOrDefault("admin");
        // _logger.LogInformation("Role Admin: {@Role}", adminRole);

        #endregion

        #region Dapper Basic - Execute (INSERT / UPDATE / DELETE)

        // - InsertRole (INSERT simple en AspNetRoles)
        // var newRole = new AspNetRole
        // {
        //     Id = Guid.NewGuid().ToString(),
        //     Name = "Supervisor",
        //     NormalizedName = "SUPERVISOR",
        //     ConcurrencyStamp = Guid.NewGuid().ToString()
        // };
        // var rowsInserted = _dapperBasic.InsertRole(newRole);
        // _logger.LogInformation("InsertRole rows affected: {Rows}", rowsInserted);

        // - UpdateUserSecurityStamp (UPDATE de un campo)
        // var rowsStamp = _dapperBasic.UpdateUserSecurityStamp(
        //     "user-id-ejemplo", Guid.NewGuid().ToString());
        // _logger.LogInformation("UpdateSecurityStamp rows: {Rows}", rowsStamp);

        // - IncrementAccessFailedCount (UPDATE con operación matemática)
        // var rowsFailed = _dapperBasic.IncrementAccessFailedCount("user-id-ejemplo");
        // _logger.LogInformation("IncrementAccessFailed rows: {Rows}", rowsFailed);

        // - InsertUserClaim con SCOPE_IDENTITY() - retorna el nuevo ID
        // var newClaim = new AspNetUserClaim
        // {
        //     UserId = "user-id-ejemplo",
        //     ClaimType = "Pais",
        //     ClaimValue = "Peru"
        // };
        // var newClaimId = _dapperBasic.InsertUserClaim(newClaim);
        // _logger.LogInformation("InsertUserClaim new ID: {NewId}", newClaimId);

        // - AssignRoleToUser (INSERT con IF NOT EXISTS para evitar duplicados)
        // var rowsAssign = _dapperBasic.AssignRoleToUser("user-id-ejemplo", "role-id-ejemplo");
        // _logger.LogInformation("AssignRoleToUser rows: {Rows}", rowsAssign);

        // - RemoveRoleFromUser (DELETE en tabla N:N)
        // var rowsUnassign = _dapperBasic.RemoveRoleFromUser("user-id-ejemplo", "role-id-ejemplo");
        // _logger.LogInformation("RemoveRoleFromUser rows: {Rows}", rowsUnassign);

        // - InsertMultipleUserClaims (BATCH: varios claims de un solo Execute)
        // var batchClaims = new List<AspNetUserClaim>
        // {
        //     new() { UserId = "user-id", ClaimType = "Edad", ClaimValue = "25" },
        //     new() { UserId = "user-id", ClaimType = "Ciudad", ClaimValue = "Lima" }
        // };
        // var rowsBatch = _dapperBasic.InsertMultipleUserClaims(batchClaims);
        // _logger.LogInformation("InsertMultipleClaims rows: {Rows}", rowsBatch);

        // - DeleteUserClaim (DELETE simple por ID)
        // var rowsDelClaim = _dapperBasic.DeleteUserClaim(1);
        // _logger.LogInformation("DeleteUserClaim rows: {Rows}", rowsDelClaim);

        #endregion

        #region Dapper Basic - ExecuteScalar (valores agregados)

        // - GetTotalUsersCount (COUNT(*) simple)
        // var totalUsers = _dapperBasic.GetTotalUsersCount();
        // _logger.LogInformation("Total Users: {Total}", totalUsers);

        // - GetTotalRolesCount
        // var totalRoles = _dapperBasic.GetTotalRolesCount();
        // _logger.LogInformation("Total Roles: {Total}", totalRoles);

        // - GetMaxAccessFailedCount (ISNULL/MAX para valor por defecto)
        // var maxFailed = _dapperBasic.GetMaxAccessFailedCount();
        // _logger.LogInformation("Max AccessFailed: {Max}", maxFailed);

        // - GetUserClaimsSummary (subqueries + QuerySingle)
        // var userSummary = _dapperBasic.GetUserClaimsSummary("user-id-ejemplo");
        // _logger.LogInformation("User summary: {@Summary}", userSummary);

        #endregion

        #region Dapper Basic - DynamicParameters (consultas con filtros opcionales)

        // - SearchUsers_DynamicParameters (filtros opcionales, sin concatenar SQL)
        // var search = _dapperBasic.SearchUsers_DynamicParameters(
        //     userName: "admin",
        //     emailConfirmed: true,
        //     minAccessFailedCount: 0
        // );
        // _logger.LogInformation("Dynamic search count: {Count}", search.Count);

        // - InsertUserLogin_WithCheck (IF NOT EXISTS + output param para saber si insertó)
        // var login = new AspNetUserLogin
        // {
        //     LoginProvider = "Google",
        //     ProviderKey = "xyz-123-google-id",
        //     ProviderDisplayName = "Google",
        //     UserId = "user-id-ejemplo"
        // };
        // var wasInserted = _dapperBasic.InsertUserLogin_WithCheck(login);
        // _logger.LogInformation("Login insertado? {Inserted}", wasInserted == 1);

        #endregion

        #region Dapper Basic - Stored Procedures

        // NOTA: Los SP deben existir en la base de datos
        // - sp_GetUsersByEmailConfirmed
        // var spUsers = _dapperBasic.sp_GetUsersByEmailConfirmed(true);
        // _logger.LogInformation("sp_GetUsersByEmailConfirmed count: {Count}", spUsers.Count);

        // - sp_GetRolesForUser
        // var spRoles = _dapperBasic.sp_GetRolesForUser("user-id-ejemplo");
        // _logger.LogInformation("sp_GetRolesForUser: {Roles}", spRoles);

        // - sp_IncrementAccessFailed con output parameter
        // bool lockedOut;
        // var rowsSp = _dapperBasic.sp_IncrementAccessFailed("user-id-ejemplo", out lockedOut);
        // _logger.LogInformation("SP result rows: {Rows}, locked: {Locked}", rowsSp, lockedOut);

        #endregion

        #region Dapper Basic - Async / Await (métodos asíncronos)

        // - GetUsersAsync (QueryAsync)
        // var asyncUsers = await _dapperBasic.GetUsersAsync();
        // _logger.LogInformation("GetUsersAsync count: {Count}", asyncUsers.Count);

        // - GetRolesAsync
        // var asyncRoles = await _dapperBasic.GetRolesAsync();
        // _logger.LogInformation("GetRolesAsync count: {Count}", asyncRoles.Count);

        // - UpdateUserEmailConfirmedAsync (ExecuteAsync)
        // var rowsAsync = await _dapperBasic.UpdateUserEmailConfirmedAsync("user-id", true);
        // _logger.LogInformation("UpdateEmailConfirmedAsync rows: {Rows}", rowsAsync);

        // - GetTotalUsersCountAsync (ExecuteScalarAsync)
        // var totalAsync = await _dapperBasic.GetTotalUsersCountAsync();
        // _logger.LogInformation("Total Users Async: {Total}", totalAsync);

        #endregion

        #region Dapper Basic - IN Clauses

        // - GetUsersByIdList (Dapper expande la colección automáticamente)
        // var ids = new[] { "id-1", "id-2", "id-3" };
        // var usersIn = _dapperBasic.GetUsersByIdList(ids);
        // _logger.LogInformation("Users IN list: {Count}", usersIn.Count);

        // - GetClaimsByIdList
        // var claimIds = new[] { 1, 2, 3 };
        // var claimsIn = _dapperBasic.GetClaimsByIdList(claimIds);
        // _logger.LogInformation("Claims IN list: {Count}", claimsIn.Count);

        // - DeleteMultipleClaims (DELETE + IN clause)
        // var rowsInDel = _dapperBasic.DeleteMultipleClaims(claimIds);
        // _logger.LogInformation("DeleteMultipleClaims rows: {Rows}", rowsInDel);

        #endregion

        #region Dapper Basic - Tablas secundarias de Identity

        // - GetClaimsForUser
        // var userClaims = _dapperBasic.GetClaimsForUser("user-id");
        // _logger.LogInformation("User claims: {Count}", userClaims.Count);

        // - GetClaimsForRole
        // var roleClaims = _dapperBasic.GetClaimsForRole("role-id");
        // _logger.LogInformation("Role claims: {Count}", roleClaims.Count);

        // - GetLoginsForUser (login externos: Google, Facebook, etc.)
        // var logins = _dapperBasic.GetLoginsForUser("user-id");
        // _logger.LogInformation("User logins: {Count}", logins.Count);

        // - GetTokensForUser (tokens de 2FA, reset password, etc.)
        // var tokens = _dapperBasic.GetTokensForUser("user-id");
        // _logger.LogInformation("User tokens: {Count}", tokens.Count);

        // - UpsertUserToken (IF EXISTS UPDATE ELSE INSERT)
        // var token = new AspNetUserToken
        // {
        //     UserId = "user-id",
        //     LoginProvider = "Default",
        //     Name = "PasswordReset",
        //     Value = "token-value-xyz"
        // };
        // var rowsUpsert = _dapperBasic.UpsertUserToken(token);
        // _logger.LogInformation("UpsertToken rows: {Rows}", rowsUpsert);

        #endregion

        #region Dapper Advanced - Multi Mapping (JOINs)

        // - GetUsersWithRoles_SimpleJoin: INNER JOIN de Users + Roles en DTO plano
        // var usersWithRoles = _dapperAdvanced.GetUsersWithRoles_SimpleJoin();
        // _logger.LogInformation("UsersWithRoles rows: {Count}", usersWithRoles.Count);

        // - GetUsersWithRoles_SplitOn: Objetos anidados Usuario + Rol (Diccionario deduplicación)
        // var usersSplitOn = _dapperAdvanced.GetUsersWithRoles_SplitOn();
        // _logger.LogInformation("SplitOn distinct users: {Count}", usersSplitOn.Count);

        // - GetUserWithRolesAndRoleClaims_ThreeLevel: 3 entidades (User -> Role -> RoleClaim)
        // var threeLevel = _dapperAdvanced.GetUserWithRolesAndRoleClaims_ThreeLevel();
        // _logger.LogInformation("Three-level users: {Count}", threeLevel.Count);

        #endregion

        #region Dapper Advanced - QueryMultiple (una conexión, múltiples SELECT)

        // - GetUsersAndRolesCounts_OneRoundTrip: 4 SELECT en 1 viaje por red
        // var (users, roles, totalU, totalR) = _dapperAdvanced.GetUsersAndRolesCounts_OneRoundTrip();
        // _logger.LogInformation("RoundTrip: {U} users, {R} roles, Totals: {TU}/{TR}",
        //     users.Count, roles.Count, totalU, totalR);

        // - GetUserFullProfile_QueryMultiple: 5 SELECT (User + Roles + Claims + Logins + Tokens)
        // var profile = _dapperAdvanced.GetUserFullProfile_QueryMultiple("user-id");
        // _logger.LogInformation("FullProfile User: {@User}", profile.User);
        // _logger.LogInformation("  Roles: {RolesCount}, Claims: {ClaimsCount}, Logins: {LCount}, Tokens: {TCount}",
        //     profile.Roles.Count, profile.Claims.Count, profile.Logins.Count, profile.Tokens.Count);

        #endregion

        #region Dapper Advanced - Transacciones

        // - CreateRoleWithClaims_Transaction: INSERT Role + N RoleClaims (todo o nada)
        // var newTxRole = new AspNetRole
        // {
        //     Id = Guid.NewGuid().ToString(),
        //     Name = "Gestor",
        //     NormalizedName = "GESTOR",
        //     ConcurrencyStamp = Guid.NewGuid().ToString()
        // };
        // var txRoleClaims = new List<AspNetRoleClaim>
        // {
        //     new() { ClaimType = "Permission", ClaimValue = "Users.Read" },
        //     new() { ClaimType = "Permission", ClaimValue = "Users.Create" }
        // };
        // var txResult = _dapperAdvanced.CreateRoleWithClaims_Transaction(newTxRole, txRoleClaims);
        // _logger.LogInformation("CreateRoleWithClaims TX result: {Result}", txResult);

        // - ReplaceUserClaims_Transaction: DELETE All + INSERT new (reemplazo atómico)
        // var newUserClaims = new List<AspNetUserClaim>
        // {
        //     new() { ClaimType = "FullName", ClaimValue = "Juan Perez" },
        //     new() { ClaimType = "TimeZone", ClaimValue = "America/Lima" }
        // };
        // _dapperAdvanced.ReplaceUserClaims_Transaction("user-id", newUserClaims);
        // _logger.LogInformation("ReplaceUserClaims TX ejecutado OK");

        #endregion

        #region Dapper Advanced - Buffered vs NonBuffered

        // - Buffered: Todo en memoria (default)
        // var bufUsers = _dapperAdvanced.GetUsers_Buffered();
        // _logger.LogInformation("Buffered count (todo en memoria): {Count}", bufUsers.Count);

        // - NonBuffered: Streaming uno a uno (IEnumerable con DataReader abierto)
        // foreach (var u in _dapperAdvanced.GetUsers_NonBuffered())
        // {
        //     _logger.LogInformation("Streaming user (NonBuffered): {UserName}", u.UserName);
        // }

        #endregion

        #region Dapper Advanced - Dynamic (sin clase definida)

        // - GetUsersDynamic: List<dynamic> sin tipar
        // var dynUsers = _dapperAdvanced.GetUsersDynamic();
        // foreach (var du in dynUsers)
        // {
        //     _logger.LogInformation("Dynamic User: {UserName} / {Email}", du.UserName, du.Email);
        // }

        // - GetUsersWithRoleCount_Dynamic: subquery count roles
        // var dynRoles = _dapperAdvanced.GetUsersWithRoleCount_Dynamic();
        // foreach (var dr in dynRoles.Take(5))
        // {
        //     _logger.LogInformation("{UserName} - Roles: {RoleCount}", dr.UserName, dr.RoleCount);
        // }

        #endregion

        #region Dapper Key Points (12 puntos teóricos para enseñar)

        // - Obtener todos los puntos clave (título + descripción + ejemplo + por qué importa)
        // var keyPoints = _dapperKeyPoints.GetAllKeyPoints();
        // foreach (var kp in keyPoints)
        // {
        //     _logger.LogInformation("KeyPoint: {Title}", kp.Title);
        //     _logger.LogInformation("  Why: {Why}", kp.WhyImportant);
        // }

        // - Comparativa Old Way vs New Way en texto explicativo
        // var compare = _dapperKeyPoints.CompareOldVsNew("user-id");
        // _logger.LogInformation("Compare Old vs New: \n{Compare}", compare);

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
