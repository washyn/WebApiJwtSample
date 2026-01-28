
postgresql origen
```sql
 create table public.datito
(
    nro_dni varchar not null,
    pat_per varchar,
    mat_per varchar,
    nom_per varchar,
    fch_nac varchar,
    eda_per varchar,
    ubi_geo varchar,
    ubi_cac varchar,
    dir_per varchar,
    sex_per varchar,
    est_civ varchar,
    imp_sue varchar,
    imp_cre varchar,
    nom_mad varchar,
    nom_par varchar,
    dep     varchar,
    pro     varchar,
    dis     varchar,
    tel_per varchar,
    nom_com varchar,
    cad_uca varchar,
    nro_cui varchar,
    fch_emi varchar,
    est_atu varchar,
    inscrip varchar,
    instru  varchar,
    restric varchar,
    flg_ren boolean default false,
    ide_per serial
        primary key
);
```

sqlite destino
```sql
create table main.Peoples
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
);


```