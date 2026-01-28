using System.Collections.Generic;
using System.Threading.Tasks;

namespace EtlDapper;

public interface IDataSource<T>
{
    Task<Batch<T>> FetchBatchAsync(long lastId, int batchSize);
}

public interface ITransform<TIn, TOut>
{
    Task<List<TOut>> TransformAsync(List<TIn> input);
}

public interface IDataDestination<T>
{
    Task InitializeAsync();
    Task WriteBatchAsync(List<T> items);
}

public record Batch<T>(List<T> Items, long LastId);

public class EtlPipeline<TIn, TOut>
{
    private readonly IDataSource<TIn> _source;
    private readonly ITransform<TIn, TOut> _transform;
    private readonly IDataDestination<TOut> _destination;
    private readonly int _batchSize;

    public EtlPipeline(IDataSource<TIn> source, ITransform<TIn, TOut> transform, IDataDestination<TOut> destination, int batchSize)
    {
        _source = source;
        _transform = transform;
        _destination = destination;
        _batchSize = batchSize;
    }

    public async Task RunAsync()
    {
        await _destination.InitializeAsync();
        long last = 0;
        while (true)
        {
            var batch = await _source.FetchBatchAsync(last, _batchSize);
            if (batch.Items.Count == 0) break;
            var transformed = await _transform.TransformAsync(batch.Items);
            await _destination.WriteBatchAsync(transformed);
            last = batch.LastId;
        }
    }
}
