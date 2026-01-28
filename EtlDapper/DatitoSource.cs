using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using EtlDapper.Lib;
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

    public async Task<DataPage<PeopleRecord>> FetchPageAsync(int pageIndex, int pageSize)
    {
        await using var pg = new NpgsqlConnection(_configuration.GetConnectionString("Postgres"));
        await pg.OpenAsync();

        var offset = (pageIndex - 1) * pageSize;

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
order by ide_per
limit @limit offset @offset;";

        var rows = await pg.QueryAsync<PeopleRecord>(selectSql, new { limit = pageSize, offset = offset });
        var list = rows.AsList();
        return new DataPage<PeopleRecord>(list, pageIndex);
    }
}
