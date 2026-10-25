using Dapper;
using Delosi.Orders.Domain.Contracts.DataTransferObjects.InputModel;
using Delosi.Orders.Domain.Contracts.DataTransferObjects.OutputModel;
using Delosi.Orders.Infrastructure.Conexion;
using Delosi.Orders.Infrastructure.Constants;
using Delosi.Orders.Infrastructure.Interfaces;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System.Data;

namespace Delosi.Orders.Infrastructure.AccesoDatos
{
    public class CanalAccesoDatos : ICanalAccesoDatos
    {
        private readonly IServidorOracle _servidorOracle;

        public CanalAccesoDatos(IServidorOracle servidorOracle)
        {
            _servidorOracle = servidorOracle;
        }
        public async Task<IEnumerable<CanalMarcaDomainOutputModel>> ObtenerCanalMarca(CanalMarcaDomainInputModel request)
        {
            var parameter = new OracleDynamicParametersNew();
            parameter.Add("vID_CANAL", OracleDbType.Varchar2, ParameterDirection.Input, request.IdCanal);
            parameter.Add("vDES_ABREVIA", OracleDbType.Varchar2, ParameterDirection.Input, request.DescripcionBreveMarca);
            parameter.Add("CUR_RESULTADO", OracleDbType.RefCursor, ParameterDirection.Output);

            using (var connection = new OracleConnection(_servidorOracle.ObtenerConexion()))
            {
                connection.Open();
                var result = await connection.QueryAsync<CanalMarcaDomainOutputModel>(ProcedimientoAlmacenado.ValidarCanalMarca,
                parameter,
                commandType: CommandType.StoredProcedure);
                connection.Close();
                return result;
            }
        }

        public async Task<Dictionary<string, string>> ValidarCanalCodigoExterno(int idCanal, string codigoExterno)
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            var parameter = new OracleDynamicParametersNew();
            parameter.Add("vID_CANAL", OracleDbType.Varchar2, ParameterDirection.Input, idCanal);
            parameter.Add("vCODIGO_EXTERNO", OracleDbType.Varchar2, ParameterDirection.Input, codigoExterno);
            parameter.Add("vRESULTADO", OracleDbType.Int16, ParameterDirection.Output);
            parameter.Add("vDESCANAL", OracleDbType.Varchar2, ParameterDirection.Output, size: 100);
            parameter.Add("vFLG_BONUS", OracleDbType.Varchar2, ParameterDirection.Output, size: 1);

            using (var connection = new OracleConnection(_servidorOracle.ObtenerConexion()))
            {
                // connection.Open();
                var result = await connection.ExecuteAsync(ProcedimientoAlmacenado.ValidarCanalCodigoExterno,
                                                                                        parameter,
                                                                                        commandType: CommandType.StoredProcedure);
                connection.Close();
                dictionary.Add("isRegister", (parameter.Get<OracleDecimal>("vRESULTADO").ToInt16() > 0).ToString());
                dictionary.Add("canal", parameter.Get<OracleString>("vDESCANAL").ToString());
                dictionary.Add("flgBonus", parameter.Get<OracleString>("vFLG_BONUS").ToString());
                return dictionary;
            }
        }
        public async Task<IEnumerable<CanalOutputModel>> ObtenerCanal(CanalInputModel request)
        {
            var parameter = new OracleDynamicParametersNew();
            parameter.Add("vID_CANAL", OracleDbType.Int32, ParameterDirection.Input, request.IdCanal);
            parameter.Add("vDES_ABREVIA", OracleDbType.Varchar2, ParameterDirection.Input, request.Nombre);
            parameter.Add("CUR_RESULTADO", OracleDbType.RefCursor, ParameterDirection.Output);

            using (var connection = new OracleConnection(_servidorOracle.ObtenerConexion()))
            {
                connection.Open();
                var result = await connection.QueryAsync<CanalOutputModel>(ProcedimientoAlmacenado.ValidarCanal,
                parameter,
                commandType: CommandType.StoredProcedure);
                connection.Close();
                return result;
            }
        }
        public async Task<IEnumerable<MarcaCanalMenuTypeDomainOutputModel>> ObtenerConfiguracionMarcaCanalMenuType(MarcaCanalMenuTypeDomainInputModel request)
        {
            var parameter = new OracleDynamicParametersNew();
            parameter.Add("vID_MARCA", OracleDbType.Int32, ParameterDirection.Input, request.IdMarca);
            parameter.Add("vID_CANAL", OracleDbType.Int32, ParameterDirection.Input, request.IdCanal);
            parameter.Add("vID_MENUTYPE", OracleDbType.Int32, ParameterDirection.Input, request.IdMenuType);
            parameter.Add("vID_TIENDA", OracleDbType.Int32, ParameterDirection.Input, request.IdTienda);
            parameter.Add("CUR_RESULTADO", OracleDbType.RefCursor, ParameterDirection.Output);


            using (var connection = new OracleConnection(_servidorOracle.ObtenerConexion()))
            {
                var result = await connection.QueryAsync<MarcaCanalMenuTypeDomainOutputModel>(ProcedimientoAlmacenado.ValidarMarcaCanalMenuType,
                parameter,
                commandType: CommandType.StoredProcedure).ConfigureAwait(false);

                return result;
            }
        }
        public async Task<IEnumerable<TiendaConfigDomainOutputModel>> ObtenerConfiguracionTienda(int idTienda)
        {
            var parameter = new OracleDynamicParametersNew();
            parameter.Add("vID_TIENDA", OracleDbType.Int32, ParameterDirection.Input, idTienda);
            parameter.Add("CUR_RESULTADO", OracleDbType.RefCursor, ParameterDirection.Output);

            using (var connection = new OracleConnection(_servidorOracle.ObtenerConexion()))
            {
                connection.Open();
                var result = await connection.QueryAsync<TiendaConfigDomainOutputModel>(ProcedimientoAlmacenado.ValidarTienda,
                parameter,
                commandType: CommandType.StoredProcedure);
                connection.Close();
                return result;
            }
        }
    }
}
