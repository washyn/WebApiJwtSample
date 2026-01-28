using Volo.Abp.DependencyInjection;

namespace EtlDapper;

public class ReadmeEtlRunner : ITransientDependency
{
    private readonly DatitoSource _source;
    private readonly IdentityTransform<PeopleRecord> _transform;
    private readonly PeoplesDestination _destination;

    public ReadmeEtlRunner(DatitoSource source, IdentityTransform<PeopleRecord> transform, PeoplesDestination destination)
    {
        _source = source;
        _transform = transform;
        _destination = destination;
    }

    public async Task RunAsync()
    {
        var pipeline = new EtlPipeline<PeopleRecord, PeopleRecord>(_source, _transform, _destination, 10_000);
        await pipeline.RunAsync();
    }
}
