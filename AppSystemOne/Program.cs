using NLaya;
using NLaya.TorchSharp;

namespace AppSystemOne;

public class Program
{
    public static async Task Main(string[] args)
    {
        await using var agent = await Laya.LoadAsync(
            "convaiinnovations/laya-multilingual",
            o => o.UseTorchSharp()
        );

        var questions = new Questions()
            .Choice(
                "department",
                "¿A qué área corresponde este ticket?",
                ("network", "Problemas de conexión y redes"),
                ("billing", "Facturación y pagos"),
                ("support", "Soporte técnico general")
            )
            .Noul(
                "urgent",
                "¿El problema requiere atención urgente?"
            );

        var result = agent.Predict(
            new
            {
                body = "No tengo conexión a Internet desde hace dos horas."
            },
            questions
        );

        Console.WriteLine(result.Choice("department").Choice);
        Console.WriteLine(result.Noul("urgent").Value);
        Console.WriteLine(result.ToJsonString(indented: true));
    }
}
// TODO: test with microsoft model