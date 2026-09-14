using System.Collections.Generic;
using System.Data;
using System.Linq;

using Dapper;

using Microsoft.Data.SqlClient;

namespace Dapper.ConsoleApp;

// =============================================
// PASO 3: Tipos de Query (First / Single / OrDefault)
// =============================================
// Resumen:
//
//                     | 0 resultados | 1 resultado  | +1 resultados
//   ------------------+---------------+--------------+---------------
//   QueryFirst        |   ❌ EXCEPCIÓN| ✅ PRIMERO   | ✅ PRIMERO
//   QueryFirstOrDefault| ✅ null/default| ✅ PRIMERO | ✅ PRIMERO
//   QuerySingle       |   ❌ EXCEPCIÓN| ✅ ÚNICO     | ❌ EXCEPCIÓN
//   QuerySingleOrDefault| ✅ null/default| ✅ ÚNICO   | ❌ EXCEPCIÓN
//   ------------------+---------------+--------------+---------------
//
// REGLAS PRÁCTICAS:
// - Buscar por PK/Unique (Id, NormalizedUserName único) => QuerySingleOrDefault
//   Si hay +1 => detecta bug de integridad.
// - Buscar el primero de una lista posible (TOP 1) => QueryFirstOrDefault
// - Sabes con SEGURIDAD que EXISTE al menos uno => QueryFirst / QuerySingle
// =============================================

public class Paso3_TiposDeQuery
{
    private readonly string _conn = Consts.connString;

    // =====================================
    // QueryFirst
    // =====================================
    // Lanza excepción si NO HAY resultados.
    // Si hay varios, devuelve el PRIMERO (sin error).
    public AspNetUser QueryFirst_UsuarioConEmailConfirmado()
    {
        using (IDbConnection db = new SqlConnection(_conn))
        {
            // ⚠ Si NINGÚN usuario tiene el email confirmado => EXCEPCIÓN
            var sql = "SELECT TOP 1 * FROM AspNetUsers WHERE EmailConfirmed = 1 ORDER BY UserName";
            return db.QueryFirst<AspNetUser>(sql);
        }
    }

    public AspNetRole QueryFirst_RolCualquiera()
    {
        using (IDbConnection db = new SqlConnection(_conn))
        {
            return db.QueryFirst<AspNetRole>("SELECT TOP 1 * FROM AspNetRoles ORDER BY Name");
        }
    }

    // =====================================
    // QueryFirstOrDefault (el más común)
    // =====================================
    // Devuelve null/default si NO HAY resultados.
    // Si hay varios, devuelve el PRIMERO (sin error).
    public AspNetUser QueryFirstOrDefault_UsuarioPorUserName(string userName)
    {
        using (IDbConnection db = new SqlConnection(_conn))
        {
            var normalized = userName.ToUpperInvariant();
            var sql = "SELECT * FROM AspNetUsers WHERE NormalizedUserName = @NormalizedUserName";

            // ✅ Si el usuario no existe => retorna null, sin excepción
            return db.QueryFirstOrDefault<AspNetUser>(
                sql,
                new { NormalizedUserName = normalized }
            );
        }
    }

    public AspNetUserClaim QueryFirstOrDefault_CualquierClaimDeUsuario(string userId)
    {
        using (IDbConnection db = new SqlConnection(_conn))
        {
            return db.QueryFirstOrDefault<AspNetUserClaim>(
                "SELECT * FROM AspNetUserClaims WHERE UserId = @UserId ORDER BY Id",
                new { UserId = userId }
            );
        }
    }

    // =====================================
    // QuerySingle
    // =====================================
    // Lanza excepción si HAY 0 O MÁS DE 1 resultado.
    // Útil para detectar problemas de integridad.
    public AspNetUser QuerySingle_UsuarioPorId(string userId)
    {
        using (IDbConnection db = new SqlConnection(_conn))
        {
            var sql = "SELECT * FROM AspNetUsers WHERE Id = @Id";

            // ✅ Como Id es PK, debe haber EXACTAMENTE 1
            // 🚨 Si hubiera 2 => EXCEPCIÓN, lo que nos avisa de un bug!
            return db.QuerySingle<AspNetUser>(sql, new { Id = userId });
        }
    }

    public AspNetRole QuerySingle_RolPorNormalizedName(string normalizedRoleName)
    {
        using (IDbConnection db = new SqlConnection(_conn))
        {
            // NormalizedName tiene UNIQUE INDEX, así que debe ser único
            return db.QuerySingle<AspNetRole>(
                "SELECT * FROM AspNetRoles WHERE NormalizedName = @Name",
                new { Name = normalizedRoleName.ToUpperInvariant() }
            );
        }
    }

    // =====================================
    // QuerySingleOrDefault
    // =====================================
    // Lanza excepción SOLO si hay MÁS DE 1 resultado.
    // Si hay 0 => null/default.
    public AspNetUser QuerySingleOrDefault_UsuarioPorId(string userId)
    {
        using (IDbConnection db = new SqlConnection(_conn))
        {
            var sql = "SELECT * FROM AspNetUsers WHERE Id = @Id";

            // Si 0 usuarios => null
            // Si 1 usuario  => usuario
            // Si +1         => Excepción (detectamos duplicados en PK!?)
            return db.QuerySingleOrDefault<AspNetUser>(sql, new { Id = userId });
        }
    }

    // Útil para validar que un rol con nombre NORMALIZADO no esté duplicado
    public AspNetRole QuerySingleOrDefault_RolPorNombre(string roleName)
    {
        using (IDbConnection db = new SqlConnection(_conn))
        {
            return db.QuerySingleOrDefault<AspNetRole>(
                "SELECT * FROM AspNetRoles WHERE Name = @Name",
                new { Name = roleName }
            );
        }
    }

    // =====================================
    // Ejemplo práctico: el método correcto para cada caso
    // =====================================
    public List<AspNetUser> Ejemplo_ReglasDeUso()
    {
        using (IDbConnection db = new SqlConnection(_conn))
        {
            // Lista: siempre Query<T>
            return db.Query<AspNetUser>("SELECT * FROM AspNetUsers").ToList();
        }
    }
}
