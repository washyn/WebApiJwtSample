
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using RestSharp;
using Serilog;

namespace ConsoleApp
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .WriteTo.File("Logs/log.log", shared: true, rollingInterval: RollingInterval.Hour)
                .CreateLogger();
            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
            // this has 6369 elements
            var fileName = "C:\\Users\\WashingtonAceroMaman\\source-code\\productos_KFC_2026_01_14.json";
            var filePath = fileName;
            var requestJson = File.ReadAllText(filePath);
            // parse to json and count data
            var data = JsonConvert.DeserializeObject<MenuItemRequest>(requestJson);
            Log.Information("Data count: {Count}", data.MenuItems.Count);
            // TODO: take 1000 elements and send to lambda

            var collections = new List<string>();
            for (int i = 0; i < data.MenuItems.Count; i += 1000)
            {
                var temp = data.MenuItems.Skip(i).Take(1000);
                Log.Information("Temp count: {Count}", temp.Count());
                var newRequestJson = JsonConvert.SerializeObject(new MenuItemRequest
                    { MenuItems = temp.ToList(), ServerName = data.ServerName });
                collections.Add(newRequestJson);
                Log.Information("Collection: {Collection}", newRequestJson);
            }

            var url = "https://pb38g7q36b.execute-api.us-east-1.amazonaws.com/dev/syncMicrosProducts";

            // TODO: save chunks collection in files
            var index = 1;
            foreach (var element in collections)
            {
                var patth = Path.Combine(Directory.GetCurrentDirectory(), $"chunk{index}.json");
                File.WriteAllText(patth, element);
                index++;
            }

            foreach (var element in collections)
            {
                var restClient = new RestClient();
                var restRequest = new RestRequest(url);
                restRequest.AddHeader("content-type", "application/json");
                restRequest.AddHeader("x-signature", "e2481bb4-03aa-48ed-98d9-295f2907faae");
                restRequest.AddStringBody(element, DataFormat.Json);
                var resultRequest = await restClient.PostAsync(restRequest);
                Log.Information("Result: {StatusCode}", resultRequest.StatusCode);
                Log.Information("Result: {IsSuccessStatusCode}", resultRequest.IsSuccessStatusCode);
                Log.Information("Result: {Content}", resultRequest.Content);
                Log.Information("Result end...");
            }
        }
    }

    public class MenuItemRequest
    {
        [JsonProperty("serverName")] public string ServerName { get; set; }

        [JsonProperty("menuItems")] public List<MenuItem> MenuItems { get; set; }
    }

    public class MenuItem
    {
        public int Id { get; set; }
        [JsonProperty("EM")] public string EM { get; set; }

        [JsonProperty("plu")] public int Plu { get; set; }

        [JsonProperty("name")] public string Name { get; set; }

        [JsonProperty("secondName")] public string SecondName { get; set; }

        [JsonProperty("priceGroup")] public PriceGroup PriceGroup { get; set; }

        [JsonProperty("comboMeal")] public ComboMeal ComboMeal { get; set; }

        [JsonProperty("menuLevelClass")] public MenuLevelClass MenuLevelClass { get; set; }

        [JsonProperty("menuItemClass")] public MenuItemClass MenuItemClass { get; set; }

        [JsonProperty("versions")] public List<Version> Versions { get; set; }
        public string ModifiedBy { get; set; }
        public string ModifiedDate { get; set; }
    }

    public class PriceGroup
    {
        [JsonProperty("id")] public int Id { get; set; }

        [JsonProperty("name")] public string Name { get; set; }
    }

    public class ComboMeal
    {
        [JsonProperty("id")] public int Id { get; set; }

        [JsonProperty("name")] public string Name { get; set; }
    }

    public class MenuLevelClass
    {
        [JsonProperty("id")] public int Id { get; set; }

        [JsonProperty("name")] public string Name { get; set; }
    }

    public class MenuItemClass
    {
        [JsonProperty("id")] public int Id { get; set; }

        [JsonProperty("name")] public string Name { get; set; }
    }


    public class Version
    {
        public int VersionId { get; set; }
        public string VersionName { get; set; }
        [JsonProperty("menuLevelClass")] public MenuLevelClass MenuLevelClass { get; set; }

        [JsonProperty("menuItemClass")] public MenuItemClass MenuItemClass { get; set; }
        public string ModifiedBy { get; set; }
        public string ModifiedDate { get; set; }
    }
}