using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using EtlDapper.Lib;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace EtlDapper;

public class PeoplesDestination : IDataDestination<PeopleRecord>
{
    private readonly IConfiguration _configuration;

    public PeoplesDestination(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task InitializeAsync()
    {
        await using var sqlite = new SqliteConnection(_configuration.GetConnectionString("Destination"));
        await sqlite.OpenAsync();
        await sqlite.ExecuteAsync("PRAGMA journal_mode=WAL;");
        await sqlite.ExecuteAsync("PRAGMA synchronous=OFF;");
        var exists =
            await sqlite.ExecuteScalarAsync<long>(
                "select count(*) from sqlite_schema where type='table' and name='Peoples'");
        if (exists > 0) return;
        var ddl = @"create table main.Peoples
(
    Id                 INTEGER not null
        constraint PK_Peoples
            primary key autoincrement,
    Dni                TEXT    not null,
    ApellidoPaterno    TEXT    not null,
    ApellidoMaterno    TEXT    not null,
    Nombre             TEXT    not null,
    Nacimiento         TEXT    not null,
    Edad               TEXT    not null,
    Ubigeo             TEXT    not null,
    Ubicacion          TEXT    not null,
    Direccion          TEXT    not null,
    Sexo               TEXT    not null,
    EstadoCivil        TEXT    not null,
    Importacion        TEXT    not null,
    ImportacionCredito TEXT    not null,
    NombreMadre        TEXT    not null,
    NombrePadre        TEXT    not null,
    Departamento       TEXT    not null,
    Provincia          TEXT    not null,
    Distrito           TEXT    not null,
    Telefono           TEXT    not null,
    NombreCompleto     TEXT    not null,
    Caducidad          TEXT    not null,
    Cuil               TEXT    not null,
    FechaEmision       TEXT    not null,
    EstadoAtencion     TEXT    not null,
    Inscripcion        TEXT    not null,
    Instruccion        TEXT    not null,
    Restriccion        TEXT    not null,
    Renovacion         INTEGER not null
);";
        await sqlite.ExecuteAsync(ddl);
    }

    public async Task WriteBatchAsync(List<PeopleRecord> items)
    {
        if (items.Count == 0) return;
        await using var sqlite = new SqliteConnection(_configuration.GetConnectionString("Destination"));
        await sqlite.OpenAsync();
        using var tx = sqlite.BeginTransaction();
        var insertSql = @"insert into Peoples (
Dni, ApellidoPaterno, ApellidoMaterno, Nombre, Nacimiento, Edad, Ubigeo, Ubicacion, Direccion, Sexo, EstadoCivil,
Importacion, ImportacionCredito, NombreMadre, NombrePadre, Departamento, Provincia, Distrito, Telefono,
NombreCompleto, Caducidad, Cuil, FechaEmision, EstadoAtencion, Inscripcion, Instruccion, Restriccion, Renovacion
) values (
@Dni, @ApellidoPaterno, @ApellidoMaterno, @Nombre, @Nacimiento, @Edad, @Ubigeo, @Ubicacion, @Direccion, @Sexo, @EstadoCivil,
@Importacion, @ImportacionCredito, @NombreMadre, @NombrePadre, @Departamento, @Provincia, @Distrito, @Telefono,
@NombreCompleto, @Caducidad, @Cuil, @FechaEmision, @EstadoAtencion, @Inscripcion, @Instruccion, @Restriccion, @Renovacion
);";
        foreach (var item in items)
        {
            await sqlite.ExecuteAsync(insertSql, item, tx);
        }

        tx.Commit();
    }
}
