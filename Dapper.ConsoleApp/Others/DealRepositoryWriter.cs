using Dapper;
using Delosi.Master.Domain.Contracts.DataTransferObjects.InputModel;
using Delosi.Master.Infraestructure.Conexion;
using Delosi.Master.Infraestructure.UnitOfWork;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System;
using System.Data;
using System.Threading.Tasks;

namespace Delosi.Master.Infraestructure.Repositories.Writer
{
    internal class DealRepositoryWriter : BaseRepositoryUoW, IDealRepositoryWriter
    {
        private readonly IDbConnectionFactory _dbConnection;
        public DealRepositoryWriter(IDbConnectionFactory dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<int> CreateDeal(DealInputModel request)
        {
            var parameters = new OracleDynamicParameters();
            parameters.Add("vID_MARCA", OracleDbType.Int32, ParameterDirection.Input, request.BrandId);
            parameters.Add("vID_CANAL", OracleDbType.Int32, ParameterDirection.Input, request.ChannelId);
            parameters.Add("vDES_NOMBRE", OracleDbType.Varchar2, ParameterDirection.Input, request.DealName);
            parameters.Add("vDES_TITULO", OracleDbType.Varchar2, ParameterDirection.Input, request.DealTitle);
            parameters.Add("vDES_DESCRIPCION", OracleDbType.Varchar2, ParameterDirection.Input, request.DealDescription);
            parameters.Add("vDES_URL_ITEM", OracleDbType.Varchar2, ParameterDirection.Input, request.DealUrlItem);
            parameters.Add("vDES_TAG_NAME", OracleDbType.Varchar2, ParameterDirection.Input, request.DealTagName);
            parameters.Add("vFLG_ESTADO", OracleDbType.Varchar2, ParameterDirection.Input, request.Status);
            parameters.Add("vORDEN", OracleDbType.Int32, ParameterDirection.Input, request.NroOrder);
            parameters.Add("vFLG_SEMANA_COMPLETA", OracleDbType.Varchar2, ParameterDirection.Input, request.FlgAllWeek);
            parameters.Add("vFCH_FEC_INI", OracleDbType.Date, ParameterDirection.Input, request.DateStart);
            parameters.Add("vFCH_FEC_FIN", OracleDbType.Date, ParameterDirection.Input, request.DateEnd);
            parameters.Add("vHRS_HOR_INI", OracleDbType.Varchar2, ParameterDirection.Input, request.StartHour);
            parameters.Add("vHRS_HOR_FIN", OracleDbType.Varchar2, ParameterDirection.Input, request.EndHour);
            parameters.Add("vINTERVALO_DIAS", OracleDbType.Varchar2, ParameterDirection.Input, request.IntervalDays);
            parameters.Add("vCOD_USR_CREA", OracleDbType.Varchar2, ParameterDirection.Input, request.CreationUser);
            parameters.Add("vID_OFERTA", OracleDbType.Int32, direction: ParameterDirection.Output);

            using (var dbConnection = new OracleConnection(_dbConnection.GetConnectionOracle()))
            {
                await dbConnection.ExecuteAsync($"{Schema.DATABASE_SCHEMA_DEAL}USP_INS_GEN_OFERTA",
                parameters,
                commandType: CommandType.StoredProcedure);

                return !parameters.Get<OracleDecimal>("vID_OFERTA").IsNull ?
                    Convert.ToInt32(parameters.Get<OracleDecimal>("vID_OFERTA").ToString())
                    : 0;
            }
        }

        public async Task UpdateDeal(DealInputModel request)
        {
            var parameters = new OracleDynamicParameters();
            parameters.Add("vID_OFERTA", OracleDbType.Int32, ParameterDirection.Input, request.DealId);
            parameters.Add("vID_MARCA", OracleDbType.Int32, ParameterDirection.Input, request.BrandId);
            parameters.Add("vID_CANAL", OracleDbType.Int32, ParameterDirection.Input, request.ChannelId);
            parameters.Add("vDES_NOMBRE", OracleDbType.Varchar2, ParameterDirection.Input, request.DealName);
            parameters.Add("vDES_TITULO", OracleDbType.Varchar2, ParameterDirection.Input, request.DealTitle);
            parameters.Add("vDES_DESCRIPCION", OracleDbType.Varchar2, ParameterDirection.Input, request.DealDescription);
            parameters.Add("vDES_URL_ITEM", OracleDbType.Varchar2, ParameterDirection.Input, request.DealUrlItem);
            parameters.Add("vDES_TAG_NAME", OracleDbType.Varchar2, ParameterDirection.Input, request.DealTagName);
            parameters.Add("vFLG_ESTADO", OracleDbType.Varchar2, ParameterDirection.Input, request.Status);
            parameters.Add("vORDEN", OracleDbType.Int32, ParameterDirection.Input, request.NroOrder);
            parameters.Add("vFLG_SEMANA_COMPLETA", OracleDbType.Varchar2, ParameterDirection.Input, request.FlgAllWeek);
            parameters.Add("vFCH_FEC_INI", OracleDbType.Date, ParameterDirection.Input, request.DateStart);
            parameters.Add("vFCH_FEC_FIN", OracleDbType.Date, ParameterDirection.Input, request.DateEnd);
            parameters.Add("vHRS_HOR_INI", OracleDbType.Varchar2, ParameterDirection.Input, request.StartHour);
            parameters.Add("vHRS_HOR_FIN", OracleDbType.Varchar2, ParameterDirection.Input, request.EndHour);
            parameters.Add("vINTERVALO_DIAS", OracleDbType.Varchar2, ParameterDirection.Input, request.IntervalDays);
            parameters.Add("vID_IMAGEN", OracleDbType.Int32, ParameterDirection.Input, request.ImageId);
            parameters.Add("vCOD_USR_MOD", OracleDbType.Varchar2, ParameterDirection.Input, request.CreationUser);
            using (var dbConnection = new OracleConnection(_dbConnection.GetConnectionOracle()))
            {
                await dbConnection.ExecuteAsync($"{Schema.DATABASE_SCHEMA_DEAL}USP_UPD_GEN_OFERTA",
                         parameters,
                         commandType: CommandType.StoredProcedure);
            }
        }

        public async Task DeleteDeal(int dealId)
        {
            var parameters = new OracleDynamicParameters();
            parameters.Add("vID_OFERTA", OracleDbType.Int32, ParameterDirection.Input, dealId);
            using (var dbConnection = new OracleConnection(_dbConnection.GetConnectionOracle()))
            {
                await dbConnection.ExecuteAsync($"{Schema.DATABASE_SCHEMA_DEAL}USP_DEL_GEN_OFERTA",
                parameters,
                commandType: CommandType.StoredProcedure);
            }
        }

        public async Task UpdateImageIdDeal(int dealId, int imageId)
        {
            var parameters = new OracleDynamicParameters();
            parameters.Add("vID_OFERTA", OracleDbType.Int32, ParameterDirection.Input, dealId);
            parameters.Add("vID_IMAGEN", OracleDbType.Int32, ParameterDirection.Input, imageId);
            using (var dbConnection = new OracleConnection(_dbConnection.GetConnectionOracle()))
            {
                await dbConnection.ExecuteAsync($"{Schema.DATABASE_SCHEMA_DEAL}USP_UPD_IMAGE_ID_OFERTA",
                parameters,
                commandType: CommandType.StoredProcedure);
            }
        }
    }
}
