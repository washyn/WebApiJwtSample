using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

using Dapper;

using Volo.Abp.DependencyInjection;

namespace Dapper.ConsoleApp;

public class DapperKeyPoints : ITransientDependency
{
    private readonly IDbConnection _connection;

    public DapperKeyPoints(IDbConnection connection)
    {
        _connection = connection;
    }

    public class KeyPoint
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string CodeExample { get; set; }
        public string WhyImportant { get; set; }
    }

    public List<KeyPoint> GetAllKeyPoints()
    {
        return new List<KeyPoint>
        {
            new()
            {
                Title = "1. Prevención de SQL Injection (Siempre usa parámetros)",
                Description = "Dapper usa parámetros con @ en la consulta. NUNCA concatena strings para construir queries.",
                CodeExample =
                    "BUENO:\nvar user = connection.QueryFirst<AspNetUser>(\n    \"SELECT * FROM AspNetUsers WHERE Id = @Id\",\n    new { Id = userId });\n\nMALO:\nvar user = connection.QueryFirst<AspNetUser>(\n    \"SELECT * FROM AspNetUsers WHERE Id = '\" + userId + \"'\");",
                WhyImportant = "SQL Injection es una de las vulnerabilidades más comunes. Dapper protege automáticamente al usar parámetros."
            },
            new()
            {
                Title = "2. QueryFirst vs QuerySingle (diferencia crítica)",
                Description = "Conocer la diferencia evita excepciones y ayuda a detectar errores de datos.",
                CodeExample =
                    "QueryFirst: Devuelve el PRIMER elemento, lanza excepción si NO HAY resultados.\nQueryFirstOrDefault: Devuelve el PRIMERO o default si no hay.\n\nQuerySingle: Lanza excepción si HAY 0 o MÁS DE 1 resultado.\nQuerySingleOrDefault: Lanza excepción SOLO si hay MÁS DE 1 resultado.",
                WhyImportant = "Usa QuerySingle cuando debe existir EXACTAMENTE 1 registro (ej: buscar por Id único). Detecta duplicados."
            },
            new()
            {
                Title = "3. Dapper es Micro ORM - No es Entity Framework Core",
                Description = "Dapper solo mapea SQL a objetos. No genera SQL, no tiene migraciones ni Change Tracker.",
                CodeExample =
                    "Dapper: Escribes SQL, Dapper mapea resultados.\nEF Core: Escribes LINQ, EF genera SQL.\n\nDapper mejor cuando:\n- Rendimiento máximo\n- Control TOTAL del SQL (queries complejos)\n- Bases de datos legacy\n\nEF mejor cuando:\n- Productividad máxima\n- No quieres escribir SQL\n- Necesitas migraciones automáticas",
                WhyImportant = "La elección depende del caso. Dapper = control y velocidad. EF = productividad y convención."
            },
            new()
            {
                Title = "4. Buffered vs Non-Buffered (Rendimiento y Memoria)",
                Description = "Por defecto buffered: true (carga todo en memoria).",
                CodeExample =
                    "Buffered (default):\nvar list = connection.Query<AspNetUser>(sql, buffered: true).ToList();\n// Todo en memoria, conexión ya cerrada\n\nNon-Buffered:\nforeach (var u in connection.Query<AspNetUser>(sql, buffered: false))\n{\n    // Procesa uno a la vez\n}\n// DataReader se mantiene abierto",
                WhyImportant = "Para tablas AspNetUsers con MILES de registros, usa Non-Buffered para evitar OutOfMemoryException."
            },
            new()
            {
                Title = "5. Manejo de conexiones: Dapper abre y cierra sola (excepto transacciones)",
                Description = "Dapper gestiona la conexión si no está abierta. Las transacciones son la excepción.",
                CodeExample =
                    "Dapper gestiona la conexión:\nusing (var conn = new SqlConnection(connString))\n{\n    // Dapper Open/Close automático\n    var users = conn.Query<AspNetUser>(sql);\n}\n\nPara transacciones, abre tú la conexión:\nusing (var conn = new SqlConnection(connString))\n{\n    conn.Open();\n    using (var tran = conn.BeginTransaction())\n    {\n        conn.Execute(insertRole, role, tran);\n        conn.Execute(insertClaim, claim, tran);\n        tran.Commit();\n    }\n}",
                WhyImportant = "Evita leaks de conexiones y errores: 'ExecuteReader requiere una conexión abierta y disponible'."
            },
            new()
            {
                Title = "6. Mapeo de columnas: Convenciones y aliases",
                Description = "Dapper mapea por nombre (case-insensitive). Si difieren, usa alias o [Column].",
                CodeExample =
                    "Default (funciona automático):\nSELECT Id, UserName, Email FROM AspNetUsers\nmapea a:\npublic string Id { get; set; }\npublic string UserName { get; set; }\n\nSi nombres difieren, usa alias:\nSELECT u.Id AS UserId, r.Name AS RoleName\nFROM AspNetUsers u JOIN AspNetUserRoles ...\n\nO usa atributos:\n[Column(\"UserEmail\")] public string Email { get; set; }",
                WhyImportant = "En vistas o joins complejos, los nombres de columna rara vez coinciden 1:1 con la entidad."
            },
            new()
            {
                Title = "7. DynamicParameters: Consultas dinámicas SIN concatenar SQL",
                Description = "Filtros opcionales? Usa DynamicParameters, no armes SQL con ifs + strings.",
                CodeExample =
                    "var p = new DynamicParameters();\nvar sql = \"SELECT * FROM AspNetUsers WHERE 1=1\";\n\nif (!string.IsNullOrEmpty(userName))\n{\n    sql += \" AND NormalizedUserName LIKE @UserName\";\n    p.Add(\"UserName\", \"%\" + userName.ToUpper() + \"%\");\n}\n\nif (emailConfirmed.HasValue)\n{\n    sql += \" AND EmailConfirmed = @Confirmed\";\n    p.Add(\"Confirmed\", emailConfirmed);\n}\n\nvar result = connection.Query<AspNetUser>(sql, p);",
                WhyImportant = "Conserva la seguridad (SQL Injection) aunque los filtros sean opcionales."
            },
            new()
            {
                Title = "8. IN Clauses: Dapper expande la lista automáticamente",
                Description = "No necesitas construir manualmente \"IN (@p1, @p2, @p3)\" - Dapper lo hace.",
                CodeExample =
                    "var roleIds = new[] { \"role-1\", \"role-2\", \"role-3\" };\nvar roles = connection.Query<AspNetRole>(\n    \"SELECT * FROM AspNetRoles WHERE Id IN @RoleIds\",\n    new { RoleIds = roleIds }\n);\n// Dapper genera: WHERE Id IN (@RoleIds1, @RoleIds2, @RoleIds3)",
                WhyImportant = "Ahorra código y evita errores. Funciona con Execute también: IN clause para DELETEs múltiples."
            },
            new()
            {
                Title = "9. Multi-Mapping (splitOn): Materializa JOINs en objetos anidados",
                Description = "Para evitar el problema N+1 (1 query + N queries por cada relación), usa multi-mapping.",
                CodeExample =
                    "var sql = @\"\n    SELECT u.Id, u.UserName, r.Id, r.Name\n    FROM AspNetUsers u\n    INNER JOIN AspNetUserRoles ur ON u.Id = ur.UserId\n    INNER JOIN AspNetRoles r ON ur.RoleId = r.Id\";\n\nvar userMap = new Dictionary<string, AspNetUser>();\nconnection.Query<AspNetUser, AspNetRole, AspNetUser>(\n    sql,\n    (user, role) =>\n    {\n        if (!userMap.ContainsKey(user.Id))\n            userMap[user.Id] = user;\n        return userMap[user.Id];\n    },\n    splitOn: \"Id\"\n).Distinct().ToList();",
                WhyImportant = "Reduce drásticamente los viajes a la BD al evitar el patrón anti N+1 queries."
            },
            new()
            {
                Title = "10. QueryMultiple: Varios SELECT en UNA sola ida a la BD",
                Description = "Ejecuta múltiples SELECT y consume secuencialmente con multi.Read*.",
                CodeExample =
                    "var sql = @\"\n    SELECT * FROM AspNetUsers WHERE Id = @UserId;\n    SELECT r.* FROM AspNetRoles r\n        INNER JOIN AspNetUserRoles ur ON r.Id = ur.RoleId\n        WHERE ur.UserId = @UserId;\n    SELECT * FROM AspNetUserClaims WHERE UserId = @UserId;\";\n\nusing (var multi = connection.QueryMultiple(sql, new { UserId = id }))\n{\n    var user   = multi.ReadSingleOrDefault<AspNetUser>();\n    var roles  = multi.Read<AspNetRole>().ToList();\n    var claims = multi.Read<AspNetUserClaim>().ToList();\n}",
                WhyImportant = "En lugar de 3 viajes por red, haces 1. Impacto enorme en escenarios con latencia alta."
            },
            new()
            {
                Title = "11. Execute con colección (Batch): Múltiples inserts en una llamada",
                Description = "Pasa un IEnumerable y Dapper ejecuta el comando N veces - más eficiente que un foreach.",
                CodeExample =
                    "var claims = new[]\n{\n    new AspNetUserClaim { UserId = \"...\", ClaimType = \"Edad\", ClaimValue = \"30\" },\n    new AspNetUserClaim { UserId = \"...\", ClaimType = \"Pais\", ClaimValue = \"PE\" }\n};\n\nconnection.Execute(\n    \"INSERT INTO AspNetUserClaims (UserId, ClaimType, ClaimValue)\n     VALUES (@UserId, @ClaimType, @ClaimValue)\",\n    claims\n);",
                WhyImportant = "Mucho más eficiente que llamar Execute() dentro de un foreach."
            },
            new()
            {
                Title = "12. SCOPE_IDENTITY / OUTPUT: Recuperar ID tras INSERT (IDENTITY)",
                Description = "En tablas con IDENTITY (o NEWSEQUENTIALID()), recupera el id generado.",
                CodeExample =
                    "// Para columnas IDENTITY (INT autoincremental):\nvar sql = @\"\n    INSERT INTO AspNetUserClaims (UserId, ClaimType, ClaimValue)\n    VALUES (@UserId, @ClaimType, @ClaimValue);\n    SELECT CAST(SCOPE_IDENTITY() as int)\";\n\nvar newClaimId = connection.QuerySingle<int>(sql, claim);\n\n// Para NEWID() (string/GUID): generas el Id ANTES del INSERT\nvar role = new AspNetRole { Id = Guid.NewGuid().ToString(), ... };\nconnection.Execute(\"INSERT INTO AspNetRoles ...\", role);",
                WhyImportant = "Sin esto no puedes obtener el ID del claim recién insertado para usarlo después."
            }
        };
    }

    public string CompareOldVsNew(string userId)
    {
        var oldWay =
            "FORMA ANTIGUA (ADO.NET puro, ~40 líneas):\n" +
            "- Crear SqlConnection, Open()\n" +
            "- Crear SqlCommand con parámetros\n" +
            "- Crear SqlDataReader\n" +
            "- while(reader.Read()) { ... }\n" +
            "- Mapear CADA propiedad manualmente:\n" +
            "    u.Id   = reader.GetString(reader.GetOrdinal(\"Id\"));\n" +
            "    u.Email = reader.IsDBNull(3) ? null : reader.GetString(3);\n" +
            "    u.EmailConfirmed = reader.GetBoolean(reader.GetOrdinal(\"EmailConfirmed\"));\n" +
            "- 10+ propiedades más igual ...\n" +
            "- Close connection (o using)";

        var newWay =
            "FORMA NUEVA (Dapper, ~3 líneas):\n" +
            "- Dapper abre/cierra sola la conexión\n" +
            "- Dapper crea y mapea el objeto automáticamente\n" +
            "- Dapper maneja DBNull -> null\n" +
            "- Case-insensitive en nombres de columna\n" +
            "\n" +
            "var sql = \"SELECT * FROM AspNetUsers WHERE Id = @Id\";\n" +
            "var user = connection.QueryFirstOrDefault<AspNetUser>(\n" +
            "    sql, new { Id = userId });";

        return oldWay + "\n\n========================================\n\n" + newWay;
    }
}
