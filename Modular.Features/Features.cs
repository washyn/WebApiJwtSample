using Modular.Lib;

namespace Modular.Features;

public class Features : IModule
{
    public string Name => "Features";

    public void LoadModule()
    {
        Console.WriteLine( "Features loaded!");
    }
}