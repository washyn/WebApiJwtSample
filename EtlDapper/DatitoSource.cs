using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Npgsql;

namespace EtlDapper;

public class DatitoSource : IDataSource<PeopleRecord>
{
    private readonly IConfiguration _configuration;

    public DatitoSource(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<Batch<PeopleRecord>> FetchBatchAsync(long lastId, int batchSize)
    {
        await using var pg = new NpgsqlConnection(_configuration.GetConnectionString("Postgres"));
        await pg.OpenAsync();
        var selectSql = @"select
    nro_dni as Dni,
    pat_per as ApellidoPaterno,
    mat_per as ApellidoMaterno,
    nom_per as Nombre,
    fch_nac as Nacimiento,
    eda_per as Edad,
    ubi_geo as Ubigeo,
    ubi_cac as Ubicacion,
    dir_per as Direccion,
    sex_per as Sexo,
    est_civ as EstadoCivil,
    imp_sue as Importacion,
    imp_cre as ImportacionCredito,
    nom_mad as NombreMadre,
    nom_par as NombrePadre,
    dep as Departamento,
    pro as Provincia,
    dis as Distrito,
    tel_per as Telefono,
    nom_com as NombreCompleto,
    cad_uca as Caducidad,
    nro_cui as Cuil,
    fch_emi as FechaEmision,
    est_atu as EstadoAtencion,
    inscrip as Inscripcion,
    instru as Instruccion,
    restric as Restriccion,
    case when flg_ren then 1 else 0 end as Renovacion,
    ide_per as SourceId
from public.datito
where ide_per > @last
order by ide_per
limit @limit;";

        var rows = await pg.QueryAsync<PeopleRecord>(selectSql, new { last = lastId, limit = batchSize });
        var list = rows.AsList();
        var nextLast = list.Count == 0 ? lastId : list[^1].SourceId;
        return new Batch<PeopleRecord>(list, nextLast);
    }
}
