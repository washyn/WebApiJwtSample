using System.ComponentModel;
using Dapper;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


using (var sqlConnextion = new SqlConnection("Server=(LocalDb)\\MSSQLLocalDB;Database=DB_SDM;Trusted_Connection=True;TrustServerCertificate=True"))
{
    var query = "select * from TIPO_SERVICIOS";
    var data = sqlConnextion.Query(query).ToList();

    foreach (var item in data)
    {
        var json = JsonConvert.SerializeObject(item);
        var jobj = JObject.Parse(json);
        Console.WriteLine(jobj["TSER_Codigo"]);
    }
}