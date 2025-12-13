namespace Modular.Lib;

public interface IModule
{
    public string Name { get; }
    void LoadModule();
}