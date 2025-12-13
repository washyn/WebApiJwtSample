using Modular.Lib;

namespace Modular.Main
{
    public class Program
    {
        // la interfaz podria estar dentro de este mismo ensamblado y solo plublicar el dll...
        public static void Main(string[] args)
        {
            var plugin = "D:\\git-proyects\\CodeExamples\\Modular.Features\\bin\\Debug\\net8.0\\Modular.Features.dll";
            var pluginAssembly = System.Reflection.Assembly.LoadFrom(plugin);
            var pluginType = pluginAssembly.GetTypes()
                .Where(t => typeof(IModule).IsAssignableFrom(t) && !t.IsAbstract && !t.IsInterface);
            foreach (var type in pluginType)
            {
                var pluginInstance = (IModule)Activator.CreateInstance(type)!;
                Console.WriteLine("Ejecutando modulo: " + pluginInstance.Name);
                pluginInstance.LoadModule();
            }
        }
    }
}